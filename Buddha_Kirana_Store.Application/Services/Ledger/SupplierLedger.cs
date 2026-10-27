using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class SupplierLedger : IPartyLedger
{
    public PartyType PartyType => PartyType.Supplier;
    public string DefaultRemarks => "Supplier Balance Payout";
    public CashFlowDirection CashDirection => CashFlowDirection.Out;
    public CashBookSource CashSource => CashBookSource.SupplierPayment;

    public async Task ApplyPaymentAsync(IUnitOfWork uow, int partyId, decimal amount)
    {
        var supplier = await uow.GetSupplierAsync(partyId);
        supplier.MakePayment(amount);
    }
}
