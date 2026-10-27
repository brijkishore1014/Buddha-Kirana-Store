using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface IPartyPaymentService
{
    /// <summary>Records an Udhaar recovery (customer) or a payout (supplier) and updates the balance.</summary>
    Task RecordPartyPaymentAsync(PartyType partyType, int partyId, decimal amount, PaymentMode mode, DateTime paymentDate, string? remarks);

    Task<List<PaymentTransaction>> GetPaymentTransactionsAsync(PartyType partyType);
}
