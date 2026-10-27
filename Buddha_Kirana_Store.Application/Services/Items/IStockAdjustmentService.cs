using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface IStockAdjustmentService
{
    Task<List<StockAdjustment>> GetStockAdjustmentsAsync();

    /// <summary>Positive quantity adds stock, negative quantity removes it.</summary>
    Task SaveStockAdjustmentAsync(int itemId, decimal quantityChanged, AdjustmentReason reason, DateTime adjustmentDate, string remarks);
}
