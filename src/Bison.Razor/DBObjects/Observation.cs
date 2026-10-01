using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLitePCL;

public class Observation : Post
{
	[Required]
	public required Taxon Taxon { get; set; }
}
