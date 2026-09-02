using SimpleDB;
using System.CommandLine;

public record Cheep(string Author, string Observation, long Timestamp);

class Program
{
    static void Main(string[] args)
    {
        RootCommand rootCommand = new("Bison.CLI");

        var readCommand = new Command("--read", 
                    "Prints out entire contents of CSV to the console");

        readCommand.SetAction(parseResult => read());
        readCommand.Aliases.Add("-r");

        var obsTarg = new Argument<string>("Description");
        var obsCommand = new Command("--observe", "Add observation to the CSV database")
        {
            obsTarg
        };
        obsCommand.Aliases.Add("-o");
        
        obsCommand.SetAction(parseResult => observe(parseResult.GetValue(obsTarg)!));

        rootCommand.Subcommands.Add(readCommand);
        rootCommand.Subcommands.Add(obsCommand);
        rootCommand.Parse(args).Invoke();

        #if FLAG_TEST
            Console.WriteLine("omg my flag works");
        #endif
    }
    static void read() 
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        var records = database.read();
        foreach (var record in records)
        {
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Author} @ {utcTime.LocalDateTime}: {record.Observation}");
        }
    }
    
    static void observe(string observation)
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Cheep(author, observation, timeStamp);

        database.store(rec);
        
    }
}