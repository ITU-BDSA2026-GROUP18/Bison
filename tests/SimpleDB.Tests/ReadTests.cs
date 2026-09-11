namespace SimpleDB.Tests;

public class SimpleDBReadTests
{
public record Cheep(string Author, string Observation, long Timestamp);
    private CSVDatabase<Cheep> database;
    public SimpleDBReadTests()
    {
        this.database = CSVDatabase<Cheep>.getInstance();
        this.database.setPath("../../../ReadTestDB.csv");
    }

    [Fact]
    public void ReadGetsAllLines()
    {
        //arrange in constructor

        // act
        var records = database.read();
        var list = records.ToList();
        // assert
        Assert.Equal(3, list.Count);
    }

    [Theory]
    [InlineData(0, "test", "this is the first line", 69420)]
    [InlineData(1, "test2", "this is the middle line", 13376767)]
    [InlineData(2, "test3", "this is the final line", 123456)]
    public void ReadGetsCorrectData(int line, String author, string observation, long timeStamp)
    {
        //arrange in constructor

        // act
        var records = database.read();
        var list = records.ToList();
        // assert
        Assert.Equal(author, list[line].Author);
        Assert.Equal(observation, list[line].Observation);
        Assert.Equal(timeStamp, list[line].Timestamp);
    }
}
