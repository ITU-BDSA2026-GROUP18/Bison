namespace SimpleDB;

using System.Globalization;
using CsvHelper;

public enum CheepType
{
	Observation,
	Comment,
}

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
	private string dbpath = "src/SimpleDB/";

	private CSVDatabase() { }

	private static CSVDatabase<T> instance = new();

	public static CSVDatabase<T> getInstance()
	{
		return instance;
	}

	private string getPath(CheepType t)
	{
		switch (t)
		{
			case CheepType.Observation:
				return "bison_observe_cli_db.csv";

			case CheepType.Comment:
				return "bison_comment_cli_db.csv";
			default:
				throw new ArgumentException("Invalid CheepType");
		}
	}

	public IEnumerable<T> read(CheepType t, int? limit = null)
	{
		using var reader = new StreamReader(dbpath + getPath(t));
		var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
		var records = csv.GetRecords<T>().ToList();
		return records;
	}

	public void store(T record, CheepType t)
	{
		using var writer = new StreamWriter(dbpath + getPath(t), append: true);
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

		csv.NextRecord();
		csv.WriteRecord(record);
	}

	public void setPath(string path) // set custom path, used for tests
	{
		dbpath = path;
	}

	public void storeNoAppend(T record, CheepType t) // will write to the database as if its empty, aka overwrite
	{
		using var writer = new StreamWriter(dbpath + getPath(t));
		using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
		csv.WriteHeader<T>();
		csv.NextRecord();
		csv.WriteRecord(record);
	}
}
