using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

public class BisonFactory : WebApplicationFactory<Program>
{
	private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

	public BisonFactory() { }

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

			var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<BisonDbContext>();
			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();

			var taxon = new Taxon
			{
				TaxonId = 1,
				DwcTaxonId = "TEST",
				VernacularName = "Bird",
			};
			db.Taxons.Add(taxon);

			var mette = new User
			{
				UserId = 1,
				Username = "Mette",
				Email = "mette@test.com",
				PwHash = "",
			};
			var adrian = new User
			{
				UserId = 2,
				Username = "Adrian",
				Email = "adrian@test.com",
				PwHash = "",
			};
			db.Users.AddRange(mette, adrian);

			var o1 = new Observation
			{
				PostId = 1,
				Author = mette,
				Taxon = taxon,
				Text =
					"Found a Glossy Ibis in the marsh behind the dunes. Bare skin on the face and head.",
				PubDate = DateTime.Parse("2026-03-04 07:11:53"),
			};
			var o2 = new Observation
			{
				PostId = 2,
				Author = adrian,
				Taxon = taxon,
				Text = "A horse",
				PubDate = DateTime.Parse("2023-08-01 12:16:49"),
			};
			var o3 = new Observation
			{
				PostId = 3,
				Author = adrian,
				Taxon = taxon,
				Text = "A bird",
				PubDate = DateTime.Parse("2023-08-01 12:16:47"),
			};
			db.Observations.AddRange(o1, o2, o3);
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

public class APIUnitTest : IClassFixture<BisonFactory>
{
	private readonly HttpClient _client;
	private readonly ITestOutputHelper _output;

	public APIUnitTest(BisonFactory factory, ITestOutputHelper output)
	{
		_client = factory.CreateClient();
		_output = output;
	}

	[Fact]
	public async Task ObsEndpointGetRequestTest()
	{
		var response = await _client.GetAsync("/obs");
		var body = await response.Content.ReadAsStringAsync();

		_output.WriteLine($"Status: {response.StatusCode}");
		_output.WriteLine(body);

		response.EnsureSuccessStatusCode();
		Assert.Contains("Showing 3 observations", body);

		Assert.Contains("Mette", body);
		Assert.Contains(
			"Found a Glossy Ibis in the marsh behind the dunes. Bare skin on the face and head.",
			body
		);
		Assert.True(body.Contains("03/04/26 7:11:53") || body.Contains("03/04/26 7.11.53"));

		Assert.Contains("Adrian", body);
		Assert.Contains("A horse", body);
		Assert.True(body.Contains("08/01/23 12:16:49") || body.Contains("08/01/23 12.16.49"));

		Assert.Contains("Adrian", body);
		Assert.Contains("A bird", body);
		Assert.True(body.Contains("08/01/23 12:16:47") || body.Contains("08/01/23 12.16.47"));
	}

	[Fact]
	public async Task AutherEndpointGetRequestTest()
	{
		var response = await _client.GetAsync("/obs/Adrian");
		var body = await response.Content.ReadAsStringAsync();

		_output.WriteLine($"Status: {response.StatusCode}");
		_output.WriteLine(body);

		response.EnsureSuccessStatusCode();
		Assert.Contains("Showing 2 observations", body);

		Assert.DoesNotContain("Mette", body);
		Assert.DoesNotContain(
			"Found a Glossy Ibis in the marsh behind the dunes. Bare skin on the face and head.",
			body
		);
		Assert.True(!body.Contains("03/04/26 7:11:53") && !body.Contains("03/04/26 7.11.53"));

		Assert.Contains("Adrian", body);
		Assert.Contains("A horse", body);
		Assert.True(body.Contains("08/01/23 12:16:49") || body.Contains("08/01/23 12.16.49"));

		Assert.Contains("Adrian", body);
		Assert.Contains("A bird", body);
		Assert.True(body.Contains("08/01/23 12:16:47") || body.Contains("08/01/23 12.16.47"));
	}
}
