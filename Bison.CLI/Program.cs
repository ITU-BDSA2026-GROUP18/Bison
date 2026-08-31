using System.Globalization;
using CsvHelper;

public record Cheep(string Author, string Observation, long Timestamp);

class Program
{
    static void Main(string[] args)
    {
        #if FLAG_TEST
            Console.WriteLine("omg my flag works");
        #endif

        if(args.Length == 0){
            Console.WriteLine("No argument was given");
            return;
        }

        if(args[0] == "read") read();
        else if (args[0] == "observe") observe(args[1]);

        static void read() 
        {
            using var reader = new StreamReader("bison_observe_cli_db.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Cheep>();
            foreach (var record in records)
            {
                DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
                Console.WriteLine($"{record.Author} @ {utcTime.LocalDateTime}: {record.Observation}");
            }
        }
        
        static void observe(string observation)
        {
            using var writer = new StreamWriter("bison_observe_cli_db.csv", append: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            string author = Environment.UserName;
            long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var cheep = new Cheep(author, observation, timeStamp);
            csv.WriteRecord(cheep);
            csv.NextRecord();
        }
    }
}