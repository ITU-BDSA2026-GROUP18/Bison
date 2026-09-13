using System.Globalization;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTest
{

    public BisonTest()
    {
        
        var db = CSVDatabase<Observation>.getInstance();
        db.setPath("../../../");
        Observation rec = new Observation(2, "tuff", "cat at home", 1789151813);
        db.storeNoAppend(rec, CheepType.Observation); // reset the db
    }
    
    [Fact]
    public void ReadE2E()
    {
        // arrange
        var expectedTime = DateTimeOffset.FromUnixTimeSeconds(1789151813);
        string expected = $"{expectedTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}";
        var sw = new StringWriter();
        string[] args = ["--read"];
        Console.SetOut(sw); // steal console output
        // act
        Program.Main(args);
        // assert
        Assert.Contains("tuff", sw.ToString()); // easier than full string cmp
        Assert.Contains("cat at home", sw.ToString());
        Assert.Contains(expected, sw.ToString());
        Assert.Contains("1", sw.ToString());
    }

    [Theory]
    [InlineData("i saw big bird")]
    [InlineData("the moon out my window")]
    public void ObserveAndReadE2E(string ovbservation)
    {
        // arrange
        var sw = new StringWriter();
        CSVDatabase<Observation>.getInstance().setPath("../../../");
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
