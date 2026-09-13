using System.Globalization;
using SimpleDB;

public record Observation(long Id, string Author, string Description, long Timestamp);

public record Comment(long ParentId, string Author, string Description, long Timestamp);
//var names could be better for the above, might get around to changing it

/*
namespace EnumCheeps  // For telling database.store() the type of Cheep
{
	enum CheepType
	{
		Observation,
		Comment
	}
}
*/

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
        var database = CSVDatabase<Observation>.getInstance();
        database.setPath("../SimpleDB/bison_observe_cli_db.csv");
        var records = database.read();
        foreach (var record in records)
        {
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Id} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Description}");
        }
    }

	public static void discussion(long observationId) // very similar to the above, could be refactored to be cleaner
	{
        var database = CSVDatabase<Comment>.getInstance();
        database.setPath("../SimpleDB/bison_comment_cli_db.csv");
        var records = database.read();
        foreach (var record in records)
        {
			if (record.ParentId == observationId)
			{
            	DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            	Console.WriteLine($"{record.ParentId} - {record.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}: {record.Description}");
			}
        }
	}
 	

    public static void observe(string observation)
    {
        var database = CSVDatabase<Observation>.getInstance();
        database.setPath("../SimpleDB/bison_observe_cli_db.csv");
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Observation(new Random().NextInt64(0, Int64.MaxValue), author, observation, timeStamp);
		// Pulling a random value here is also not great, throwing Ids into a set to check against 
		// or hashing contents of Observation would be ideal

        database.store(rec);
        
    }

    public static void comment(string comment, long Id)
    {

		// below is scope limited given it's just a routine for checking if ID exists.
		// Ideally down the line we want to store this as a Set to avoid slowdown 
		// checking against larger databases
		{

			bool idExists = false;

        	var csvData = CSVDatabase<Observation>.getInstance();
        	var csvRec = csvData.read();
		
        	foreach ( var existingRecord in csvRec )
        	{
        		if (existingRecord.Id == Id)
					idExists = true;
        	}

			if (!idExists) // ID not found!
			{
           		Console.WriteLine("Observation ID not found!");
				return;
			}

		}


        var database = CSVDatabase<Comment>.getInstance();
        database.setPath("../SimpleDB/bison_comment_cli_db.csv");
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Comment(Id, author, comment, timeStamp);
		
		database.store(rec);
	}
}
