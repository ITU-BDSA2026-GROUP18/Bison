using Microsoft.EntityFrameworkCore;
public class BisonDbContext : DbContext
{
    public BisonDbContext(DbContextOptions<BisonDbContext> options) : base(options) {}

  public DbSet<User> Users => Set<User>();
    public DbSet<Observation> Observations => Set<Observation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("user");
            e.HasKey(u => u.UserId);
            e.Property(u => u.UserId).HasColumnName("user_id");
            e.Property(u => u.Username).HasColumnName("username");
            e.Property(u => u.Email).HasColumnName("email");
        });

        modelBuilder.Entity<Observation>(e =>
        {
            e.ToTable("observation");
            e.HasKey(o => o.ObservationId);
            e.Property(o => o.ObservationId).HasColumnName("observation_id");
            e.Property(o => o.AuthorId).HasColumnName("author_id");
            e.Property(o => o.Text).HasColumnName("text");
            e.Property(o => o.PubDate).HasColumnName("pub_date");

            e.HasOne(o => o.Author)
             .WithMany()
             .HasForeignKey(o => o.AuthorId);
        });
    }
        
}