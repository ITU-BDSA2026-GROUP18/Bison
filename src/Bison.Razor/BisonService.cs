public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
    public List<ObservationViewModel> GetObservations(int page = 0);
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 0);
}

public class ObservationService : IObservationService
{
    // These would normally be loaded from a database for example
    private static readonly DBFacade _db;

    public ObservationService(DBFacade db) => db = _db;
    public List<ObservationViewModel> GetObservations(int page = 0)
    {
        return _obs;
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 0)
    {
        // filter by the provided author name
        return _obs.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
