using Microsoft.EntityFrameworkCore;

public class BisonDbContext : DbContext
{
	public BisonDbContext(DbContextOptions<BisonDbContext> options)
		: base(options) { }

	public DbSet<User> Users => Set<User>();
	public DbSet<Taxon> Taxons => Set<Taxon>();
	public DbSet<Observation> Observations => Set<Observation>();
	public DbSet<Comment> Comments => Set<Comment>();
	public DbSet<Proposal> Proposals => Set<Proposal>();
	public DbSet<Post> Posts => Set<Post>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(e =>
		{
			e.ToTable("user");
			e.HasKey(u => u.UserId);
			e.Property(u => u.UserId).HasColumnName("user_id");
			e.Property(u => u.Username).HasColumnName("username").UseCollation("NOCASE");
			e.Property(u => u.Email).HasColumnName("email").UseCollation("NOCASE");
			e.Property(u => u.PwHash).HasColumnName("pw_hash");

			e.HasIndex(u => u.Username).IsUnique();
			e.HasIndex(u => u.Email).IsUnique();
		});

		modelBuilder.Entity<Post>(e =>
		{
			// This strategy forces EF Core to use a separate table for each derived type (Observation, Comment, Proposal) while still allowing queries against the base type (Post).
			// Using the default strategy, (Table-per-Hierarchy (TPH)), would result in a single table for all derived types, which would force our relations to become nullable.
			e.UseTptMappingStrategy();

			e.ToTable("post");
			e.HasKey(p => p.PostId);
			e.Property(p => p.PostId).HasColumnName("post_id");
			e.Property(p => p.AuthorId).HasColumnName("author_id");
			e.Property(p => p.Text).HasColumnName("text");
			e.Property(p => p.PubDate)
				.HasColumnName("pub_date")
				.HasConversion(
					v =>
						new DateTimeOffset(
							DateTime.SpecifyKind(v, DateTimeKind.Utc)
						).ToUnixTimeSeconds(),
					v => DateTimeOffset.FromUnixTimeSeconds(v).UtcDateTime
				);

			e.HasOne(p => p.Author).WithMany(u => u.Posts).HasForeignKey(p => p.AuthorId);
		});

		modelBuilder.Entity<Observation>(e =>
		{
			e.ToTable("observation");
			e.Property(o => o.TaxonId).HasColumnName("taxon_id");

			e.HasOne(o => o.Taxon).WithMany(t => t.Observations).HasForeignKey(o => o.TaxonId);
		});

		modelBuilder.Entity<Comment>(e =>
		{
			e.ToTable("comment");
			e.Property(c => c.ObservationId).HasColumnName("observation_id");

			e.HasOne(c => c.Observation)
				.WithMany(o => o.Comments)
				.HasForeignKey(c => c.ObservationId);
		});

		modelBuilder.Entity<Taxon>(e =>
		{
			e.ToTable("taxon");
			e.HasKey(t => t.TaxonId);
			e.Property(t => t.TaxonId).HasColumnName("taxon_id");
			e.Property(t => t.ParentTaxonId).HasColumnName("parent_taxon_id");
			e.Property(t => t.DwcTaxonId).HasColumnName("dwc_taxon_id");
			e.Property(t => t.VernacularName).HasColumnName("vernacular_name");

			e.HasOne(t => t.ParentTaxon)
				.WithMany(tp => tp.ChildTaxons)
				.HasForeignKey(t => t.ParentTaxonId);
		});

		modelBuilder.Entity<Proposal>(e =>
		{
			e.ToTable("proposal");
			e.Property(p => p.ObservationId).HasColumnName("observation_id");
			e.Property(p => p.TaxonId).HasColumnName("taxon_id");

			e.HasOne(p => p.Observation)
				.WithMany(o => o.Proposals)
				.HasForeignKey(p => p.ObservationId);
			e.HasOne(p => p.Taxon).WithMany(t => t.Proposals).HasForeignKey(p => p.TaxonId);
		});
	}
}
