using Bison.CLI;
using SimpleDB;

namespace Bison.CLI.Tests;

public class BisonTest
{

    public BisonTest()
    {
        
        var db = CSVDatabase<Cheep>.getInstance();
        db.setPath("../../../testDB.csv");
        Cheep rec = new Cheep(Environment.UserName,"cat at home", 1789151813);
        db.storeNoAppend(rec); // reset the db
    }
    
    [Fact]
    public void ReadE2E()
    {
        // arrange
        var sw = new StringWriter();
        string[] args = ["--read"];
        Console.SetOut(sw); // steal console output
        // act
        Program.Main(args);
        // assert
        Assert.Equal(Environment.UserName + " @ 9/11/2026 8:36:53 PM: cat at home\n", sw.ToString());
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
        Assert.Contains(ovbservation, sw.ToString()); // easier than full string cmp
    }
}
