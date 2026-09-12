using Microsoft.VisualBasic;
using SimpleDB;

public record Observation(long Id, string Author, 
							string Description, long Timestamp);

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
        var records = database.read();
        foreach (var record in records)
        {
            DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp);
            Console.WriteLine($"{record.Id} - {record.Author} @ {utcTime.LocalDateTime}: {record.Description}");
        }
    }
 	

    public static void observe(string observation)
    {
        var database = CSVDatabase<Observation>.getInstance();
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Observation(new Random().NextInt64(0, Int64.MaxValue), author, observation, timeStamp);

        database.store(rec, CheepType.Observation);
        
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
        string author = Environment.UserName;
        long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var rec = new Comment(Id, author, comment, timeStamp);
		
		database.store(rec, CheepType.Comment);
	}
}
