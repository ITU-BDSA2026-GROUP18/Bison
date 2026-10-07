using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
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
	public string PwHash { get; set; } = "";

	public List<Post> Posts { get; set; } = [];
}
