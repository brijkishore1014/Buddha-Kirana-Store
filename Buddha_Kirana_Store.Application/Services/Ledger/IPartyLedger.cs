using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>
/// Knows how a payment changes the balance of one kind of party.
/// Every implementation must throw <see cref="DomainException"/> if the party does not exist
/// or the amount is invalid, so callers can treat them interchangeably.
/// </summary>
public interface IPartyLedger
{
    PartyType PartyType { get; }
    string DefaultRemarks { get; }

    /// <summary>Whether a payment to / from this kind of party brings money in or sends it out.</summary>
    CashFlowDirection CashDirection { get; }

    CashBookSource CashSource { get; }

    Task ApplyPaymentAsync(IUnitOfWork uow, int partyId, decimal amount);
}
