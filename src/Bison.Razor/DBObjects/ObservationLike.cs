namespace Bison.Razor.DBObjects;

using System.ComponentModel.DataAnnotations.Schema;

// One row per (user, observation) like, so a user can only like an observation once
// and the UI can show whether the current user has liked it.
[Table("observation_like")]
public class ObservationLike
{
	[Column("user_id")]
	public string UserId { get; set; } = "";

	[Column("observation_id")]
	public int ObservationId { get; set; }

	public User User { get; set; } = null!;
	public Observation Observation { get; set; } = null!;
}
