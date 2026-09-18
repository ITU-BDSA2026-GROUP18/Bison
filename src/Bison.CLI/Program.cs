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

public class Program
{
	public static string ObserveDatabasePath { get; set; } = "./data/bison_observe_cli_db.csv";
	public static string CommentDatabasePath { get; set; } = "./data/bison_comment_cli_db.csv";
	public static Microsoft.AspNetCore.Builder.WebApplicationBuilder builder =
		WebApplication.CreateBuilder();
	public static Microsoft.AspNetCore.Builder.WebApplication app = builder.Build();

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

	private static IEnumerable<Comment> getMatchingId(long Id)
	{
		List<Comment> filteredRecords = new List<Comment>();

		var database = CSVDatabase<Comment>.getInstance();
		database.setPath(CommentDatabasePath);
		var records = database.read();

		foreach (var record in records)
		{
			if (record.ParentId == Id)
			{
				filteredRecords.Add(record);
			}
		}
		return filteredRecords;
	}

#if WEBSERVER
	public static void discussion() // very similar to the above, could be refactored to be cleaner
#else
	public static void discussion(long observationId)
#endif
	{
#if WEBSERVER
		app.MapPost(
			"/comments",
			(long netId) =>
			{
				var filtered = getMatchingId(netId);
				Results.Created($"Comments to request:\n {filtered}", filtered);
			}
		);
		app.Run();
#else
		var filtered = getMatchingId(observationId);
		foreach (var record in filtered)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
			Console.WriteLine(
				$"{record.ParentId} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Description}"
			);
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

#if WEBSERVER //TODO: make this more boring
	public static void observe()
#else
	public static void observe(string observation, string location)
#endif
	{
		var database = CSVDatabase<Observation>.getInstance();
		database.setPath(ObserveDatabasePath);

#if WEBSERVER
		app.MapPost(
			"/observation",
			(Observation netObservation) =>
			{
				database.store(netObservation);
			}
		);
		app.Run(); // unfortunately the endpoint client has to supply their own ID here.
#else
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
#endif
	}

	private static bool doesIdExist(long Id)
	{
		var csvData = CSVDatabase<Observation>.getInstance();
		csvData.setPath(ObserveDatabasePath);
		var csvRec = csvData.read();
		foreach (var existingRecord in csvRec)
		{
			if (existingRecord.Id == Id)
				return true;
		}

		return false;
	}

#if WEBSERVER
	public static void comment()
#else
	public static void comment(string comment, long Id)
#endif
	{
#if WEBSERVER

		app.MapPost(
			"/comment",
			(Comment netComment) =>
			{
				if (doesIdExist(netComment.ParentId))
				{
					var database = CSVDatabase<Comment>.getInstance();
					database.setPath(CommentDatabasePath);
					database.store(netComment);
					return Results.Created($"created {netComment}", netComment);
				}
				else
				{
					return Results.BadRequest(netComment);
				}
			}
		);
		app.Run();
#else

		if (!(doesIdExist(Id))) // ID not found!
		{
			Console.WriteLine("Observation ID not found!");
			return;
		}

		var database = CSVDatabase<Comment>.getInstance();
		database.setPath(CommentDatabasePath);
		string author = Environment.UserName;
		long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var rec = new Comment(Id, author, comment, timeStamp);

		database.store(rec);
#endif
	}
}
