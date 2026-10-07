using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

public class BisonFactory : WebApplicationFactory<Program>
{
	private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

	public BisonFactory()
	{
		//copies the soruce db to not make any lasting changes
		var source = Path.Combine(AppContext.BaseDirectory, "data", "apitestdata.db");
		File.Copy(source, _dbPath);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");
		builder.ConfigureServices(services =>
		{
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
		//_output.WriteLine($"Status: {response.StatusCode}");
		_output.WriteLine(body);

		//response.EnsureSuccessStatusCode();
		Assert.Contains("Showing 5 observations", body);

		Assert.Contains("Roger Histand", body);
		Assert.Contains("Grey heron.", body);
		Assert.Contains("08/01/23 7.12.00", body);

		Assert.Contains("Roger Histand", body);
		Assert.Contains("Large white heron in a wet meadow.", body);
		Assert.Contains("08/02/23 6.45.00", body);

		Assert.Contains("Roger Histand", body);
		Assert.Contains("Heard a booming call from the reed bed at dusk.", body);
		Assert.Contains("08/03/23 21.10.00", body);

		Assert.Contains("Luanna Muro", body);
		Assert.Contains("Gannets diving offshore.", body);
		Assert.Contains("08/02/23 11.05.00", body);

		Assert.Contains("Luanna Muro", body);
		Assert.Contains("Flock of about 40 cormorants.", body);
		Assert.Contains("08/01/23 8.30.00", body);
	}

	[Fact]
	public async Task AutherEndpointGetRequestTest()
	{
		var response = await _client.GetAsync("/obs/Luanna Muro");
		var body = await response.Content.ReadAsStringAsync();

		_output.WriteLine($"Status: {response.StatusCode}");
		_output.WriteLine(body);

		response.EnsureSuccessStatusCode();
		Assert.Contains("Showing 2 observations", body);

		Assert.DoesNotContain("Roger Histand", body);
		Assert.DoesNotContain("Grey heron.", body);
		Assert.DoesNotContain("08/01/23 7.12.00", body);

		Assert.DoesNotContain("Roger Histand", body);
		Assert.DoesNotContain("Large white heron in a wet meadow.", body);
		Assert.DoesNotContain("08/02/23 6.45.00", body);

		Assert.DoesNotContain("Roger Histand", body);
		Assert.DoesNotContain("Heard a booming call from the reed bed at dusk.", body);
		Assert.DoesNotContain("08/03/23 21.10.00", body);

		Assert.Contains("Luanna Muro", body);
		Assert.Contains("Gannets diving offshore.", body);
		Assert.Contains("08/02/23 11.05.00", body);

		Assert.Contains("Luanna Muro", body);
		Assert.Contains("Flock of about 40 cormorants.", body);
		Assert.Contains("08/01/23 8.30.00", body);
	}
}
