using Microsoft.EntityFrameworkCore;

public class DBFacade
{
    private readonly BisonDbContext _context;
    string DBPath { get; set;}
    public DBFacade(string DBPath)
    {
        this.DBPath = DBPath;
    }

    public async Task<List<Observation>> getObservations(int page = 0)
    {
        var result = new List<Observation>();
        var query = (from Observation in _context.Observatons
                    orderby Observation.pubDate descending
                    select Observation)
                    .Include(c => c.author)
                    .Skip(page*32).Take(32);
        result = await query.ToListAsync();
        return result;
    }

    public async Task<List<Observation>> getObservationsByAuthor(string username, int page = 0)
    {
        var result = new List<Observation>();
        var query = (from Observation in _context.Observatons
                    where Observation.author.username == username
                    orderby Observation.pubDate descending
                    select Observation)
                    .Include(c => c.author)
                    .Skip(page*32).Take(32);
        result = await query.ToListAsync();
        return result;
    }
}