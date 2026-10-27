using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Look-ups shared by several use cases (one place for the "not found" rules).</summary>
internal static class UnitOfWorkExtensions
{
    public static async Task<Customer> GetCustomerAsync(this IUnitOfWork uow, int customerId) =>
        await uow.Customers.GetByIdAsync(customerId)
        ?? throw new DomainException($"Customer #{customerId} was not found.");

    public static async Task<Supplier> GetSupplierAsync(this IUnitOfWork uow, int supplierId) =>
        await uow.Suppliers.GetByIdAsync(supplierId)
        ?? throw new DomainException($"Supplier #{supplierId} was not found.");

    public static async Task<Item> GetItemAsync(this IUnitOfWork uow, int itemId) =>
        await uow.Items.GetWithUnitAsync(itemId)
        ?? throw new DomainException($"Item #{itemId} was not found.");

    /// <summary>Loads all items of a document in one query, keyed by item id.</summary>
    public static async Task<Dictionary<int, Item>> LoadItemsAsync(this IUnitOfWork uow, IEnumerable<int> itemIds)
    {
        var ids = itemIds.Distinct().ToList();
        var items = await uow.Items.GetWithUnitByIdsAsync(ids);
        var map = items.ToDictionary(i => i.Id);

        var missing = ids.Where(id => !map.ContainsKey(id)).ToList();
        if (missing.Count > 0)
            throw new DomainException($"Item(s) not found: {string.Join(", ", missing)}.");

        return map;
    }
}
