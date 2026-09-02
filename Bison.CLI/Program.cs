using System.Globalization;
using CsvHelper;
using SimpleDB;


public record Cheep(string Author, string Observation, long Timestamp);

class Program
{
    static void Main(string[] args)
    {
        #if FLAG_TEST
            Console.WriteLine("omg my flag works");
        #endif

        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();

        if(args.Length == 0){
            Console.WriteLine("No argument was given");
            return;
        }

        if(args[0] == "read") read(database);
        else if (args[0] == "observe") observe(args[1], database);

        static void read(IDatabaseRepository<Cheep> database) 
        {
            var records = database.read();
            foreach (var record in records)
            {
                DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
                Console.WriteLine($"{record.Author} @ {utcTime.LocalDateTime}: {record.Observation}");
            }
        }
        
        static void observe(string observation, IDatabaseRepository<Cheep> database)
        {
            string author = Environment.UserName;
            long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var rec = new Cheep(author, observation, timeStamp);

            database.store(rec);
            
        }
    }
}