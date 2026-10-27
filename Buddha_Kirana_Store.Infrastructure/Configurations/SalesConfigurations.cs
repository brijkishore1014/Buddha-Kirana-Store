using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buddha_Kirana_Store.Data.Configurations;

public sealed class SalesInvoiceConfiguration : IEntityTypeConfiguration<SalesInvoice>
{
    public void Configure(EntityTypeBuilder<SalesInvoice> builder)
    {
        builder.Property(x => x.InvoiceNumber).HasMaxLength(30).IsRequired();

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.SalesInvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.HasIndex(x => x.InvoiceDate);
        builder.HasIndex(x => new { x.CustomerId, x.InvoiceDate });
    }
}

public sealed class SalesInvoiceItemConfiguration : IEntityTypeConfiguration<SalesInvoiceItem>
{
    public void Configure(EntityTypeBuilder<SalesInvoiceItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);
        builder.Property(x => x.SellingUnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);

        builder.HasOne(x => x.Item)
               .WithMany()
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ItemId);
    }
}

public sealed class SalesReturnConfiguration : IEntityTypeConfiguration<SalesReturn>
{
    public void Configure(EntityTypeBuilder<SalesReturn> builder)
    {
        builder.Property(x => x.ReturnNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.OriginalInvoiceNo).HasMaxLength(30);
        builder.Property(x => x.Remarks).HasMaxLength(500);

        builder.HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
               .WithOne()
               .HasForeignKey(x => x.SalesReturnId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ReturnNumber).IsUnique();
        builder.HasIndex(x => x.ReturnDate);
    }
}

public sealed class SalesReturnItemConfiguration : IEntityTypeConfiguration<SalesReturnItem>
{
    public void Configure(EntityTypeBuilder<SalesReturnItem> builder)
    {
        builder.Property(x => x.Quantity).HasPrecision(18, 3);

        builder.HasOne(x => x.Item)
               .WithMany()
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
