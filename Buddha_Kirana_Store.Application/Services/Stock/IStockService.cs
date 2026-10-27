using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>
/// The single place where stock moves. Changes the item and returns the matching ledger
/// row (with the running closing stock) for the caller to add to its unit of work.
/// </summary>
public interface IStockService
{
    /// <summary>Stock-in at a cost; updates the moving average cost.</summary>
    StockLedger Receive(Item item, decimal quantity, decimal unitCost, StockTransactionType type, int referenceId, DateTime? at = null);

    /// <summary>Stock-in without touching the average cost.</summary>
    StockLedger Increase(Item item, decimal quantity, StockTransactionType type, int referenceId, DateTime? at = null);

    /// <summary>Stock-out; throws if there is not enough stock.</summary>
    StockLedger Decrease(Item item, decimal quantity, StockTransactionType type, int referenceId, DateTime? at = null);
}
