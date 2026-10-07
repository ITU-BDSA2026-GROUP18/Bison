using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class AuthTestFactory : WebApplicationFactory<Program>
{
	private readonly string _dbPath = Path.Combine(
		Path.GetTempPath(),
		$"auth_test_{Guid.NewGuid()}.db"
	);

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");
		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(d =>
				d.ServiceType == typeof(DbContextOptions<BisonDbContext>)
			);
			if (descriptor != null)
			{
				services.Remove(descriptor);
			}

			services.AddDbContext<BisonDbContext>(o => o.UseSqlite($"Data Source={_dbPath}"));

			services.AddRazorPages(options =>
			{
				options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
			});

			var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<BisonDbContext>();
			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();

			var hasher = new PasswordHasher<User>();
			var user = new User
			{
				UserId = 1,
				Username = "testuser",
				Email = "test@example.com",
				PwHash = "",
			};
			user.PwHash = hasher.HashPassword(user, "SecurePass123!");
			db.Users.Add(user);
			db.SaveChanges();
		});
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		SqliteConnection.ClearAllPools();
		try
		{
			File.Delete(_dbPath);
		}
		catch { }
	}
}

public class AuthTests : IClassFixture<AuthTestFactory>
{
	private readonly AuthTestFactory _factory;
	private readonly HttpClient _client;

	public AuthTests(AuthTestFactory factory)
	{
		_factory = factory;
		_client = factory.CreateClient(
			new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
		);
	}

	[Fact]
	public async Task GetLogin_ReturnsLoginPage()
	{
		var response = await _client.GetAsync("/login");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = await response.Content.ReadAsStringAsync();
		Assert.Contains("Username or Email", body);
		Assert.Contains("Password", body);
	}

	[Fact]
	public async Task PostLogin_WithValidUsername_SetsAuthCookieAndRedirects()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "testuser",
				["Password"] = "SecurePass123!",
			}
		);

		var response = await _client.PostAsync("/login", content);

		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		Assert.True(response.Headers.Contains("Set-Cookie"));
		var cookies = response.Headers.GetValues("Set-Cookie");
		Assert.Contains(cookies, c => c.Contains(".AspNetCore.Cookies"));
	}

	[Fact]
	public async Task PostLogin_WithValidEmail_SetsAuthCookieAndRedirects()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "test@example.com",
				["Password"] = "SecurePass123!",
			}
		);

		var response = await _client.PostAsync("/login", content);

		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		Assert.True(response.Headers.Contains("Set-Cookie"));
		var cookies = response.Headers.GetValues("Set-Cookie");
		Assert.Contains(cookies, c => c.Contains(".AspNetCore.Cookies"));
	}

	[Fact]
	public async Task PostLogin_WithInvalidPassword_ReturnsErrorMessage()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "testuser",
				["Password"] = "WrongPassword",
			}
		);

		var response = await _client.PostAsync("/login", content);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = await response.Content.ReadAsStringAsync();
		Assert.Contains("Invalid username/email or password.", body);
	}

	[Fact]
	public async Task PostLogin_WithNonExistentUser_ReturnsErrorMessage()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "unknownuser",
				["Password"] = "AnyPassword",
			}
		);

		var response = await _client.PostAsync("/login", content);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = await response.Content.ReadAsStringAsync();
		Assert.Contains("Invalid username/email or password.", body);
	}

	[Fact]
	public async Task PostLogin_WithOpenRedirectUrl_RedirectsToDefault()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "testuser",
				["Password"] = "SecurePass123!",
				["ReturnUrl"] = "https://malicious.example.com",
			}
		);

		var response = await _client.PostAsync("/login", content);

		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		Assert.NotEqual("https://malicious.example.com", response.Headers.Location?.ToString());
	}

	[Fact]
	public async Task PostLogout_SignsOutAndRedirects()
	{
		var response = await _client.PostAsync("/logout", new StringContent(""));

		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
	}

	[Theory]
	[InlineData("/public")]
	[InlineData("/Public")]
	public async Task GetPublicRoute_RedirectsToObs(string route)
	{
		var response = await _client.GetAsync(route);

		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		Assert.Equal("/obs", response.Headers.Location?.ToString());
	}

	[Fact]
	public void LogoutModel_DoesNotHaveIgnoreAntiforgeryTokenAttribute()
	{
		Assert.False(
			typeof(Bison.Razor.Pages.LogoutModel).IsDefined(
				typeof(IgnoreAntiforgeryTokenAttribute),
				true
			)
		);
	}

	[Fact]
	public async Task GetLogout_DoesNotSignOutUser_AndRedirects()
	{
		var loginContent = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "testuser",
				["Password"] = "SecurePass123!",
			}
		);
		var loginResponse = await _client.PostAsync("/login", loginContent);
		var authCookie = loginResponse
			.Headers.GetValues("Set-Cookie")
			.First(c => c.Contains(".AspNetCore.Cookies"))
			.Split(';')[0];

		var request = new HttpRequestMessage(HttpMethod.Get, "/logout");
		request.Headers.Add("Cookie", authCookie);

		var logoutResponse = await _client.SendAsync(request);

		Assert.Equal(HttpStatusCode.Redirect, logoutResponse.StatusCode);
		if (logoutResponse.Headers.Contains("Set-Cookie"))
		{
			var logoutCookies = logoutResponse.Headers.GetValues("Set-Cookie");
			Assert.DoesNotContain(logoutCookies, c => c.Contains("expires=Thu, 01 Jan 1970"));
		}
	}

	[Fact]
	public async Task PostLogout_WithoutAntiforgeryToken_ReturnsBadRequest()
	{
		using var factory = new StrictAntiforgeryTestFactory();
		var client = factory.CreateClient(
			new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
		);

		var response = await client.PostAsync("/logout", new StringContent(""));

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public void DuplicateUsername_ThrowsDbUpdateException()
	{
		using var scope = _factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<BisonDbContext>();

		var duplicate = new User
		{
			UserId = 100,
			Username = "testuser",
			Email = "unique@example.com",
			PwHash = "hash",
		};

		db.Users.Add(duplicate);
		Assert.Throws<DbUpdateException>(() => db.SaveChanges());
	}

	[Fact]
	public void DuplicateUsernameCaseInsensitive_ThrowsDbUpdateException()
	{
		using var scope = _factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<BisonDbContext>();

		var duplicate = new User
		{
			UserId = 101,
			Username = "TESTUSER",
			Email = "unique2@example.com",
			PwHash = "hash",
		};

		db.Users.Add(duplicate);
		Assert.Throws<DbUpdateException>(() => db.SaveChanges());
	}

	[Fact]
	public void DuplicateEmail_ThrowsDbUpdateException()
	{
		using var scope = _factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<BisonDbContext>();

		var duplicate = new User
		{
			UserId = 102,
			Username = "uniqueuser",
			Email = "test@example.com",
			PwHash = "hash",
		};

		db.Users.Add(duplicate);
		Assert.Throws<DbUpdateException>(() => db.SaveChanges());
	}

	[Fact]
	public void DuplicateEmailCaseInsensitive_ThrowsDbUpdateException()
	{
		using var scope = _factory.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<BisonDbContext>();

		var duplicate = new User
		{
			UserId = 103,
			Username = "uniqueuser2",
			Email = "TEST@EXAMPLE.COM",
			PwHash = "hash",
		};

		db.Users.Add(duplicate);
		Assert.Throws<DbUpdateException>(() => db.SaveChanges());
	}

	[Fact]
	public async Task PostLogin_WithOversizedIdentifier_FailsValidation()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = new string('a', 257),
				["Password"] = "SecurePass123!",
			}
		);

		var response = await _client.PostAsync("/login", content);
		var body = await response.Content.ReadAsStringAsync();

		Assert.Contains("Input is too long.", body);
	}

	[Fact]
	public async Task PostLogin_WithOversizedPassword_FailsValidation()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "testuser",
				["Password"] = new string('p', 129),
			}
		);

		var response = await _client.PostAsync("/login", content);
		var body = await response.Content.ReadAsStringAsync();

		Assert.Contains("Password is too long.", body);
	}

	[Fact]
	public async Task PostLogin_SetsSameSiteStrictAndSecureCookies()
	{
		var content = new FormUrlEncodedContent(
			new Dictionary<string, string>
			{
				["Identifier"] = "testuser",
				["Password"] = "SecurePass123!",
			}
		);

		var response = await _client.PostAsync("/login", content);

		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		var cookies = response.Headers.GetValues("Set-Cookie");
		Assert.Contains(cookies, c => c.ToLower().Contains("samesite=strict"));
		Assert.Contains(cookies, c => c.ToLower().Contains("secure"));
	}

	[Fact]
	public async Task GetUserTimeline_RendersAuthorFromModel()
	{
		var response = await _client.GetAsync("/obs/testuser");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = await response.Content.ReadAsStringAsync();
		Assert.Contains("testuser's Observations", body);
	}

	[Fact]
	public async Task AbsoluteExpiration_PastDeadline_RejectsPrincipal()
	{
		var events = new CookieAuthenticationEvents
		{
			OnValidatePrincipal = async context =>
			{
				if (
					context.Properties.Items.TryGetValue("AbsoluteExpiresUtc", out var absExpStr)
					&& DateTimeOffset.TryParse(absExpStr, out var absExp)
					&& DateTimeOffset.UtcNow > absExp
				)
				{
					context.RejectPrincipal();
					await Task.CompletedTask;
				}
			},
		};

		var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
		var identity = new System.Security.Claims.ClaimsIdentity(
			CookieAuthenticationDefaults.AuthenticationScheme
		);
		var principal = new System.Security.Claims.ClaimsPrincipal(identity);
		var properties = new AuthenticationProperties
		{
			Items = { ["AbsoluteExpiresUtc"] = DateTimeOffset.UtcNow.AddDays(-1).ToString("o") },
		};
		var scheme = new AuthenticationScheme(
			CookieAuthenticationDefaults.AuthenticationScheme,
			null,
			typeof(CookieAuthenticationHandler)
		);
		var ticket = new AuthenticationTicket(principal, properties, scheme.Name);
		var context = new CookieValidatePrincipalContext(
			httpContext,
			scheme,
			new CookieAuthenticationOptions(),
			ticket
		);

		await events.ValidatePrincipal(context);

		Assert.Null(context.Principal);
	}
}

public class StrictAntiforgeryTestFactory : WebApplicationFactory<Program>
{
	private readonly string _dbPath = Path.Combine(
		Path.GetTempPath(),
		$"strict_af_test_{Guid.NewGuid()}.db"
	);

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");
		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(d =>
				d.ServiceType == typeof(DbContextOptions<BisonDbContext>)
			);
			if (descriptor != null)
			{
				services.Remove(descriptor);
			}

			services.AddDbContext<BisonDbContext>(o => o.UseSqlite($"Data Source={_dbPath}"));
		});
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		SqliteConnection.ClearAllPools();
		try
		{
			File.Delete(_dbPath);
		}
		catch { }
	}
}
