namespace Buddha_Kirana_Store.Domain;

public class PaymentTransaction
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public PartyType PartyType { get; set; }
    public int PartyId { get; set; }
    public int? ReferenceInvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string? Remarks { get; set; }
}
