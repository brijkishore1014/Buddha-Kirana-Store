using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>
/// How a sales invoice is priced. Add a new billing mode by adding a new strategy,
/// without touching <see cref="ISalesService"/>.
/// </summary>
public interface IBillingStrategy
{
    BillingMode Mode { get; }

    /// <summary>Fills line prices, <c>SubTotal</c> and <c>NetTotalAmount</c> of the invoice.</summary>
    void Apply(SalesInvoice sale, IReadOnlyDictionary<int, Item> items);
}
