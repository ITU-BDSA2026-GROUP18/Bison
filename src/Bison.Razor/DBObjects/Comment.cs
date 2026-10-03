using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("comment")]
public class Comment : Post
{
	[Column("observation_id")]
	public int ObservationId { get; set; }

	public Observation Observation { get; set; } = null!;
}
