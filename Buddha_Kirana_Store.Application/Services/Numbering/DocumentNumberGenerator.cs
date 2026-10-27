using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class DocumentNumberGenerator(IClock clock) : IDocumentNumberGenerator
{
    private static readonly IReadOnlyDictionary<DocumentType, string> Prefixes = new Dictionary<DocumentType, string>
    {
        [DocumentType.SalesInvoice] = "INV",
        [DocumentType.SalesReturn] = "SRN",
        [DocumentType.PurchaseReturn] = "PRN"
    };

    public async Task<string> NextAsync(IUnitOfWork uow, DocumentType type)
    {
        var key = $"{Prefixes[type]}-{clock.Now:yyyyMMdd}";

        var sequence = await uow.DocumentSequences.GetByKeyAsync(key);
        if (sequence is null)
        {
            sequence = new DocumentSequence { SequenceKey = key };
            uow.DocumentSequences.Add(sequence);
        }

        return $"{key}-{sequence.Next():D4}";
    }
}
