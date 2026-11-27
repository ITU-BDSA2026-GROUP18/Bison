using Bison.Razor.DBObjects;

public interface IUserRepository
{
	User? GetUserByName(string username);

	bool IsFollowing(string followerId, string followeeId);

	// Follows the user if followerId does not follow them yet, otherwise unfollows.
	// Returns true when followerId follows the user afterwards.
	bool ToggleFollow(string followerId, string followeeId);
}
