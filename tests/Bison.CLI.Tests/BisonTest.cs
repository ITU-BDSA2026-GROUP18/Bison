using System.ComponentModel.Design;
using System.Data.Common;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTest : IDisposable
{
	private readonly WebApplication app;

	public BisonTest()
	{
		CSVDatabase<Observation>.ObserveDatabasePath = Path.Combine(
			AppContext.BaseDirectory,
			"../../../../data/bison_observe_cli_db.csv"
		);

		CSVDatabase<Comment>.CommentDatabasePath = Path.Combine(
			AppContext.BaseDirectory,
			"../../../../data/bison_comment_cli_db.csv"
		);

		var builder = WebApplication.CreateBuilder();
		builder.WebHost.UseUrls("http://localhost:5000");
		app = builder.Build();

		CSVDatabase<Comment>.getInstance().Configure(app, true);
		app.StartAsync().GetAwaiter().GetResult();

		var db = CSVDatabase<Observation>.getInstance();
		db.setPath(dbPath);
		Observation rec = new Observation(1, "tuff", "cat at home", 1789151813, "Vestamager");
		db.storeNoAppend(rec); // reset the db
		Program.Main(["--start"]);
	}

	public void Dispose()
	{
		app.StopAsync().GetAwaiter().GetResult();
		app.DisposeAsync().AsTask().GetAwaiter().GetResult();
	}

	[Fact]
	public void ReadE2E()
	{
		// arrange
		Console.WriteLine(Environment.CurrentDirectory);
		var expectedTime = DateTimeOffset.FromUnixTimeSeconds(1789151813);
		string expected =
			$"{expectedTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

		Console.WriteLine(Environment.CurrentDirectory);
		var sw = new StringWriter();
		Console.WriteLine(Environment.CurrentDirectory);
		string[] args = ["-r"];
		Console.SetOut(sw); // steal console output
		// act
		Program.Main(args);
		// assert
		Assert.Contains("1", sw.ToString());
		Assert.Contains("tuff", sw.ToString()); // easier than full string cmp
		Assert.Contains("cat at home", sw.ToString());
		Assert.Contains(expected, sw.ToString());
		Assert.Contains("Vestamager", sw.ToString());
	}

	[Theory]
	[InlineData("i saw big bird", "Vestamager")]
	[InlineData("the moon out my window", "Home")]
	public void ObserveAndReadE2E(string observation, string location)
	{
		// arrange
		var sw = new StringWriter();
		string[] args = ["-o", observation, location];
		string[] args2 = ["-r"];
		// act
		Console.SetOut(sw); // steal console output
		Program.Main(args);
		Program.Main(args2);
		// assert
		Assert.Contains(Environment.UserName, sw.ToString()); // easier than full string cmp
		Assert.Contains(observation, sw.ToString());
	}

	[Theory]
	[InlineData("ITU")]
	[InlineData("Vestamager")]
	public void LocationOutputsCorrectRecords(string location)
	{
		// arrange
		var sw = new StringWriter();
		var db = CSVDatabase<Observation>.getInstance();
		db.setPath(CSVDatabase<Observation>.ObserveDatabasePath);
		var records = db.read();
		string[] args = ["-l", location];
		List<Observation> relevantRecords = new List<Observation>();
		// act
		foreach (var record in records)
		{
			if (record.Location == location)
			{
				relevantRecords.Add(record);
			}
		}
		Console.SetOut(sw);
		Program.Main(args);
		// assert
		string[] result = sw.ToString()
			.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < relevantRecords.Count; i++)
		{
			Assert.Contains(relevantRecords[i].Id.ToString(), result[i]);
			Assert.Contains(relevantRecords[i].Author, result[i]);
			Assert.Contains(relevantRecords[i].Description, result[i]);
			Assert.Contains(relevantRecords[i].Location, result[i]);
		}
		Assert.Equal(relevantRecords.Count, result.Length);
	}
}
