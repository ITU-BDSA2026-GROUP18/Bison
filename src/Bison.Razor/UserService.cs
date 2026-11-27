public record ProfileViewModel(string Name, bool IsOwnProfile, bool IsDeletedUser, bool IsFollowing)
{
	// Signed-out viewers still see the button; tapping it sends them to the login page.
	public bool ShowFollowButton => !IsOwnProfile && !IsDeletedUser;
}

public interface IUserService
{
	ProfileViewModel? GetProfile(string username, string? currentUserId);

	// Returns true when currentUserId follows the user afterwards.
	bool ToggleFollow(string currentUserId, string username);
}

public class UserService : IUserService
{
	private readonly IUserRepository _repository;

	public UserService(IUserRepository repository)
	{
		_repository = repository;
	}

	public ProfileViewModel? GetProfile(string username, string? currentUserId)
	{
		var user = _repository.GetUserByName(username);
		if (user is null)
			return null;

		bool isOwn = currentUserId is not null && currentUserId == user.Id;
		bool isFollowing =
			currentUserId is not null && !isOwn && _repository.IsFollowing(currentUserId, user.Id);

		return new ProfileViewModel(
			user.Name,
			isOwn,
			user.Id == DeletedUserid.deletedUserId,
			isFollowing
		);
	}

	public bool ToggleFollow(string currentUserId, string username)
	{
		var user =
			_repository.GetUserByName(username)
			?? throw new ArgumentException($"User {username} not found.", nameof(username));
		return _repository.ToggleFollow(currentUserId, user.Id);
	}
}
