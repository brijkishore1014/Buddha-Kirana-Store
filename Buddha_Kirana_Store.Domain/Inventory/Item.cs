namespace Buddha_Kirana_Store.Domain;

/// <summary>
/// Item master + current stock position. Stock and average cost can only change
/// through the methods below, so every flow follows the same rules.
/// </summary>
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortCode { get; set; }
    public int UnitId { get; set; }
    public UnitOfMeasure Unit { get; set; } = null!;

    public decimal CurrentStock { get; private set; }
    public decimal AvgCostPrice { get; private set; }
    public decimal SellingPrice { get; set; }
    public decimal MinStockThreshold { get; set; }
    public bool IsActive { get; set; } = true;

    public static Item Create(string name, string? shortCode, UnitOfMeasure unit, decimal sellingPrice, decimal minStockThreshold)
    {
        var item = new Item { Unit = unit, UnitId = unit.Id };
        item.UpdateMaster(name, shortCode, sellingPrice, minStockThreshold);
        return item;
    }

    public void UpdateMaster(string name, string? shortCode, decimal sellingPrice, decimal minStockThreshold)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name is required.");
        if (sellingPrice < 0)
            throw new DomainException("Selling price cannot be negative.");
        if (minStockThreshold < 0)
            throw new DomainException("Minimum stock threshold cannot be negative.");

        Name = name.Trim();
        ShortCode = shortCode?.Trim();
        SellingPrice = sellingPrice;
        MinStockThreshold = minStockThreshold;
    }

    /// <summary>Stock-in at a known cost (purchase / opening stock). Recalculates the moving weighted-average cost.</summary>
    public void ReceiveStock(decimal quantity, decimal unitCost)
    {
        EnsureValidQuantity(quantity);
        if (unitCost < 0)
            throw new DomainException($"Unit cost for '{Name}' cannot be negative.");

        var existingValue = CurrentStock > 0 ? CurrentStock * AvgCostPrice : 0;
        var newStock = CurrentStock + quantity;

        if (newStock > 0)
            AvgCostPrice = Math.Round((existingValue + quantity * unitCost) / newStock, 2);

        CurrentStock = newStock;
    }

    /// <summary>Stock-in that does not change average cost (sales return, positive audit correction).</summary>
    public void AddStock(decimal quantity)
    {
        EnsureValidQuantity(quantity);
        CurrentStock += quantity;
    }

    /// <summary>Stock-out (sale, purchase return, damage, theft...). Never lets stock go below zero.</summary>
    public void RemoveStock(decimal quantity)
    {
        EnsureValidQuantity(quantity);
        if (quantity > CurrentStock)
            throw new DomainException($"Insufficient stock for '{Name}'. Available: {CurrentStock:0.###}");

        CurrentStock -= quantity;
    }

    public void EnsureValidQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException($"Quantity for '{Name}' must be greater than zero.");

        if (Unit is { AllowDecimals: false } && quantity % 1 != 0)
            throw new DomainException($"'{Name}' is measured in {Unit.ShortCode} and does not allow decimal quantities.");
    }
}
