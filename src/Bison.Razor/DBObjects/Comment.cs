using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comment
{
	[Key]
	[Column("comment_id")]
	public int CommentId { get; set; }

	[Column("observation_id")]
	public int ObservationId { get; set; }

	[Column("author_id")]
	public int AuthorId { get; set; }

	[Column("comment")]
	public string Message { get; set; } = "";

	[Column("pub_date")]
	public long PubDate { get; set; }

	/* Foreign keys should be figured out auto-magically :) */

	public User Author { get; set; } = null;
	public Observation observation { get; set; } = null;
}
