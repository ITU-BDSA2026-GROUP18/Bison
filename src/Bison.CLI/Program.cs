using System;
using System.Globalization;
using SimpleDB;

public record Observation(
	long Id,
	string Author,
	string Description,
	long Timestamp,
	string Location
);

public record Comment(long ParentId, string Author, string Description, long Timestamp);

//var names could be better for the above, might get around to changing it

public record ObservationTest(string Author, string Message, long Timestamp); //TODO: REMOVE

public class Program
{
	public static string ObserveDatabasePath { get; set; } = "./data/bison_observe_cli_db.csv";
	public static string CommentDatabasePath { get; set; } = "./data/bison_comment_cli_db.csv";

	public static void Main(string[] args)
	{
#if WEBSERVER
		Console.WriteLine("------- WEB SERVER BUILD -------");

#endif

		CLIHandler clh = new CLIHandler(args);

#if FLAG_TEST
		Console.WriteLine("omg my flag works");
#endif
	}

	public static void setEnvPath(string envPath)
	{
		if (envPath != null)
		{
			Environment.CurrentDirectory = envPath;
			Console.WriteLine(Environment.CurrentDirectory);
		}
	}

	public static void read(string? pathOption = null)
	{
		setEnvPath(pathOption);

		var database = CSVDatabase<Observation>.getInstance();
		database.setPath(ObserveDatabasePath);
		var records = database.read();

#if WEBSERVER
		var builder = WebApplication.CreateBuilder();
		var app = builder.Build();
		app.MapGet("/observations", () => records);
		app.Run();
#else

		foreach (var record in records)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
			Console.WriteLine(
				$"{record.Id} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {record.Location}: {record.Description}"
			);
		}
#endif
	}

	public static void discussion(long observationId) // very similar to the above, could be refactored to be cleaner
	{
		var database = CSVDatabase<Comment>.getInstance();
		database.setPath(CommentDatabasePath);
		var records = database.read();

#if WEBSERVER
		var builder = WebApplication.CreateBuilder();
		var app = builder.Build();
		app.MapGet("/discussion", () => records);
		app.Run();
#else
		foreach (var record in records)
		{
			if (record.ParentId == observationId)
			{
				DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
				Console.WriteLine(
					$"{record.ParentId} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Description}"
				);
			}
		}

#endif
	}

	public static void location(string location) // very similar to the above, could be refactored to be cleaner
	{
		var database = CSVDatabase<Observation>.getInstance();
		database.setPath(ObserveDatabasePath);
		var records = database.read();
		foreach (var record in records)
		{
			if (record.Location == location)
			{
				DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
				Console.WriteLine(
					$"{record.Id} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {record.Location}: {record.Description}"
				);
			}
		}
	}

	public static void observe(string observation, string location)
	{
		var database = CSVDatabase<Observation>.getInstance();
		database.setPath(ObserveDatabasePath);
		string author = Environment.UserName;
		long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var rec = new Observation(
			new Random().NextInt64(0, Int64.MaxValue),
			author,
			observation,
			timeStamp,
			location
		);
		// Pulling a random value here is also not great, throwing Ids into a set to check against
		// or potentially hashing contents of Observation would be more ideal

		database.store(rec);
	}

	public static void comment(string comment, long Id)
	{
		// below is scope limited given it's just a routine for checking if ID exists.
		// Ideally down the line we want to store this as a Set to avoid slowdown
		// checking against larger databases
		{
			bool idExists = false;

			var csvData = CSVDatabase<Observation>.getInstance();
			csvData.setPath(ObserveDatabasePath);
			var csvRec = csvData.read();

			foreach (var existingRecord in csvRec)
			{
				if (existingRecord.Id == Id)
					idExists = true;
			}

			if (!idExists) // ID not found!
			{
				Console.WriteLine("Observation ID not found!");
				return;
			}
		}

		var database = CSVDatabase<Comment>.getInstance();
		database.setPath(CommentDatabasePath);
		string author = Environment.UserName;
		long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var rec = new Comment(Id, author, comment, timeStamp);

		database.store(rec);
	}
}
