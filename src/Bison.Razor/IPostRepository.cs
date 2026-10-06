public interface IPostRepository
{
	List<Observation> GetObservations(int page, int pageSize);
	List<Observation> GetObservationsByAuthor(string username, int page, int pageSize);
	Observation? GetObservationById(int observationId);
	List<Comment> GetCommentsForObservation(int observationId);
}
