using Bison.Razor.DBObjects;
using Bison.Razor.ViewModels;

public record LikeResult(bool Liked, int Likes);

public interface IPostRepository
{
	List<Observation> GetObservations(int page, int pageSize);
	List<Observation> GetObservations();
	List<Observation> GetObservationsByAuthor(string username, int page, int pageSize);

	// Observations written by users that followerId follows, newest first.
	List<Observation> GetObservationsByFollowedAuthors(string followerId, int page, int pageSize);
	List<Observation> GetObservationsByFollowedAuthors(string followerId);

	// Which of the given observations userId has liked.
	HashSet<int> GetLikedObservationIds(string userId, IEnumerable<int> observationIds);

	// Likes the observation if userId has not liked it yet, otherwise removes the like.
	LikeResult ToggleLike(int observationId, string userId);

	// Removes every like made by userId and keeps the cached like counts in sync.
	Task RemoveLikesByUserId(string userId);
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
