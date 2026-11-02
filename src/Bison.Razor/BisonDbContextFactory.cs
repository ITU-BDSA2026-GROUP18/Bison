using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class BisonDbContextFactory : IDesignTimeDbContextFactory<BisonDbContext>
{
	public BisonDbContext CreateDbContext(string[] args)
	{
		string? envPath = Environment.GetEnvironmentVariable("BISONDBPATH");
		string dbPath = string.IsNullOrWhiteSpace(envPath)
			? Path.Combine(Path.GetTempPath(), "bison.db")
			: Path.Combine("../Database", envPath);
		var options = new DbContextOptionsBuilder<BisonDbContext>()
			.UseSqlite($"Data Source={dbPath}")
			.Options;
		return new BisonDbContext(options);
	}
}
