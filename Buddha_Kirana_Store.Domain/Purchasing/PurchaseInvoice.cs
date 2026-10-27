namespace Buddha_Kirana_Store.Domain;

public class PurchaseInvoice
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public string? SupplierInvoiceNo { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public List<PurchaseInvoiceItem> Items { get; set; } = [];

    /// <summary>Computes line totals, bill total, balance and payment status; validates the paid amount.</summary>
    public void CalculateTotals()
    {
        if (Items.Count == 0)
            throw new DomainException("A purchase bill needs at least one item.");

        foreach (var line in Items)
            line.LineTotal = Math.Round(line.Quantity * line.PurchaseUnitPrice, 2);

        TotalAmount = Math.Round(Items.Sum(x => x.Quantity * x.PurchaseUnitPrice), 2);

        if (PaidAmount < 0 || PaidAmount > TotalAmount)
            throw new DomainException("Paid amount must be between 0 and the bill total.");

        BalanceDue = TotalAmount - PaidAmount;
        PaymentStatus = BalanceDue <= 0 ? PaymentStatus.Paid
                      : PaidAmount > 0 ? PaymentStatus.Partial
                      : PaymentStatus.Unpaid;
    }
}

public class PurchaseInvoiceItem
{
    public int Id { get; set; }
    public int PurchaseInvoiceId { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal PurchaseUnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
