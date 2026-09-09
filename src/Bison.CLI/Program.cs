using SimpleDB;

public record Cheep(string Author, string Observation, long Timestamp);

class Program
{
    static void Main(string[] args)
    {
        CLIHandler clh = new CLIHandler(args);

        #if FLAG_TEST
            Console.WriteLine("omg my flag works");
        #endif
    }
    public static void read() 
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        var records = database.read();
        foreach (var record in records)
        {
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {utcTime.LocalDateTime}: {record.Observation}");
        }
    }
    
    public static void observe(string observation)
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Cheep(author, observation, timeStamp);

        database.store(rec);
        
    }
}