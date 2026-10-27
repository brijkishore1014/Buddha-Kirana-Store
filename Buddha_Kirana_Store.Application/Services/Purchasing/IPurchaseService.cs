using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface IPurchaseService
{
    /// <summary>One page of the purchase history plus totals over everything the filters match.</summary>
    Task<PurchaseInvoiceSearchResult> SearchPurchaseInvoicesAsync(PurchaseInvoiceQuery query, int page, int pageSize);

    /// <summary>Stock-in, average cost, supplier payable and payment entry in one transaction.</summary>
    Task<int> SavePurchaseInvoiceAsync(
        PurchaseInvoice invoice,
        IReadOnlyDictionary<int, decimal> updatedSellingPrices,
        PaymentMode paymentMode);
}

public interface IPurchaseReturnService
{
    Task<List<PurchaseReturn>> GetPurchaseReturnsAsync();
    Task<int> SavePurchaseReturnAsync(PurchaseReturn purchaseReturn);
}
