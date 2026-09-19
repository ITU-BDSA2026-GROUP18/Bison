using System;
using System.Globalization;
using SimpleDB;

public class Program
{
	private static HttpClient Client = new HttpClient
	{
		BaseAddress = new Uri("http://localhost:5000"),
	};

	public static void Main(string[] args)
	{
		CLIHandler clh = new CLIHandler(args);
	}

	public static void setEnvPath(string envPath)
	{
		if (envPath != null)
		{
			Environment.CurrentDirectory = envPath;
			Console.WriteLine(Environment.CurrentDirectory);
		}
	}

	public static async Task read(string s)
	{
		var observation = await Client.GetFromJsonAsync<List<Observation>>("/observations");
		foreach (Observation o in observation)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(o.Timestamp);
			Console.WriteLine(
				$"{o.Id} - {o.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {o.Location}: {o.Description}"
			);
		}
	}

	public static async Task discussion(long obsID)
	{
		var comments = await Client.GetFromJsonAsync<List<Comment>>($"/comments/{obsID}");
		foreach (Comment c in comments)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(c.Timestamp);
			Console.WriteLine(
				$"{c.ParentId} - {c.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {c.Description}"
			);
		}
	}

	public static async Task location(string location)
	{
		// should prolly move the filter logic into the db
		var observation = await Client.GetFromJsonAsync<List<Observation>>("/observations");
		foreach (Observation o in observation)
		{
			DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(o.Timestamp);
			if (o.Location == location)
			{
				Console.WriteLine(
					$"{o.Id} - {o.Author} @ {utcTime.LocalDateTime.ToString("d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}, {o.Location}: {o.Description}"
				);
			}
		}
	}

	public static async Task comment(string comment, long id)
	{
		string author = Environment.UserName;
		long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var rec = new Comment(id, author, comment, timeStamp);
		var response = await Client.PostAsJsonAsync("/comment", rec);
		response.EnsureSuccessStatusCode();
	}

	public static async Task observe(string obs, string loc)
	{
		string author = Environment.UserName;
		long timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		var rec = new Observation(
			new Random().NextInt64(0, Int64.MaxValue),
			author,
			obs,
			timeStamp,
			loc
		);
		var response = await Client.PostAsJsonAsync("/observation", rec);
		response.EnsureSuccessStatusCode();
	}
}
