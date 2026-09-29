using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

public class BisonFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

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
            services.AddDbContext<BisonDbContext>(o =>
                o.UseSqlite($"Data Source={_dbPath}"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SqliteConnection.ClearAllPools();
        try { File.Delete(_dbPath); } catch { }
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
    public async Task Test1()
    {
        var response = await _client.GetAsync("/obs");
        var body = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status: {response.StatusCode}");
        _output.WriteLine(body);

        response.EnsureSuccessStatusCode();
    }
}