namespace Buddha_Kirana_Store.Domain;

public class StockLedger
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public int ReferenceId { get; set; }
    public decimal QtyIn { get; set; }
    public decimal QtyOut { get; set; }
    public decimal ClosingStock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
