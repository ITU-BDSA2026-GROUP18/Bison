using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLitePCL;

[Table("observation")]
public class Observation : Post
{
	[Column("taxon_id")]
	public int TaxonId { get; set; }

	public Taxon Taxon { get; set; } = null!;
	public List<Comment> Comments { get; set; } = [];
	public List<Proposal> Proposals { get; set; } = [];
}
