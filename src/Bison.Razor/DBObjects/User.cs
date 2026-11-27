namespace Bison.Razor.DBObjects;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
	[Column("username")]
	public required string Name { get; set; } = "";

	public List<Post> Posts { get; set; } = [];

	// Users this user follows.
	public List<Follow> Following { get; set; } = [];

	// Users following this user.
	public List<Follow> Followers { get; set; } = [];

	public List<ObservationLike> Likes { get; set; } = [];
}
