namespace SimpleDB;

public interface IDatabaseRepository<T>
{
	public void read();
	public void store();
}
