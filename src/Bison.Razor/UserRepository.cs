public class UserRepository : IUserRepository
{
	private readonly BisonDbContext _context;

	public UserRepository(BisonDbContext context)
	{
		_context = context;
	}

	public User? GetByUsernameOrEmail(string identifier)
	{
		var normalized = identifier.Trim().ToLower();
		return _context.Users.FirstOrDefault(u =>
			u.Username.ToLower() == normalized || u.Email.ToLower() == normalized
		);
	}
}
