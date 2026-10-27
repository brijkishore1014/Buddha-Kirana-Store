namespace Buddha_Kirana_Store.Domain;

public class StockAdjustment
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    public decimal QuantityChanged { get; set; }
    public AdjustmentReason ReasonType { get; set; }
    public bool IsCompanyLoss { get; set; }
    public decimal UnitCostSnapshot { get; set; }
    public decimal TotalLossAmount { get; set; }
    public string Remarks { get; set; } = string.Empty;
}
