namespace Buddha_Kirana_Store.Domain;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsWalkIn { get; set; }
    public decimal CurrentReceivableBalance { get; private set; }

    public bool CanTakeCredit => !IsWalkIn;

    public static Customer Create(string name, string? phone, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required.");

        return new Customer
        {
            Name = name.Trim(),
            Phone = phone?.Trim(),
            Address = address?.Trim(),
            IsWalkIn = false
        };
    }

    /// <summary>Udhaar given on a sale.</summary>
    public void AddCredit(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Credit amount cannot be negative.");
        if (amount == 0)
            return;
        if (!CanTakeCredit)
            throw new DomainException("Walk-in Customer cannot take credit (Udhaar). Please select or add a Named Customer.");

        CurrentReceivableBalance += amount;
    }

    /// <summary>Udhaar recovery received from the customer.</summary>
    public void ReceivePayment(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        CurrentReceivableBalance -= amount;
    }

    /// <summary>Part of a sales return that is set off against the customer's Udhaar.</summary>
    public void AdjustAgainstReturn(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Adjusted amount cannot be negative.");
        if (amount == 0)
            return;
        if (!CanTakeCredit)
            throw new DomainException("Walk-in Customer does not have an Udhaar account. Please refund the full return amount.");

        CurrentReceivableBalance -= amount;
    }
}
