public interface IPostRepository
{
	List<Observation> GetObservations(int page, int pageSize);
	List<Observation> GetObservations();
	List<Observation> GetObservationsByAuthor(string username, int page, int pageSize);
	Observation? GetObservationById(int observationId);
	List<Comment> GetCommentsForObservation(int observationId);
	List<Proposal> GetProposalsForObservation(int observationId);
	Taxon GetTaxonById(int taxonId);
}
