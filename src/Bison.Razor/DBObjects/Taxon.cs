public class Taxon
{
	public required int TaxonId { get; set; }

	public required string VernacularName { get; set; }
	public Taxon? Parent { get; set; }
	public List<Taxon> Children { get; set; }
}
