using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface IItemService
{
    Task<List<Item>> GetItemsAsync();

    /// <summary>Quick-add from the purchase screen (no opening stock).</summary>
    Task<Item> CreateItemInlineAsync(string name, string? shortCode, int unitId, decimal sellingPrice, decimal minThreshold);

    /// <summary>Add from the inventory screen, optionally with opening stock.</summary>
    Task<Item> CreateItemWithOpeningStockAsync(
        string name, string? shortCode, int unitId,
        decimal openingStock, decimal openingCostPrice,
        decimal sellingPrice, decimal minThreshold);

    Task UpdateItemMasterAsync(int itemId, string name, string? shortCode, decimal sellingPrice, decimal minThreshold);
}

public interface IUnitOfMeasureService
{
    Task<List<UnitOfMeasure>> GetUnitsAsync();
}
