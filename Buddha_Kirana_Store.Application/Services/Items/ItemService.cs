using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class ItemService(IUnitOfWorkFactory uowFactory, IStockService stock) : IItemService
{
    public async Task<List<Item>> GetItemsAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Items.GetActiveAsync();
    }

    public Task<Item> CreateItemInlineAsync(string name, string? shortCode, int unitId, decimal sellingPrice, decimal minThreshold) =>
        CreateItemWithOpeningStockAsync(name, shortCode, unitId, 0, 0, sellingPrice, minThreshold);

    public async Task<Item> CreateItemWithOpeningStockAsync(
        string name, string? shortCode, int unitId,
        decimal openingStock, decimal openingCostPrice,
        decimal sellingPrice, decimal minThreshold)
    {
        if (openingStock < 0)
            throw new DomainException("Opening stock cannot be negative.");
        if (openingStock > 0 && openingCostPrice <= 0)
            throw new DomainException("Please enter the Opening Cost / Unit for the existing stock so profit can be calculated.");

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var unit = await uow.Units.GetByIdAsync(unitId)
                   ?? throw new DomainException("Unit of measure was not found.");

        var item = Item.Create(name, shortCode, unit, sellingPrice, minThreshold);
        uow.Items.Add(item);

        if (openingStock > 0)
        {
            var adjustment = new StockAdjustment
            {
                Item = item,
                QuantityChanged = openingStock,
                ReasonType = AdjustmentReason.OpeningStock,
                IsCompanyLoss = false,
                UnitCostSnapshot = openingCostPrice,
                TotalLossAmount = 0,
                Remarks = "Initial Opening Stock during setup"
            };
            uow.StockAdjustments.Add(adjustment);
            await uow.SaveChangesAsync(); // gives the adjustment (and item) their ids

            uow.StockLedgers.Add(stock.Receive(item, openingStock, openingCostPrice, StockTransactionType.Adjustment, adjustment.Id));
        }

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
        return item;
    }

    public async Task UpdateItemMasterAsync(int itemId, string name, string? shortCode, decimal sellingPrice, decimal minThreshold)
    {
        await using var uow = uowFactory.Create();
        var item = await uow.GetItemAsync(itemId);
        item.UpdateMaster(name, shortCode, sellingPrice, minThreshold);
        await uow.SaveChangesAsync();
    }
}

internal sealed class UnitOfMeasureService(IUnitOfWorkFactory uowFactory) : IUnitOfMeasureService
{
    public async Task<List<UnitOfMeasure>> GetUnitsAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Units.GetAllAsync();
    }
}
