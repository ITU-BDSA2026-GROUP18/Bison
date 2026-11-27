namespace Bison.Razor.DBObjects;

using System.ComponentModel.DataAnnotations.Schema;

[Table("observation")]
public class Observation : Post
{
	[Column("taxon_id")]
	public int TaxonId { get; set; }

	public byte[]? Image { get; set; }

	public string? ImageContentType { get; set; }

	// Cached number of rows in observation_like for this observation.
	[Column("likes")]
	public int Likes { get; set; }

	public List<ObservationLike> LikedBy { get; set; } = [];

	[NotMapped]
	public string? ImageUrl => Image is null ? null : $"/ob/{PostId}/image";

	public Taxon Taxon { get; set; } = null!;

	public Taxon getTaxon() => Taxon;

	public List<Comment> Comments { get; set; } = [];
	public List<Proposal> Proposals { get; set; } = [];
}
