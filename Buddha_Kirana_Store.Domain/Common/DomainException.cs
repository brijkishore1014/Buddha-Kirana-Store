namespace Buddha_Kirana_Store.Domain;

/// <summary>
/// Raised when a business rule is violated. The message is safe to show to the shop user.
/// </summary>
public class DomainException(string message) : Exception(message);
