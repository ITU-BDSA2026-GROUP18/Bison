public record ObservationViewModel(string Author, string Message, string Timestamp, int Id = 0);

public record CommentViewModel(
	string Author,
	string Message,
	string Timestamp,
	int ObservationId = 0
);

public interface IObservationService
{
	public List<ObservationViewModel> GetObservations(int page, int pageSize = 32);
	public List<ObservationViewModel> GetObservationsFromAuthor(
		string author,
		int page,
		int pageSize = 32
	);
	public ObservationViewModel GetObservationById(int observationId);
	public List<CommentViewModel> GetCommentsForObservation(int observationId);
}

public class ObservationService : IObservationService
{
	// These would normally be loaded from a database for example
	private readonly IPostRepository _repository;

	public ObservationService(IPostRepository repository)
	{
		_repository = repository;
	}

	public List<ObservationViewModel> GetObservations(int page, int pageSize = 32)
	{
		var result = new List<ObservationViewModel>();

		foreach (var observation in _repository.GetObservations(page, pageSize))
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

		foreach (var observation in _repository.GetObservationsByAuthor(author, page, pageSize))
		{
			result.Add(ToViewModel(observation));
		}

		return result;
	}

	public ObservationViewModel? GetObservationById(int observationId)
	{
		var observation = _repository.GetObservationById(observationId);
		if (observation == null)
			return null;
		return ToViewModel(observation);
	}

	public List<CommentViewModel> GetCommentsForObservation(int observationId)
	{
		var result = new List<CommentViewModel>();

		foreach (var comment in _repository.GetCommentsForObservation(observationId))
		{
			result.Add(ToViewModel(comment));
		}

		return result;
	}

	private static ObservationViewModel ToViewModel(Observation o)
	{
		return new ObservationViewModel(
			o.Author.Username,
			o.Text,
			o.PubDate.ToString("MM/dd/yy H:mm:ss"),
			o.PostId
		);
	}

	private static CommentViewModel ToViewModel(Comment c)
	{
		return new CommentViewModel(
			c.Author.Username,
			c.Text,
			c.PubDate.ToString("MM/dd/yy H:mm:ss"),
			c.ObservationId
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
