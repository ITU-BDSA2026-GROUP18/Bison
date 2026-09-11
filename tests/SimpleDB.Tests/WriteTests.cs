namespace SimpleDB.Tests;

public class SimpleDBWriteTests
{
public record Cheep(string Author, string Observation, long Timestamp);
    private CSVDatabase<Cheep> database;
    public SimpleDBWriteTests()
    {
        this.database = CSVDatabase<Cheep>.getInstance();
        this.database.setPath("../../../WriteTestDB.csv");
    }

    [Theory]
    [InlineData("test", "test line", 123456789)]
    [InlineData("onemoretest", "yep cool", 987654321)]
    [InlineData("finaltest", "alr buddy", 694201337)]
    public void StoreWritesDataCorrectly(string author, string observation, long timeStamp)
    {
        var rec = new Cheep(author, observation, timeStamp);
        database.storeNoAppend(rec);
        var list = database.read().ToList();
        Assert.Equal(author, list[0].Author);
        Assert.Equal(observation, list[0].Observation);
        Assert.Equal(timeStamp, list[0].Timestamp);
    }
}
