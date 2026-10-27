namespace Buddha_Kirana_Store.Abstractions;

/// <summary>Minimal write-side contract shared by every repository.</summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    void Add(T entity);
    void Remove(T entity);
}
