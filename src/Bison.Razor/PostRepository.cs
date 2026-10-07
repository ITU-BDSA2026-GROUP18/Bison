using Microsoft.EntityFrameworkCore;

public class PostRepository : IPostRepository
{
	private readonly BisonDbContext _context;

	public PostRepository(BisonDbContext context)
	{
		_context = context;
	}

	public List<Observation> GetObservations(int page, int pageSize)
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			orderby Observation.PubDate descending
			select Observation
		)
			.Include(c => c.Author)
			.Skip((page - 1) * pageSize)
			.Take(pageSize);
		result = query.ToList();
		return result;
	}

	public List<Observation> GetObservationsByAuthor(string username, int page, int pageSize)
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			where Observation.Author.Username == username
			orderby Observation.PubDate descending
			select Observation
		)
			.Include(c => c.Author)
			.Skip((page - 1) * pageSize)
			.Take(pageSize);
		result = query.ToList();
		return result;
	}

	public Observation? GetObservationById(int observationId)
	{
		Console.WriteLine($"Searching for observation with ID: {observationId}");
		var query = (
			from Observation in _context.Observations
			where Observation.PostId == observationId
			select Observation
		).Include(c => c.Author);
		return query.ToList().FirstOrDefault();
	}

	public List<Comment> GetCommentsForObservation(int observationId)
	{
		var result = new List<Comment>();
		var query = (
			from Comment in _context.Comments
			where Comment.ObservationId == observationId
			select Comment
		)
			.Include(c => c.Author)
			.Include(c => c.Observation);

		result = [.. query];
		return result;
	}

	public List<Proposal> GetProposalsForObservation(int observationId)
	{
		var result = new List<Proposal>();
		var query = (
			from Proposal in _context.Proposals
			where Proposal.ObservationId == observationId
			select Proposal
		)
			.Include(c => c.Author)
			.Include(c => c.Taxon);

		result = [.. query];
		return result;
	}
}
