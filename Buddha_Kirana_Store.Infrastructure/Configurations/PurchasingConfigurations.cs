using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buddha_Kirana_Store.Data.Configurations;

public sealed class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.Property(x => x.SupplierInvoiceNo).HasMaxLength(50);

        builder.HasOne(x => x.Supplier)
               .WithMany()
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.PurchaseInvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.InvoiceDate);
        builder.HasIndex(x => new { x.SupplierId, x.InvoiceDate });
    }
}

public sealed class PurchaseInvoiceItemConfiguration : IEntityTypeConfiguration<PurchaseInvoiceItem>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoiceItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);

        builder.HasOne(x => x.Item)
               .WithMany()
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.Property(x => x.ReturnNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.SupplierInvoiceNo).HasMaxLength(50);
        builder.Property(x => x.Remarks).HasMaxLength(500);

        builder.HasOne(x => x.Supplier)
               .WithMany()
               .HasForeignKey(x => x.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.PurchaseReturnId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ReturnNumber).IsUnique();
        builder.HasIndex(x => x.ReturnDate);
    }
}

public sealed class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);

        builder.HasOne(x => x.Item)
               .WithMany()
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
