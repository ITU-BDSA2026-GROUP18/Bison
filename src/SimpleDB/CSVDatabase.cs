namespace SimpleDB;
using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string dbpath = "src/SimpleDB/bison_observe_cli_db.csv";
    private CSVDatabase() {}
    private static CSVDatabase<T> instance = new();

    public static CSVDatabase<T> getInstance()
    {
        return instance;
    }
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
    public void setPath(string path) // set custom path, used for tests
    {
        dbpath = path;
    }
    public void storeNoAppend(T record) // will write to the database as if its empty, aka overwrite
    {
        using var writer = new StreamWriter(dbpath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<T>();
        csv.NextRecord();
        csv.WriteRecord(record);
    }
}
