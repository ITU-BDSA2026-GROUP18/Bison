namespace SimpleDB;

using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public record Observation(
	long Id,
	string Author,
	string Description,
	long Timestamp,
	string Location
);

public record Comment(long ParentId, string Author, string Description, long Timestamp);

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
	public static string ObserveDatabasePath { get; set; } = "./data/bison_observe_cli_db.csv";
	public static string CommentDatabasePath { get; set; } = "./data/bison_comment_cli_db.csv";

	private string dbpath = "/data/bison_observe_cli_db.csv";

	private CSVDatabase() { }

	private static CSVDatabase<T> instance = new();

	public static CSVDatabase<T> getInstance()
	{
		return instance;
	}

	public void setPath(string path) // set custom path, used for tests
	{
		dbpath = path;
	}

#if WEBSERVER

	Microsoft.AspNetCore.Builder.WebApplication app;
	public void start(Microsoft.AspNetCore.Builder.WebApplication app_)
	{
		app = app_;
		Console.WriteLine("TEST");
		// her sætter du alt server shit op

		read();
		store();

		app.Run();
	}

	private IEnumerable<T> internalRead(int? limit = null)
	{
		using var reader = new StreamReader(dbpath);
		var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
		var records = csv.GetRecords<T>().ToList();
		return records;
	}

	private void internalStore(T record)
	{
		using var writer = new StreamWriter(dbpath, append: true);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

		csv.NextRecord();
		csv.WriteRecord(record);
	}

	private static IEnumerable<Comment> getMatchingId(long Id)
	{
		List<Comment> filteredRecords = new List<Comment>();

		var database = CSVDatabase<Comment>.getInstance();
		database.setPath(CommentDatabasePath);
		var records = database.internalRead();

		foreach (var record in records)
		{
			if (record.ParentId == Id)
			{
				filteredRecords.Add(record);
			}
		}
		return filteredRecords;
	}

	private static bool doesIdExist(long Id)
	{
		var csvData = CSVDatabase<Observation>.getInstance();
		csvData.setPath(ObserveDatabasePath);
		var csvRec = csvData.internalRead();
		foreach (var existingRecord in csvRec)
		{
			if (existingRecord.Id == Id)
				return true;
		}

		return false;
	}

	public void read()
	{
		var observationsDB = CSVDatabase<Observation>.getInstance();
		var commentsDB = CSVDatabase<Comment>.getInstance();
		observationsDB.setPath(ObserveDatabasePath);
		commentsDB.setPath(CommentDatabasePath);
		var observationRec = observationsDB.internalRead();
		var commentRec = commentsDB.internalRead();

		app.MapGet("/observations", () => commentRec);

		app.MapPost(
			"/comments",
			(long netId) =>
			{
				var filtered = getMatchingId(netId);
				Results.Created($"Comments to request:\n {filtered}", filtered);
			}
		);

	}

	public void store()
	{
		// same same til at store
		var observationsDB = CSVDatabase<Observation>.getInstance();
		var commentsDB = CSVDatabase<Comment>.getInstance();
		observationsDB.setPath(ObserveDatabasePath);
		commentsDB.setPath(CommentDatabasePath);

		app.MapPost(
			"/observation",
			(Observation netObservation) =>
			{
				observationsDB.internalStore(netObservation);
			}
		);

		app.MapPost(
			"/comment",
			(Comment netComment) =>
			{
				if (doesIdExist(netComment.ParentId))
				{
					commentsDB.setPath(CommentDatabasePath);
					commentsDB.internalStore(netComment);
					return Results.Created($"created {netComment}", netComment);
				}
				else
				{
					return Results.BadRequest(netComment);
				}
			}
		);
	}

#else

	public IEnumerable<T> read(int? limit = null)
	{
		using var reader = new StreamReader(dbpath);
		var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
		var records = csv.GetRecords<T>().ToList();
		return records;
	}

	public void store(T record)
	{
		using var writer = new StreamWriter(dbpath, append: true);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

		csv.NextRecord();
		csv.WriteRecord(record);
	}

	public void storeNoAppend(T record) // will write to the database as if its empty, aka overwrite
	{
		using var writer = new StreamWriter(dbpath);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
		csv.WriteHeader<T>();
		csv.NextRecord();
		csv.WriteRecord(record);
	}
#endif
}
