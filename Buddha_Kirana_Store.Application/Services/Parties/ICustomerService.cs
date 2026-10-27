using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetCustomersAsync();
    Task<Customer> CreateCustomerAsync(string name, string? phone, string? address);
}

public interface ISupplierService
{
    Task<List<Supplier>> GetSuppliersAsync();
    Task<Supplier> CreateSupplierAsync(string name, string? phone, string? address);
}
