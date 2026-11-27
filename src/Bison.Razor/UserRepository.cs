using Bison.Razor.DBObjects;

public class UserRepository : IUserRepository
{
	private readonly BisonDbContext _context;

	public UserRepository(BisonDbContext context)
	{
		_context = context;
	}

	public User? GetUserByName(string username)
	{
		return _context.Users.FirstOrDefault(u => u.Name == username);
	}

	public bool IsFollowing(string followerId, string followeeId)
	{
		return _context.Follows.Any(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
	}

	public bool ToggleFollow(string followerId, string followeeId)
	{
		if (string.IsNullOrWhiteSpace(followerId))
			throw new ArgumentException("Follower is required.", nameof(followerId));

		if (followerId == followeeId)
			throw new ArgumentException("You cannot follow yourself.", nameof(followeeId));

		if (followeeId == DeletedUserid.deletedUserId)
			throw new ArgumentException("You cannot follow a deleted user.", nameof(followeeId));

		if (!_context.Users.Any(u => u.Id == followerId))
			throw new ArgumentException(
				$"User with ID {followerId} not found.",
				nameof(followerId)
			);

		if (!_context.Users.Any(u => u.Id == followeeId))
			throw new ArgumentException(
				$"User with ID {followeeId} not found.",
				nameof(followeeId)
			);

		var existing = _context.Follows.Find(followerId, followeeId);
		if (existing is null)
		{
			_context.Follows.Add(new Follow { FollowerId = followerId, FolloweeId = followeeId });
		}
		else
		{
			_context.Follows.Remove(existing);
		}
		_context.SaveChanges();

		return existing is null;
	}
}
