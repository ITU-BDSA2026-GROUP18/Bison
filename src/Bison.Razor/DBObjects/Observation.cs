using System.ComponentModel.DataAnnotations.Schema;

[Table("observation")]
public class Observation : Post
{
	[Column("taxon_id")]
	public int TaxonId { get; set; }

	public byte[]? Image { get; set; }

	public string? ImageContentType { get; set; }

	[NotMapped]
	public string? ImageUrl => Image is null ? null : $"/ob/{PostId}/image";

	public Taxon Taxon { get; set; } = null!;

	public Taxon getTaxon() => Taxon;

	public List<Comment> Comments { get; set; } = [];
	public List<Proposal> Proposals { get; set; } = [];
}
