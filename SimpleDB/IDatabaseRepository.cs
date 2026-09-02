namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> read(int? limit = null);
    public void store(T record);
}
