using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class StockAdjustmentService(
    IUnitOfWorkFactory uowFactory,
    IStockService stock,
    IAdjustmentLossPolicy lossPolicy) : IStockAdjustmentService
{
    public async Task<List<StockAdjustment>> GetStockAdjustmentsAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.StockAdjustments.GetAllWithItemAsync();
    }

    public async Task SaveStockAdjustmentAsync(int itemId, decimal quantityChanged, AdjustmentReason reason, DateTime adjustmentDate, string remarks)
    {
        if (quantityChanged == 0)
            throw new DomainException("Adjustment quantity cannot be zero.");

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var item = await uow.GetItemAsync(itemId);

        var isLoss = lossPolicy.IsCompanyLoss(reason, quantityChanged);
        var unitCost = item.AvgCostPrice;

        var adjustment = new StockAdjustment
        {
            ItemId = item.Id,
            AdjustmentDate = adjustmentDate,
            QuantityChanged = quantityChanged,
            ReasonType = reason,
            IsCompanyLoss = isLoss,
            UnitCostSnapshot = unitCost,
            TotalLossAmount = isLoss ? Math.Round(Math.Abs(quantityChanged) * unitCost, 2) : 0,
            Remarks = remarks?.Trim() ?? string.Empty
        };
        uow.StockAdjustments.Add(adjustment);
        await uow.SaveChangesAsync(); // gives the adjustment its id for the ledger reference

        var ledgerEntry = quantityChanged > 0
            ? stock.Increase(item, quantityChanged, StockTransactionType.Adjustment, adjustment.Id, adjustmentDate)
            : stock.Decrease(item, Math.Abs(quantityChanged), StockTransactionType.Adjustment, adjustment.Id, adjustmentDate);
        uow.StockLedgers.Add(ledgerEntry);

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
