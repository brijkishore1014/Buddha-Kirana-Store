using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class CustomerService(IUnitOfWorkFactory uowFactory) : ICustomerService
{
    public async Task<List<Customer>> GetCustomersAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Customers.GetAllOrderedAsync();
    }

    public async Task<Customer> CreateCustomerAsync(string name, string? phone, string? address)
    {
        await using var uow = uowFactory.Create();
        var customer = Customer.Create(name, phone, address);
        uow.Customers.Add(customer);
        await uow.SaveChangesAsync();
        return customer;
    }
}

internal sealed class SupplierService(IUnitOfWorkFactory uowFactory) : ISupplierService
{
    public async Task<List<Supplier>> GetSuppliersAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Suppliers.GetAllOrderedAsync();
    }

    public async Task<Supplier> CreateSupplierAsync(string name, string? phone, string? address)
    {
        await using var uow = uowFactory.Create();
        var supplier = Supplier.Create(name, phone, address);
        uow.Suppliers.Add(supplier);
        await uow.SaveChangesAsync();
        return supplier;
    }
}
