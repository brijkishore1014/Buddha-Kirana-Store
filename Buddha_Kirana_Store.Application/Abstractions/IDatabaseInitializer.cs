namespace Buddha_Kirana_Store.Abstractions;

/// <summary>Brings the database schema up to date at application start.</summary>
public interface IDatabaseInitializer
{
    Task InitializeAsync();
}
