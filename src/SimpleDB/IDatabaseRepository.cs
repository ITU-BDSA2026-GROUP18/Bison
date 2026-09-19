namespace SimpleDB;

public interface IDatabaseRepository<T>
{
#if WEBSERVER
	public Task<IEnumerable<T>> read();
	public void store();
#else
	public IEnumerable<T> read(int? limit = null);
	public void store(T record);
#endif
}
