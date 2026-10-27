using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buddha_Kirana_Store.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.HasIndex(x => x.Name);

        // Default walk-in customer (cash / UPI only).
        builder.HasData(new Customer { Id = 1, Name = "Walk-in Customer", IsWalkIn = true });
    }
}

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.HasIndex(x => x.Name);
    }
}
