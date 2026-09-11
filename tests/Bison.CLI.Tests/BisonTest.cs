using System.Globalization;
using Bison.CLI;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTest
{

    public BisonTest()
    {
        
        var db = CSVDatabase<Cheep>.getInstance();
        db.setPath("../../../testDB.csv");
        Cheep rec = new Cheep("tuff","cat at home", 1789151813);
        db.storeNoAppend(rec); // reset the db
    }
    
    [Fact]
    public void ReadE2E()
    {
        // arrange
        var expectedTime = DateTimeOffset.FromUnixTimeSeconds(1789151813);
        string expected = $"{expectedTime.LocalDateTime.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}";
        var sw = new StringWriter();
        string[] args = ["--read"];
        Console.SetOut(sw); // steal console output
        // act
        Program.Main(args);
        // assert
        Assert.Contains("tuff", sw.ToString()); // easier than full string cmp
        Assert.Contains("cat at home", sw.ToString());
        Assert.Contains(expected, sw.ToString());
    }

    [Theory]
    [InlineData("i saw big bird")]
    [InlineData("the moon out my window")]
    public void ObserveAndReadE2E(string ovbservation)
    {
        // arrange
        var sw = new StringWriter();
        CSVDatabase<Cheep>.getInstance().setPath("../../../testDB.csv");
        string[] args = ["-o", ovbservation];
        string[] args2 = ["-r"];
        // act
        Program.Main(args);
        Console.SetOut(sw); // steal console output
        Program.Main(args2);
        // assert
        Assert.Contains(Environment.UserName, sw.ToString()); // easier than full string cmp
        Assert.Contains(ovbservation, sw.ToString()); 
    }
}
