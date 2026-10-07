using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? envPath = builder.Configuration["BISONDBPATH"];
string dbPath = "../Database/";
if (string.IsNullOrWhiteSpace(envPath))
{
	dbPath = Path.Combine(Path.GetTempPath(), "bison.db");
}
else
{
	dbPath += envPath;
}
Console.WriteLine($"Using database: {Path.GetFullPath(dbPath)}");

string connectionString = $"Data Source={dbPath}";

// Add services to the container.
builder.Services.AddDbContext<BisonDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddRazorPages();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IObservationService, ObservationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.Configure<RouteOptions>(options =>
{
	options.LowercaseUrls = true;
});

builder
	.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/login";
		options.LogoutPath = "/logout";
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.Cookie.SameSite = SameSiteMode.Strict;
		options.ExpireTimeSpan = TimeSpan.FromDays(7);
		options.SlidingExpiration = true;
		options.Events = new CookieAuthenticationEvents
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
					await context.HttpContext.SignOutAsync(
						CookieAuthenticationDefaults.AuthenticationScheme
					);
				}
			},
		};
	});
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<BisonDbContext>();

		// In development we don't care about database persistence, so we just delete it on each run
		// This allows us to make changes to the database schema without having to worry about migrations or data persistence.
		if (app.Environment.IsDevelopment())
		{
			dbContext.Database.EnsureDeleted();
		}

		// Ensure the database is created and apply any pending migrations
		dbContext.Database.EnsureCreated();
		DbInitializer.SeedDatabase(dbContext);
	}
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/", () => Results.Redirect("/obs"));
app.MapGet("/public", () => Results.Redirect("/obs"));

app.Run();

public partial class Program { }
