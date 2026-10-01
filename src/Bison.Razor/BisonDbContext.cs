using Microsoft.EntityFrameworkCore;

public class BisonDbContext : DbContext
{
	public BisonDbContext(DbContextOptions<BisonDbContext> options) : base(options) { }

	public DbSet<User> Users => Set<User>();
	public DbSet<Observation> Observations => Set<Observation>();
	public DbSet<Comment> Comments => Set<Comment>();
	public DbSet<Taxon> taxons => Set<Taxon>();
	public DbSet<Proposal> proposals => Set<Proposal>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(e =>
		{
			e.ToTable("user");
			e.HasKey(u => u.Id);
			e.Property(u => u.Id).HasColumnName("user_id");
			e.Property(u => u.Username).HasColumnName("username");
			e.Property(u => u.Email).HasColumnName("email");
			e.Property(u => u.PwHash).HasColumnName("pw_hash");
		});

		modelBuilder.Entity<Observation>(e =>
		{
			e.ToTable("observation");
			e.HasKey(o => o.Id);
			e.Property(o => o.Id).HasColumnName("observation_id");
			e.Property(o => o.Author.Id).HasColumnName("author_id");
			e.Property(o => o.Text).HasColumnName("text");
			e.Property(o => o.TimeStamp).HasColumnName("timestamp");

			e.HasOne(o => o.Author).WithMany().HasForeignKey(o => o.Author.Id);
		});

		modelBuilder.Entity<Comment>(e =>
		{
			e.ToTable("comment");
			e.HasKey(c => c.Id);
			e.Property(c => c.Id).HasColumnName("comment_id");
			e.Property(c => c.Author.Id).HasColumnName("author_id");
			e.Property(c => c.Text).HasColumnName("text");
			e.Property(c => c.TimeStamp).HasColumnName("timestamp");

			e.HasOne(c => c.Author).WithMany().HasForeignKey(c => c.Author.Id);
		});

		modelBuilder.Entity<Taxon>(e =>
		{
			e.ToTable("taxon");
			e.HasKey(t => t.TaxonId);
			e.Property(t => t.VernacularName).HasColumnName("vernacular_name");
			e.Property(t => t.Parent.TaxonId).HasColumnName("parent_id");
		});

		modelBuilder.Entity<Proposal>(e =>
		{
			e.ToTable("proposal");
			e.HasKey(p => p.Id);
			e.Property(p => p.observation.Id).HasColumnName("observation_id");
			e.Property(p => p.Taxon.TaxonId).HasColumnName("taxon_id");
			e.Property(p => p.Author.Id).HasColumnName("author_id");
		});
	}
}
