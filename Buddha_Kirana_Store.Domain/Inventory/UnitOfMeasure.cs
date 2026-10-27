namespace Buddha_Kirana_Store.Domain;

public class UnitOfMeasure
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;       // e.g., Kilogram, Piece
    public string ShortCode { get; set; } = string.Empty;  // e.g., Kg, Pcs
    public bool AllowDecimals { get; set; }                // true for Kg/Ltr, false for Pcs
}
