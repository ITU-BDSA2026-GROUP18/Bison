using Bison.Razor.DBObjects;
using Bison.Razor.ViewModels;

public interface IPostRepository
{
	List<Observation> GetObservations(int page, int pageSize);
	List<Observation> GetObservations();
	List<Observation> GetObservationsByAuthor(string username, int page, int pageSize);
	Observation? GetObservationById(int observationId);
	List<Comment> GetCommentsForObservation(int observationId);

	List<Comment> GetCommentsByAuthor(string username, int page, int pageSize);
	List<Proposal> GetProposalsForObservation(int observationId);

	List<Proposal> GetProposalsByAuthor(string username, int page, int pageSize);
	Taxon GetTaxonById(int taxonId);
	Comment AddCommentToObservation(int observationId, string authorId, CommentInput input);

	Task<int> ReassignPostsByUserId(string userId);

	Observation AddObservation(
		string authorId,
		string text,
		int taxonId,
		byte[]? image = null,
		string? imageContentType = null
	);
}
