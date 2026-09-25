public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
	public List<ObservationViewModel> GetObservations();
	public List<ObservationViewModel> GetObservationsFromAuthor(string author);
}

public class ObservationService : IObservationService
{
	// These would normally be loaded from a database for example
	private readonly DBFacade _db;

	public ObservationService(DBFacade db)
	{
		_db = db;
	}

	public List<ObservationViewModel> GetObservations()
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _db.getObservations())
		{
			result.Add(ToViewModel(observation));
		}
		return result;
	}

	public List<ObservationViewModel> GetObservationsFromAuthor(string author)
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _db.getObservationsByAuthor(author))
		{
			result.Add(ToViewModel(observation));
		}
		return result;
	}

	private static ObservationViewModel ToViewModel(Observation o)
	{
		return new ObservationViewModel(
			o.Author.Username,
			o.Text,
			UnixTime.UnixTimeStampToDateTimeString(o.PubDate)
		);
	}
}

public class UnixTime
{
	public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
	{
		// Unix timestamp is seconds past epoch
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddSeconds(unixTimeStamp);
		return dateTime.ToString("MM/dd/yy H:mm:ss");
	}
}
