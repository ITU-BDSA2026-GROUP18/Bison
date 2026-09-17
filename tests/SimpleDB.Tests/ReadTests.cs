using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace SimpleDB.Tests;

public class SimpleDBReadTests
{
	public record Observation(long Id, string Author, string Description, long Timestamp);

	private CSVDatabase<Observation> database = CSVDatabase<Observation>.getInstance();

	public SimpleDBReadTests()
	{
		var dbPath = Path.Combine(AppContext.BaseDirectory, "bison_observe_cli_db.csv");
		database.setPath(dbPath);
		database = CSVDatabase<Observation>.getInstance();
		this.database.setPath("../../../bison_observe_cli_db.csv");
	}

	[Fact]
	public void ReadGetsAllLines()
	{
		//arrange in constructor
		this.database.setPath("../../../bison_observe_cli_db.csv");

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
	public void ReadGetsCorrectData(long id, String author, string observation, long timeStamp)
	{
		//arrange in constructor

		// act
		var records = database.read();
		var list = records.ToList();
		// assert
		Assert.Equal(author, list.First(o => o.Id == id).Author);
		Assert.Equal(observation, list.First(o => o.Id == id).Description);
		Assert.Equal(timeStamp, list.First(o => o.Id == id).Timestamp);
	}
}
