namespace Buddha_Kirana_Store.Domain;

public class SalesInvoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public BillingMode BillingMode { get; set; } = BillingMode.PerItem;
    public decimal TotalCostAmount { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetTotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public bool SoldBelowCost { get; set; }
    public List<SalesInvoiceItem> Items { get; set; } = [];

    /// <summary>Derives credit (Udhaar) and the below-cost flag once the net total and cost are known.</summary>
    public void ApplyPayment()
    {
        if (ReceivedAmount < 0 || ReceivedAmount > NetTotalAmount)
            throw new DomainException("Received amount must be between 0 and the invoice total.");

        CreditAmount = NetTotalAmount - ReceivedAmount;
        SoldBelowCost = NetTotalAmount < TotalCostAmount;
    }
}

public class SalesInvoiceItem
{
    public int Id { get; set; }
    public int SalesInvoiceId { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal CostPriceAtSale { get; set; }
    public decimal? SellingUnitPrice { get; set; }
    public decimal? LineTotal { get; set; }
}
