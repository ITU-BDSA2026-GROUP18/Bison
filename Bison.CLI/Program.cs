using System.Globalization;

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
    string[] lines = File.ReadAllLines("bison_observe_cli_db.csv");
    List<string> db = new List<string>();
    foreach (string line in lines)
    {
        string[] parts = line.Split(',');
        foreach (string part in parts)
        {
            db.Add(part);
        }
    }
    for (int i = 3; i < db.Count; i = i + 3)
    {
        long unixTime = long.Parse(db[i+2]);
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime();

        Console.WriteLine(db[i] + " @ " + dto.ToString("MM/dd/yy HH:mm:ss", CultureInfo.InvariantCulture) + " " + db[i+1]);
    }
}
static void observe(string observation)
{
    string author = Environment.UserName;
    long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    string line = "\n" + author + ",\"" + observation + "\"," + timeStamp;
    File.AppendAllText("bison_observe_cli_db.csv", line);
}
