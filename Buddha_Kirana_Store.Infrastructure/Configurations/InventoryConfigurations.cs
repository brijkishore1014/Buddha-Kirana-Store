using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buddha_Kirana_Store.Data.Configurations;

public sealed class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ShortCode).HasMaxLength(10).IsRequired();

        builder.HasData(
            new UnitOfMeasure { Id = 1, Name = "Kilogram", ShortCode = "Kg", AllowDecimals = true },
            new UnitOfMeasure { Id = 2, Name = "Gram", ShortCode = "Gm", AllowDecimals = true },
            new UnitOfMeasure { Id = 3, Name = "Litre", ShortCode = "Ltr", AllowDecimals = true },
            new UnitOfMeasure { Id = 4, Name = "Piece", ShortCode = "Pcs", AllowDecimals = false },
            new UnitOfMeasure { Id = 5, Name = "Packet", ShortCode = "Pkt", AllowDecimals = false });
    }
}

public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ShortCode).HasMaxLength(30);
        builder.Property(x => x.CurrentStock).HasPrecision(18, 3);
        builder.Property(x => x.MinStockThreshold).HasPrecision(18, 3);

        builder.HasOne(x => x.Unit)
               .WithMany()
               .HasForeignKey(x => x.UnitId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name);
    }
}

public sealed class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.Property(x => x.QuantityChanged).HasPrecision(18, 3);
        builder.Property(x => x.Remarks).HasMaxLength(500).IsRequired();

        builder.HasOne(x => x.Item)
               .WithMany()
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ItemId, x.AdjustmentDate });
    }
}

public sealed class StockLedgerConfiguration : IEntityTypeConfiguration<StockLedger>
{
    public void Configure(EntityTypeBuilder<StockLedger> builder)
    {
        builder.Property(x => x.QtyIn).HasPrecision(18, 3);
        builder.Property(x => x.QtyOut).HasPrecision(18, 3);
        builder.Property(x => x.ClosingStock).HasPrecision(18, 3);

        // Real foreign key (the old model had none), which also gives us the ItemId index.
        builder.HasOne<Item>()
               .WithMany()
               .HasForeignKey(x => x.ItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ItemId, x.CreatedAt });
        builder.HasIndex(x => new { x.TransactionType, x.ReferenceId });
    }
}
