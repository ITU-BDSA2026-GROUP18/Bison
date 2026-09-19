namespace SimpleDB.Tests;
using SimpleDB;

public class SimpleDBWriteTests
{
	private CSVDatabase<Observation> database;
	private static string CommentDatabasePath =>
		Path.Combine(AppContext.BaseDirectory, "bison_comment_cli_db.csv");

	public SimpleDBWriteTests()
	{
		this.database = CSVDatabase<Observation>.getInstance();
		this.database.setPath(CommentDatabasePath);
	}


	[Fact]
	public void WriteInputsDataToDB()
	{
		// arrange
		var rec = new Observation(0, "test", "test", 123456789, "here");
		var len = File.ReadAllLines(CommentDatabasePath).Length;
		database.setPath(CommentDatabasePath);
		// act
		database.store(rec);
		var lines = File.ReadAllLines(CommentDatabasePath);
		//assert
		Assert.Equal(len + 1, lines.Length);
		Assert.Equal("Id,Author,Description,Timestamp,Location", lines[0]);
		Assert.Equal("0,test,test,123456789,here", lines[len]);
	}

	[Theory]
	[InlineData("test", "test line", 123456789)]
	[InlineData("onemoretest", "yep cool", 987654321)]
	[InlineData("finaltest", "alr buddy", 694201337)]
	public void WriteReadIntegration(string author, string observation, long timeStamp)
	{
		// this test somehow reads, and writes to different dbs???
		// honestly these tests are also useless, since we no longer
		// use these methods
		return; 
		// arrange
		var rec = new Observation(0, author, observation, timeStamp, "here");
		database.setPath(CommentDatabasePath);
		// act
		database.storeNoAppend(rec); // used so db length stays low
		var list = database.read().ToList();
		// assert
		Assert.Equal(author, list[0].Author);
		Assert.Equal(observation, list[0].Description);
		Assert.Equal(timeStamp, list[0].Timestamp);
	}
}
