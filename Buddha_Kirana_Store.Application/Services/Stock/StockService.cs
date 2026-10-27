using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class StockService(IClock clock) : IStockService
{
    public StockLedger Receive(Item item, decimal quantity, decimal unitCost, StockTransactionType type, int referenceId, DateTime? at = null)
    {
        item.ReceiveStock(quantity, unitCost);
        return CreateEntry(item, type, referenceId, qtyIn: quantity, qtyOut: 0, at);
    }

    public StockLedger Increase(Item item, decimal quantity, StockTransactionType type, int referenceId, DateTime? at = null)
    {
        item.AddStock(quantity);
        return CreateEntry(item, type, referenceId, qtyIn: quantity, qtyOut: 0, at);
    }

    public StockLedger Decrease(Item item, decimal quantity, StockTransactionType type, int referenceId, DateTime? at = null)
    {
        item.RemoveStock(quantity);
        return CreateEntry(item, type, referenceId, qtyIn: 0, qtyOut: quantity, at);
    }

    private StockLedger CreateEntry(Item item, StockTransactionType type, int referenceId, decimal qtyIn, decimal qtyOut, DateTime? at) => new()
    {
        ItemId = item.Id,
        TransactionType = type,
        ReferenceId = referenceId,
        QtyIn = qtyIn,
        QtyOut = qtyOut,
        ClosingStock = item.CurrentStock,
        CreatedAt = at ?? clock.Now
    };
}
