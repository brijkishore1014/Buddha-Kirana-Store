namespace Buddha_Kirana_Store.Domain;

public class ShopExpense
{
    public int Id { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.Now;
    public string Category { get; set; } = string.Empty; // e.g., Transport/Hamali, Tea/Snacks, Electricity, Packaging
    public string Title { get; set; } = string.Empty;    // Short description
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
    public string? Remarks { get; set; }
}
