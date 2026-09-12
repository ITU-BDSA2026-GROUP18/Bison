namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> read(CheepType type, int? limit = null);
    public void store(T record, CheepType type);
}
