using Microsoft.EntityFrameworkCore;

public class DBFacade
{
	private readonly BisonDbContext _context;

	public DBFacade(BisonDbContext context)
	{
		_context = context;
	}

	public List<Observation> getObservations()
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			orderby Observation.PubDate descending
			select Observation
		).Include(c => c.Author);
		result = query.ToList();
		return result;
	}

	public List<Observation> getObservationsByAuthor(string username)
	{
		var result = new List<Observation>();
		var query = (
			from Observation in _context.Observations
			where Observation.Author.Username == username
			orderby Observation.PubDate descending
			select Observation
		).Include(c => c.Author);
		result = query.ToList();
		return result;
	}
}
