namespace SimpleDB.Tests;

public class SimpleDBWriteTests
{
	public record Observation(long Id, string Author, string Description, long Timestamp);

	private CSVDatabase<Observation> database;

	public SimpleDBWriteTests()
	{
		this.database = CSVDatabase<Observation>.getInstance();
		this.database.setPath(CommentDatabasePath);
	}

	private static string CommentDatabasePath =>
		Path.Combine(AppContext.BaseDirectory, "bison_comment_cli_db.csv");

	[Fact]
	public void WriteInputsDataToDB()
	{
		// arrange
		var rec = new Observation(0, "test", "test", 123456789);
		var len = File.ReadAllLines(CommentDatabasePath).Length;
		// act
		database.store(rec);
		var lines = File.ReadAllLines(CommentDatabasePath);
		//assert
		Assert.Equal(len + 1, lines.Length);
		Assert.Equal("Id,Author,Description,Timestamp", lines[0]);
		Assert.Equal("0,test,test,123456789", lines[len]);
	}

	[Theory]
	[InlineData("test", "test line", 123456789)]
	[InlineData("onemoretest", "yep cool", 987654321)]
	[InlineData("finaltest", "alr buddy", 694201337)]
	public void WriteReadIntegration(string author, string observation, long timeStamp)
	{
		// arrange
		var rec = new Observation(0, author, observation, timeStamp);
		// act
		database.storeNoAppend(rec); // used so db length stays low
		var list = database.read().ToList();
		// assert
		Assert.Equal(author, list[0].Author);
		Assert.Equal(observation, list[0].Description);
		Assert.Equal(timeStamp, list[0].Timestamp);
	}
}
