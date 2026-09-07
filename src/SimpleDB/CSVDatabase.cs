namespace SimpleDB;
using System.Globalization;
using CsvHelper;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> read(int? limit = null)
    {
        using var reader = new StreamReader("../SimpleDB/bison_observe_cli_db.csv");
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>().ToList();
        return records;
    }

    public void store(T record)
    {
        using var writer = new StreamWriter("../SimpleDB/bison_observe_cli_db.csv", append: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.NextRecord();
        csv.WriteRecord(record);
    }
}