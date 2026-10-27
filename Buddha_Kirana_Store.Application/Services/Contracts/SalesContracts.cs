using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Filters for the sales history. Null / empty means "no filter".</summary>
public sealed record SalesInvoiceQuery(
    DateTime? From = null,
    DateTime? To = null,
    int? CustomerId = null,
    BillingMode? Mode = null,
    string? Search = null);

/// <summary>Totals over every invoice matching the filters (not just the visible page).</summary>
public sealed record SalesSummary(int InvoiceCount, decimal TotalNet, decimal TotalCredit, decimal TotalProfit);

public sealed record SalesInvoiceSearchResult(PagedResult<SalesInvoice> Invoices, SalesSummary Summary);
