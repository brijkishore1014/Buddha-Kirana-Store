namespace Buddha_Kirana_Store.Abstractions;

/// <summary>Abstracts "now" so business code can be tested with a fixed time.</summary>
public interface IClock
{
    DateTime Now { get; }
}
