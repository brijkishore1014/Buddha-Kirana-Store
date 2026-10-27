namespace Buddha_Kirana_Store.Domain;

/// <summary>
/// One money movement (cash / UPI / bank) in or out of the shop. Every sale receipt, payment,
/// refund and expense writes one of these, so the day's totals come from a single table.
/// </summary>
public class CashBookEntry
{
    public const int MaxDescriptionLength = 200;

    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public CashFlowDirection Direction { get; set; }
    public decimal Amount { get; set; }                 // always positive; Direction says in or out
    public CashBookSource Source { get; set; }
    public int SourceId { get; set; }                   // id of the sale / return / payment / expense behind it
    public string Description { get; set; } = string.Empty;

    public static CashBookEntry Create(
        CashFlowDirection direction, DateTime date, PaymentMode mode, decimal amount,
        CashBookSource source, int sourceId, string description)
    {
        if (amount <= 0)
            throw new DomainException("Cash book amount must be greater than zero.");

        var text = (description ?? string.Empty).Trim();
        if (text.Length > MaxDescriptionLength)
            text = text[..MaxDescriptionLength];

        return new CashBookEntry
        {
            Direction = direction,
            EntryDate = date,
            PaymentMode = mode,
            Amount = amount,
            Source = source,
            SourceId = sourceId,
            Description = text
        };
    }
}
