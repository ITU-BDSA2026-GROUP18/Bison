namespace Bison.Razor.DBObjects;

using System.ComponentModel.DataAnnotations.Schema;

// Follower follows Followee.
[Table("follow")]
public class Follow
{
	[Column("follower_id")]
	public string FollowerId { get; set; } = "";

	[Column("followee_id")]
	public string FolloweeId { get; set; } = "";

	public User Follower { get; set; } = null!;
	public User Followee { get; set; } = null!;
}
