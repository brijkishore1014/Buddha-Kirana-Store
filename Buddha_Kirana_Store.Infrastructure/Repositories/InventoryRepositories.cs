using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;

namespace Buddha_Kirana_Store.Data.Repositories;

internal sealed class UnitOfMeasureRepository(KiranaDbContext db) : EfRepository<UnitOfMeasure>(db), IUnitOfMeasureRepository
{
    public Task<List<UnitOfMeasure>> GetAllAsync() =>
        Db.UnitsOfMeasure.AsNoTracking().ToListAsync();
}

internal sealed class ItemRepository(KiranaDbContext db) : EfRepository<Item>(db), IItemRepository
{
    public Task<Item?> GetWithUnitAsync(int id) =>
        Db.Items.Include(i => i.Unit).FirstOrDefaultAsync(i => i.Id == id);

    public Task<List<Item>> GetWithUnitByIdsAsync(IReadOnlyCollection<int> ids) =>
        Db.Items.Include(i => i.Unit).Where(i => ids.Contains(i.Id)).ToListAsync();

    public Task<List<Item>> GetActiveAsync() =>
        Db.Items.Include(i => i.Unit)
                .Where(i => i.IsActive)
                .OrderBy(i => i.Name)
                .AsNoTracking()
                .ToListAsync();
}

internal sealed class StockAdjustmentRepository(KiranaDbContext db) : EfRepository<StockAdjustment>(db), IStockAdjustmentRepository
{
    public Task<List<StockAdjustment>> GetAllWithItemAsync() =>
        Db.StockAdjustments
          .Include(a => a.Item).ThenInclude(i => i.Unit)
          .OrderByDescending(a => a.AdjustmentDate)
          .ThenByDescending(a => a.Id)
          .AsNoTracking()
          .ToListAsync();
}
