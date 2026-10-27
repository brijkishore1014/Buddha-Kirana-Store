using Buddha_Kirana_Store.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Buddha_Kirana_Store.Data.Repositories;

internal class EfRepository<T>(KiranaDbContext db) : IRepository<T> where T : class
{
    protected KiranaDbContext Db { get; } = db;

    public virtual async Task<T?> GetByIdAsync(int id) => await Db.Set<T>().FindAsync(id);

    public void Add(T entity) => Db.Set<T>().Add(entity);

    public void Remove(T entity) => Db.Set<T>().Remove(entity);
}
