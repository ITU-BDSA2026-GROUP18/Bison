using Microsoft.EntityFrameworkCore;
public class BisonDbContext : DbContext
{
    public BisonDbContext(DbContextOptions<BisonDbContext> options) : base(options) {}

    public DbSet<User> users;
    public DbSet<Observation> Observatons;
        
}