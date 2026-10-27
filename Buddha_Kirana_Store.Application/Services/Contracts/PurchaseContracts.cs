using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Filters for the purchase history. Null / empty means "no filter".</summary>
public sealed record PurchaseInvoiceQuery(
    DateTime? From = null,
    DateTime? To = null,
    int? SupplierId = null,
    PaymentStatus? Status = null,
    string? Search = null);

/// <summary>Totals over every bill matching the filters (not just the visible page).</summary>
public sealed record PurchaseSummary(int InvoiceCount, decimal TotalBill, decimal TotalPaid, decimal TotalBalanceDue);

public sealed record PurchaseInvoiceSearchResult(PagedResult<PurchaseInvoice> Invoices, PurchaseSummary Summary);
