namespace SimpleDB;
using System.Globalization;
using CsvHelper;

public enum CheepType
{
    Observation,
    Comment
}


public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{

	

    private CSVDatabase() {}
    private static CSVDatabase<T> instance = new();

    public static CSVDatabase<T> getInstance()
    {
        return instance;
    }
    public IEnumerable<T> read(int? limit = null)
    {
        using var reader = new StreamReader("src/SimpleDB/bison_observe_cli_db.csv");
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>().ToList();
        return records;
    }

    public void store(T record, CheepType t)
    {

		string path = "";

		switch (t)
		{
			case CheepType.Observation:
					path = "src/SimpleDB/bison_observe_cli_db.csv";
			break;

			case CheepType.Comment:
					path = "src/SimpleDB/bison_comment_cli_db.csv";

			break;
		}

        using var writer = new StreamWriter(path, append: true);

        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.NextRecord();
        csv.WriteRecord(record);
    }
}
