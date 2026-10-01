using System.ComponentModel.DataAnnotations;

public class Proposal : Post
{
	[Required]
	public required Observation observation { get; set; }

	[Required]
	public required Taxon Taxon { get; set; }
}
