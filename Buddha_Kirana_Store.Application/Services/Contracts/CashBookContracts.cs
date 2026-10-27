using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Date range (inclusive, whole days) and optional payment mode.</summary>
public sealed record CashBookQuery(DateTime? From = null, DateTime? To = null, PaymentMode? Mode = null);

/// <summary>Money in / out of one payment mode over a period (used by the repository).</summary>
public sealed record CashBookModeTotal(PaymentMode Mode, decimal MoneyIn, decimal MoneyOut);

/// <summary>Opening, movement and closing balance of one payment mode.</summary>
public sealed record CashBookBalance(PaymentMode Mode, decimal Opening, decimal MoneyIn, decimal MoneyOut)
{
    public decimal Closing => Opening + MoneyIn - MoneyOut;
}

public sealed record CashBookReport(IReadOnlyList<CashBookBalance> Balances, PagedResult<CashBookEntry> Entries)
{
    public decimal TotalOpening => Balances.Sum(b => b.Opening);
    public decimal TotalIn => Balances.Sum(b => b.MoneyIn);
    public decimal TotalOut => Balances.Sum(b => b.MoneyOut);
    public decimal TotalClosing => Balances.Sum(b => b.Closing);

    public static CashBookReport Empty { get; } =
        new([], new PagedResult<CashBookEntry>([], 0, 1, 25));
}
