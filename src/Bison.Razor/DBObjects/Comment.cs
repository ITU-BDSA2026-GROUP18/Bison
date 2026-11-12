using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("comment")]
public class Comment : Post
{
	[Column("observation_id")]
	public int ObservationId { get; set; }
	[Column("parent_comment_id")]
	public int? ParentCommentId { get; set; }

	public Observation Observation { get; set; } = null!;
	public Comment? ParentComment { get; set; }
	public List<Comment> Replies { get; set; } = [];
}
