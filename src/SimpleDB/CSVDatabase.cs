namespace SimpleDB;
using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string dbpath;
    public CSVDatabase(string path)
    {
        this.dbpath = path;
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
}
