using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>One agreed total for the whole bill; lines carry quantities only.</summary>
internal sealed class LumpSumBillingStrategy : IBillingStrategy
{
    public BillingMode Mode => BillingMode.LumpSum;

    public void Apply(SalesInvoice sale, IReadOnlyDictionary<int, Item> items)
    {
        decimal referenceSubTotal = 0;

        foreach (var line in sale.Items)
        {
            var item = items[line.ItemId];
            referenceSubTotal += Math.Round(line.Quantity * item.SellingPrice, 2);
            line.SellingUnitPrice = null;
            line.LineTotal = null;
        }

        // The agreed total (NetTotalAmount) comes from the shopkeeper; SubTotal is only the MRP reference.
        sale.SubTotal = referenceSubTotal;
    }
}
