using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class BisonDbContext : IdentityDbContext<User>
{
	public BisonDbContext(DbContextOptions<BisonDbContext> options)
		: base(options) { }

	public DbSet<Taxon> Taxons => Set<Taxon>();
	public DbSet<Observation> Observations => Set<Observation>();
	public DbSet<Comment> Comments => Set<Comment>();
	public DbSet<Proposal> Proposals => Set<Proposal>();
	public DbSet<Post> Posts => Set<Post>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<User>(e =>
		{
			e.ToTable("user");
			e.Property(u => u.Id).HasColumnName("user_id");
			e.Property(u => u.Name).HasColumnName("username");
			e.Property(u => u.Email).HasColumnName("email");
			e.Property(u => u.UserName).HasColumnName("identity_user_name");
		});

		modelBuilder.Entity<Post>(e =>
		{
			// Use one table for each derived type (Observation, Comment, Proposal),
			// so their type-specific relationships are not nullable.
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
