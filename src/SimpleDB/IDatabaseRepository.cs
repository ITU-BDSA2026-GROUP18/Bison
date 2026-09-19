namespace SimpleDB;

public interface IDatabaseRepository<T>
{
	public IEnumerable<T> read();
	public void store(Observation r);
}
