using System;
using System.Globalization;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTest
{
	public BisonTest()
	{
		if (
			!(
				Environment.CurrentDirectory.EndsWith(
					"bison",
					StringComparison.CurrentCultureIgnoreCase
				)
			)
		)
		{
			string temppath = Environment.CurrentDirectory;
			int bisonIdx =
				temppath.LastIndexOf("bison/", StringComparison.CurrentCultureIgnoreCase) + 5;
			Environment.CurrentDirectory = temppath.Substring(0, bisonIdx);
		}

		var db = CSVDatabase<Observation>.getInstance();

		db.setPath(Environment.CurrentDirectory + "/tests/data/bison_observe_cli_db.csv");

		Observation rec = new Observation(1, "tuff", "cat at home", 1789151813, "Vestamager");

		db.storeNoAppend(rec); // reset the db
	}

	[Fact]
	public void ReadE2E()
	{
		// arrange
		Console.WriteLine(Environment.CurrentDirectory);
		var expectedTime = DateTimeOffset.FromUnixTimeSeconds(1789151813);
		string expected =
			$"{expectedTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";

		CSVDatabase<Observation>
			.getInstance()
			.setPath(Environment.CurrentDirectory + "tests/data/bison_observe_cli_db.csv");

		Console.WriteLine(Environment.CurrentDirectory);
		var sw = new StringWriter();
		Console.WriteLine(Environment.CurrentDirectory);
		string[] args = ["-r", "-p", (Environment.CurrentDirectory + "/tests/")];
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

		CSVDatabase<Observation>
			.getInstance()
			.setPath(Environment.CurrentDirectory + "tests/data/bison_observe_cli_db.csv");
		string[] args = ["-o", observation, location];
		string[] args2 = ["-r"];
		// act
		Program.Main(args);
		Console.SetOut(sw); // steal console output
		Program.Main(args2);
		// assert
		Assert.Contains(Environment.UserName, sw.ToString()); // easier than full string cmp
		Assert.Contains(observation, sw.ToString());
	}
}
