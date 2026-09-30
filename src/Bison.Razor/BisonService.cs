public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
	public List<ObservationViewModel> GetObservations(int page, int pageSize = 32);
	public List<ObservationViewModel> GetObservationsFromAuthor(
		string author,
		int page,
		int pageSize = 32
	);
	public ObservationViewModel GetObservationById(int observationId);
}

public class ObservationService : IObservationService
{
	// These would normally be loaded from a database for example
	private readonly DBFacade _db;

	public ObservationService(DBFacade db)
	{
		_db = db;
	}

	public List<ObservationViewModel> GetObservations(int page, int pageSize = 32)
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _db.getObservations(page, pageSize))
		{
			result.Add(ToViewModel(observation));
		}
		return result;
	}

	public List<ObservationViewModel> GetObservationsFromAuthor(
		string author,
		int page,
		int pageSize = 32
	)
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _db.getObservationsByAuthor(author, page, pageSize))
		{
			result.Add(ToViewModel(observation));
		}

		return result;
	}

	public ObservationViewModel GetObservationById(int observationId)
	{
    var observation = _db.getObservationById(observationId) ?? throw new Exception($"Observation with ID {observationId} not found.");
    return ToViewModel(observation);
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
