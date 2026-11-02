using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
	[Column("username")]
	public required string Name { get; set; } = "";

	public List<Post> Posts { get; set; } = [];
}
