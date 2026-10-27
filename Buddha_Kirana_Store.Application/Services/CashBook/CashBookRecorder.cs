using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class CashBookRecorder : ICashBookRecorder
{
    public void Record(
        IUnitOfWork uow, CashFlowDirection direction, DateTime date, PaymentMode mode, decimal amount,
        CashBookSource source, int sourceId, string description)
    {
        if (amount <= 0)
            return;

        uow.CashBook.Add(CashBookEntry.Create(direction, date, mode, amount, source, sourceId, description));
    }
}
