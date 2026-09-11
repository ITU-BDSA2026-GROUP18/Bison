namespace SimpleDB.Tests;

public class SimpleDBWriteTests
{
public record Cheep(string Author, string Observation, long Timestamp);
    private CSVDatabase<Cheep> database;
    private string dbpath = "../../../WriteTestDB.csv";
    public SimpleDBWriteTests()
    {
        this.database = CSVDatabase<Cheep>.getInstance();
        this.database.setPath(dbpath);
    }

    [Fact]
    public void WriteInputsDataToDB()
    {
        // arrange
        var rec = new Cheep("test", "test", 123456789);
        var len = File.ReadAllLines(dbpath).Length;
        // act
        database.store(rec);
        var lines = File.ReadAllLines(dbpath);
        //assert
        Assert.Equal(len+1, lines.Length);
        Assert.Equal("Author,Observation,Timestamp",lines[0]);
        Assert.Equal("test,test,123456789",lines[len]);
    }

    [Theory]
    [InlineData("test", "test line", 123456789)]
    [InlineData("onemoretest", "yep cool", 987654321)]
    [InlineData("finaltest", "alr buddy", 694201337)]
    public void WriteReadIntegration(string author, string observation, long timeStamp)
    {
        // arrange
        var rec = new Cheep(author, observation, timeStamp);
        // act
        database.storeNoAppend(rec); // used so db length stays low
        var list = database.read().ToList();
        // assert
        Assert.Equal(author, list[0].Author);
        Assert.Equal(observation, list[0].Observation);
        Assert.Equal(timeStamp, list[0].Timestamp);
    }
}
