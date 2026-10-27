namespace Buddha_Kirana_Store.Domain;

/// <summary>
/// Running counter used to build unique document numbers (e.g. INV-20261024-0001).
/// One row per prefix + day.
/// </summary>
public class DocumentSequence
{
    public int Id { get; set; }
    public string SequenceKey { get; set; } = string.Empty;
    public int LastValue { get; private set; }

    public int Next() => ++LastValue;
}
