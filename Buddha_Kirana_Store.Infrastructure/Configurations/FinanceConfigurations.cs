using Buddha_Kirana_Store.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Buddha_Kirana_Store.Data.Configurations;

public sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.Property(x => x.Remarks).HasMaxLength(500);

        // PartyId points at a customer or a supplier depending on PartyType, so it cannot be a foreign key.
        builder.HasIndex(x => new { x.PartyType, x.PartyId, x.TransactionDate });
    }
}

public sealed class ShopExpenseConfiguration : IEntityTypeConfiguration<ShopExpense>
{
    public void Configure(EntityTypeBuilder<ShopExpense> builder)
    {
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Remarks).HasMaxLength(500);

        builder.HasIndex(x => x.ExpenseDate);
    }
}

public sealed class DocumentSequenceConfiguration : IEntityTypeConfiguration<DocumentSequence>
{
    public void Configure(EntityTypeBuilder<DocumentSequence> builder)
    {
        builder.Property(x => x.SequenceKey).HasMaxLength(40).IsRequired();
        builder.HasIndex(x => x.SequenceKey).IsUnique();
    }
}

public sealed class CashBookEntryConfiguration : IEntityTypeConfiguration<CashBookEntry>
{
    public void Configure(EntityTypeBuilder<CashBookEntry> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(CashBookEntry.MaxDescriptionLength).IsRequired();

        // Day book / balance queries: by date, optionally by payment mode.
        builder.HasIndex(x => x.EntryDate);
        builder.HasIndex(x => new { x.PaymentMode, x.EntryDate });

        // Finding the entries that belong to one sale / return / expense.
        builder.HasIndex(x => new { x.Source, x.SourceId });
    }
}
