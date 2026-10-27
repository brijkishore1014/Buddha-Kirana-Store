namespace Buddha_Kirana_Store.Domain;

public class SalesReturn
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime ReturnDate { get; set; } = DateTime.Now;
    public string? OriginalInvoiceNo { get; set; }
    public string? Remarks { get; set; }

    public decimal TotalReturnAmount { get; set; }
    public decimal RefundPaidAmount { get; set; }     // Cash/UPI refunded to customer
    public decimal UdhaarAdjustedAmount { get; set; } // Deducted from Customer Udhaar
    public PaymentMode RefundMode { get; set; } = PaymentMode.Cash;

    public List<SalesReturnItem> Items { get; set; } = [];

    public void CalculateTotals()
    {
        if (Items.Count == 0)
            throw new DomainException("Please add at least one returned item.");

        foreach (var line in Items)
            line.LineTotal = Math.Round(line.Quantity * line.ReturnUnitPrice, 2);

        TotalReturnAmount = Math.Round(Items.Sum(i => i.Quantity * i.ReturnUnitPrice), 2);

        if (RefundPaidAmount < 0 || RefundPaidAmount > TotalReturnAmount)
            throw new DomainException("Refund amount must be between 0 and the return total.");

        UdhaarAdjustedAmount = TotalReturnAmount - RefundPaidAmount;
    }
}

public class SalesReturnItem
{
    public int Id { get; set; }
    public int SalesReturnId { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal ReturnUnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
