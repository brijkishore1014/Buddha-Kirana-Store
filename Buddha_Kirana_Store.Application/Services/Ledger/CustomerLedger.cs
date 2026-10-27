using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class CustomerLedger : IPartyLedger
{
    public PartyType PartyType => PartyType.Customer;
    public string DefaultRemarks => "Customer Udhaar Recovery";
    public CashFlowDirection CashDirection => CashFlowDirection.In;
    public CashBookSource CashSource => CashBookSource.CustomerPayment;

    public async Task ApplyPaymentAsync(IUnitOfWork uow, int partyId, decimal amount)
    {
        var customer = await uow.GetCustomerAsync(partyId);
        customer.ReceivePayment(amount);
    }
}
