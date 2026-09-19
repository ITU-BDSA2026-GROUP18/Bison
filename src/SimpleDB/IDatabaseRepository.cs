namespace SimpleDB;

public interface IDatabaseRepository<T>
{
#if WEBSERVER
	public Task<IEnumerable<T>> read(Object obj);
	public void store(T record);
#else
	public TaskIEnumerable<T> read(int? limit = null);
	public void store(T record);
#endif
}
