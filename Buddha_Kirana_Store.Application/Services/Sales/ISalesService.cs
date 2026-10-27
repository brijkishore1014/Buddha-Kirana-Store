using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface ISalesService
{
    /// <summary>One page of the sales history plus totals over everything the filters match.</summary>
    Task<SalesInvoiceSearchResult> SearchSalesInvoicesAsync(SalesInvoiceQuery query, int page, int pageSize);

    /// <summary>Prices the bill, moves stock, books Udhaar and the payment in one transaction.</summary>
    Task<int> SaveSalesInvoiceAsync(SalesInvoice sale, PaymentMode paymentMode);
}

public interface ISalesReturnService
{
    Task<List<SalesReturn>> GetSalesReturnsAsync();
    Task<int> SaveSalesReturnAsync(SalesReturn salesReturn);
}
