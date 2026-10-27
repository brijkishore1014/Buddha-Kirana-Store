using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class PartyPaymentService(
    IUnitOfWorkFactory uowFactory,
    ICashBookRecorder cashBook,
    IEnumerable<IPartyLedger> ledgers) : IPartyPaymentService
{
    public async Task RecordPartyPaymentAsync(PartyType partyType, int partyId, decimal amount, PaymentMode mode, DateTime paymentDate, string? remarks)
    {
        var ledger = ledgers.FirstOrDefault(l => l.PartyType == partyType)
                     ?? throw new DomainException($"Payments for '{partyType}' are not supported.");

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        await ledger.ApplyPaymentAsync(uow, partyId, amount);

        var payment = new PaymentTransaction
        {
            TransactionDate = paymentDate,
            PartyType = partyType,
            PartyId = partyId,
            Amount = amount,
            PaymentMode = mode,
            Remarks = string.IsNullOrWhiteSpace(remarks) ? ledger.DefaultRemarks : remarks.Trim()
        };
        uow.Payments.Add(payment);
        await uow.SaveChangesAsync(); // gives the payment its id for the cash book reference

        cashBook.Record(uow, ledger.CashDirection, paymentDate, mode, amount,
            ledger.CashSource, payment.Id, payment.Remarks ?? ledger.DefaultRemarks);

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<List<PaymentTransaction>> GetPaymentTransactionsAsync(PartyType partyType)
    {
        await using var uow = uowFactory.Create();
        return await uow.Payments.GetByPartyTypeAsync(partyType);
    }
}
