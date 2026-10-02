using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("proposal")]
public class Proposal
{
	[Key]
	[Column("proposal_id")]
	public int ProposalId { get; set; }

	[Column("author_id")]
	public int AuthorId { get; set; }

	[Column("observation_id")]
	public int ObservationId { get; set; }

	[Column("taxon_id")]
	public int TaxonId { get; set; }

	[Column("text")]
	public string Text { get; set; } = "";

	[Column("pub_date")]
	public long PubDate { get; set; }

	public User Author { get; set; } = null!;
	public Taxon Taxon { get; set; } = null!;
	public Observation Observation { get; set; } = null!;
}
