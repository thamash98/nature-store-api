namespace CeyloneNature.Application.Dtos;

public class CreatePayPalOrderRequest
{
    public List<CartItemRequest> Items { get; set; } = new();
    public string? PromoCode { get; set; }
}

public class CreatePayPalOrderResponse
{
    public string PayPalOrderId { get; set; } = "";
    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}

public class CapturePayPalOrderRequest
{
    public List<CartItemRequest> Items { get; set; } = new();
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public string? PromoCode { get; set; }
}
