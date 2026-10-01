using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
	[Required]
	public required int Id { get; set; }

	[Required]
	public required string Username { get; set; } = "";

	[Required]
	public required string Email { get; set; } = "";

	[Required]
	public int PwHash { get; set; }
}
