using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("taxon")]
public class Taxon
{
	[Key]
	[Column("taxon_id")]
	public int TaxonId { get; set; }

	[Column("parent_taxon_id")]
	public int? ParentTaxonId { get; set; }

	[Column("dwc_taxon_id")]
	public string DwcTaxonId { get; set; } = "";

	[Column("vernacular_name")]
	public string? VernacularName { get; set; } = "";

	public Taxon? ParentTaxon { get; set; }
	public List<Taxon> ChildTaxons { get; set; } = []; // just to navigate to children in "reverse" direction
	public List<Proposal> Proposals { get; set; } = [];
  public List<Observation> Observations { get; set; } = [];
}
