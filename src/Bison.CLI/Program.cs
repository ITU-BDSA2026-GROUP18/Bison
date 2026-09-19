using System;
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.VisualBasic;
using SimpleDB;

/*
public record Observation(
	long Id,
	string Author,
	string Description,
	long Timestamp,
	string Location
);

public record Comment(long ParentId, string Author, string Description, long Timestamp);
*/

//var names could be better for the above, might get around to changing it

public class Program
{
	//public static string ObserveDatabasePath { get; set; } = "./data/bison_observe_cli_db.csv";
	//public static string CommentDatabasePath { get; set; } = "./data/bison_comment_cli_db.csv";

	private static HttpClient Client = new HttpClient
	{
		BaseAddress = new Uri("http://localhost:5000"),
	};

	public static void Main(string[] args)
	{
		CLIHandler clh = new CLIHandler(args);
	}

	public static void setEnvPath(string envPath)
	{
		if (envPath != null)
		{
			Environment.CurrentDirectory = envPath;
			Console.WriteLine(Environment.CurrentDirectory);
		}
	}

	public static async Task read(string s)
	{
		var observation = await Client.GetFromJsonAsync<List<Observation>>("/observations");
		foreach (Observation o in observation)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(o.Timestamp);
			Console.WriteLine(
				$"{o.Id} - {o.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {o.Location}: {o.Description}"
			);
		}
	}

	public static async Task discussion(long l) { }

	public static async Task location(string location)
	{
		var observation = await Client.GetFromJsonAsync<List<Observation>>("/observations");
		foreach (Observation o in observation)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(o.Timestamp);
			if (o.Location == location)
			{
				Console.WriteLine(
					$"{o.Id} - {o.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {o.Location}: {o.Description}"
				);
			}
		}
	}

	public static async Task comment(string comment, long id) { }

	public static async Task observe(string obs, string loc)
	{
		string author = Environment.UserName;
		long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var rec = new Observation(
			new Random().NextInt64(0, Int64.MaxValue),
			author,
			obs,
			timeStamp,
			loc
		);
		var response = await Client.PostAsJsonAsync("/observation", rec);
		response.EnsureSuccessStatusCode();
	}
}
/*
public static void read(string? pathOption = null)
{
	setEnvPath(pathOption);

	var database = CSVDatabase<Observation>.getInstance();
	database.setPath(CSVDatabase<Observation>.ObserveDatabasePath);
	var records = database.read();

	foreach (var record in records)
	{
		DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
		Console.WriteLine(
			$"{record.Id} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {record.Location}: {record.Description}"
		);
	}
}

private static IEnumerable<Comment> getMatchingId(long Id)
{
	List<Comment> filteredRecords = new List<Comment>();

	var database = CSVDatabase<Comment>.getInstance();
	database.setPath(CSVDatabase<Comment>.CommentDatabasePath);
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

// very similar to the above, could be refactored to be cleaner
public static void discussion(long observationId)
{
	var filtered = getMatchingId(observationId);
	foreach (var record in filtered)
	{
		DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
		Console.WriteLine(
			$"{record.ParentId} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Description}"
		);
	}
}

public static void location(string location) // very similar to the above, could be refactored to be cleaner
{
	var database = CSVDatabase<Observation>.getInstance();
	database.setPath(CSVDatabase<Observation>.ObserveDatabasePath);
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
	database.setPath(CSVDatabase<Observation>.ObserveDatabasePath);

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

private static bool doesIdExist(long Id)
{
	var csvData = CSVDatabase<Observation>.getInstance();
	csvData.setPath(CSVDatabase<Observation>.ObserveDatabasePath);
	var csvRec = csvData.read();
	foreach (var existingRecord in csvRec)
	{
		if (existingRecord.Id == Id)
			return true;
	}

	return false;
}

public static void comment(string comment, long Id)
{
	if (!(doesIdExist(Id))) // ID not found!
	{
		Console.WriteLine("Observation ID not found!");
		return;
	}

	var database = CSVDatabase<Comment>.getInstance();
	database.setPath(CSVDatabase<Comment>.CommentDatabasePath);
	string author = Environment.UserName;
	long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
	var rec = new Comment(Id, author, comment, timeStamp);

	database.store(rec);
}
}
*/
