using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLitePCL;

public class Observation : Post
{
	public Taxon? Taxon { get; set; }
}
