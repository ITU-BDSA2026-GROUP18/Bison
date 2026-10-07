public interface IUserRepository
{
	User? GetByUsernameOrEmail(string identifier);
}
