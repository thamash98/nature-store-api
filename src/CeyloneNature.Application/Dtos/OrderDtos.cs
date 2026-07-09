namespace CeyloneNature.Application.Dtos;

public class ShippingAddressDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string StreetAddress { get; set; } = "";
    public string City { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Country { get; set; } = "";
}

public class CartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class PlaceOrderRequest
{
    public List<CartItemRequest> Items { get; set; } = new();
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public string? PromoCode { get; set; }
    public string? PayPalOrderId { get; set; }
    public string? PayPalCaptureId { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string ProductImage { get; set; } = "";
    public string ProductSlug { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class OrderDto
{
    public string Id { get; set; } = "";
    public string OrderNumber { get; set; } = "";
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public List<OrderItemDto> Items { get; set; } = new();
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public string PaymentMethod { get; set; } = "";
    public string Status { get; set; } = "";
    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? EstimatedDelivery { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = "";
}
