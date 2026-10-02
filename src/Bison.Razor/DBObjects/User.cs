using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
	[Key]
	[Column("user_id")]
	[Required]
	public required int UserId { get; set; }

	[Column("username")]
	[Required]
	public required string Username { get; set; } = "";

	[Column("email")]
	[Required]
	public required string Email { get; set; } = "";

	[Column("pw_hash")]
	[Required]
	public int PwHash { get; set; }

	public List<Proposal> Proposals { get; set; } = [];
}
