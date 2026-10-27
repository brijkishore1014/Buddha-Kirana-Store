namespace Buddha_Kirana_Store.Domain;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal CurrentPayableBalance { get; private set; }

    public static Supplier Create(string name, string? phone, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Supplier name is required.");

        return new Supplier
        {
            Name = name.Trim(),
            Phone = phone?.Trim(),
            Address = address?.Trim()
        };
    }

    /// <summary>Unpaid part of a purchase bill.</summary>
    public void AddPayable(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Payable amount cannot be negative.");

        CurrentPayableBalance += amount;
    }

    /// <summary>Payment made to the supplier against the outstanding balance.</summary>
    public void MakePayment(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        CurrentPayableBalance -= amount;
    }

    /// <summary>Part of a purchase return that is set off against the supplier payable.</summary>
    public void AdjustAgainstReturn(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Adjusted amount cannot be negative.");

        CurrentPayableBalance -= amount;
    }
}
