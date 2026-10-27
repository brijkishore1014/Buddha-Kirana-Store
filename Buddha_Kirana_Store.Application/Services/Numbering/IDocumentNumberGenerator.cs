using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Creates unique, human-readable document numbers inside the caller's unit of work.</summary>
public interface IDocumentNumberGenerator
{
    Task<string> NextAsync(IUnitOfWork uow, DocumentType type);
}
