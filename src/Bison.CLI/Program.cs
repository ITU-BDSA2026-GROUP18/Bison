using Microsoft.VisualBasic;
using SimpleDB;

public record Cheep(string Author, string Observation, long Timestamp);

public class Program
{
    public static void Main(string[] args)
    {
        CLIHandler clh = new CLIHandler(args);

        #if FLAG_TEST
            Console.WriteLine("omg my flag works");
        #endif
    }
    public static void read() 
    {
        var database = CSVDatabase<Cheep>.getInstance();
        var records = database.read();
        foreach (var record in records)
        {
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {utcTime.LocalDateTime}: {record.Observation}");
        }
    }
    
    public static void observe(string observation)
    {
        var database = CSVDatabase<Cheep>.getInstance();
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Cheep(author, observation, timeStamp);

        database.store(rec);
        
    }
}