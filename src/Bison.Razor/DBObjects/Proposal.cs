using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("proposal")]
public class Proposal : Post
{

	[Column("observation_id")]
	public int ObservationId { get; set; }

	[Column("taxon_id")]
	public int TaxonId { get; set; }

	public Taxon Taxon { get; set; } = null!;
	public Observation Observation { get; set; } = null!;
}
