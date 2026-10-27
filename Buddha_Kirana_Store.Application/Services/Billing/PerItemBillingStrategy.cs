using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Every line has its own selling price; the invoice total is the sum of lines minus discount.</summary>
internal sealed class PerItemBillingStrategy : IBillingStrategy
{
    public BillingMode Mode => BillingMode.PerItem;

    public void Apply(SalesInvoice sale, IReadOnlyDictionary<int, Item> items)
    {
        decimal subTotal = 0;

        foreach (var line in sale.Items)
        {
            var item = items[line.ItemId];
            line.SellingUnitPrice ??= item.SellingPrice;
            line.LineTotal = Math.Round(line.Quantity * line.SellingUnitPrice.Value, 2);
            subTotal += line.LineTotal.Value;
        }

        sale.SubTotal = subTotal;
        sale.NetTotalAmount = Math.Max(0, subTotal - sale.DiscountAmount);
    }
}
