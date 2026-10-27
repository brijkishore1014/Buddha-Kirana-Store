using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;

namespace Buddha_Kirana_Store.Data.Repositories;

internal sealed class CustomerRepository(KiranaDbContext db) : EfRepository<Customer>(db), ICustomerRepository
{
    public Task<List<Customer>> GetAllOrderedAsync() =>
        Db.Customers.OrderByDescending(c => c.IsWalkIn)
                    .ThenBy(c => c.Name)
                    .AsNoTracking()
                    .ToListAsync();
}

internal sealed class SupplierRepository(KiranaDbContext db) : EfRepository<Supplier>(db), ISupplierRepository
{
    public Task<List<Supplier>> GetAllOrderedAsync() =>
        Db.Suppliers.OrderBy(s => s.Name).AsNoTracking().ToListAsync();
}
