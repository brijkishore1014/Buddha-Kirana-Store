namespace Buddha_Kirana_Store.Domain;

public class PurchaseReturn
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public DateTime ReturnDate { get; set; } = DateTime.Now;
    public string? SupplierInvoiceNo { get; set; }
    public string? Remarks { get; set; }

    public decimal TotalReturnAmount { get; set; }
    public decimal RefundReceivedAmount { get; set; }  // Cash/UPI received back from supplier
    public decimal PayableAdjustedAmount { get; set; } // Deducted from Supplier Payable Balance
    public PaymentMode RefundMode { get; set; } = PaymentMode.Cash;

    public List<PurchaseReturnItem> Items { get; set; } = [];

    public void CalculateTotals()
    {
        if (Items.Count == 0)
            throw new DomainException("Please add at least one returned item.");

        foreach (var line in Items)
            line.LineTotal = Math.Round(line.Quantity * line.ReturnUnitPrice, 2);

        TotalReturnAmount = Math.Round(Items.Sum(i => i.Quantity * i.ReturnUnitPrice), 2);

        if (RefundReceivedAmount < 0 || RefundReceivedAmount > TotalReturnAmount)
            throw new DomainException("Refund received must be between 0 and the return total.");

        PayableAdjustedAmount = TotalReturnAmount - RefundReceivedAmount;
    }
}

public class PurchaseReturnItem
{
    public int Id { get; set; }
    public int PurchaseReturnId { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal ReturnUnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
