using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>
/// Used by every flow that moves money. Writes into the caller's unit of work, so the cash book
/// entry is saved (or rolled back) together with the sale, return, payment or expense.
/// Amounts of zero or less are ignored.
/// </summary>
public interface ICashBookRecorder
{
    void Record(
        IUnitOfWork uow, CashFlowDirection direction, DateTime date, PaymentMode mode, decimal amount,
        CashBookSource source, int sourceId, string description);
}
