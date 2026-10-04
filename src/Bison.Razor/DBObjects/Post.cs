using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLitePCL;

[Table("post")]
public abstract class Post
{
	[Key]
	[Column("post_id")]
	[Required]
	public int PostId { get; set; }

	[Column("author_id")]
	public int AuthorId { get; set; }

	[Column("text")]
	public string Text { get; set; } = "";

	[Column("pub_date")]
	public DateTime PubDate { get; set; }

	public User Author { get; set; } = null!;
}
