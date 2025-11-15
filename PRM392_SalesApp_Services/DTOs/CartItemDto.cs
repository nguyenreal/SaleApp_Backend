// Vị trí: .../DTOs/CartItemDto.cs
public class CartItemDto
{
    public int CartItemID { get; init; }
    public int? ProductID { get; init; }
    public string? ProductName { get; init; }
    public string? ImageURL { get; init; } // <<< THÊM DÒNG NÀY
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}