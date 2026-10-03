using Microsoft.EntityFrameworkCore;

public class DBFacade
{
	private readonly BisonDbContext _context;

	public DBFacade(BisonDbContext context)
	{
		_context = context;
	}

	public List<Observation> getObservations(int page, int pageSize)
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

	public List<Observation> getObservationsByAuthor(string username, int page, int pageSize)
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

	public Observation? getObservationById(int observationId)
	{
		Console.WriteLine($"Searching for observation with ID: {observationId}");
		var query = (
			from Observation in _context.Observations
			where Observation.PostId == observationId
			select Observation
		).Include(c => c.Author);
		return query.ToList().FirstOrDefault();
	}

	public List<Comment> getCommentsForObservation(int observationId)
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
}
