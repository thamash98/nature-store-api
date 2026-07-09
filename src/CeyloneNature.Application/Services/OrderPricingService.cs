using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CeyloneNature.Application.Services;

public class PricedLineItem
{
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal LineTotal => Product.Price * Quantity;
}

public class PricedCart
{
    public List<PricedLineItem> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Shipping { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public string? PromoCode { get; set; }
}

public class OrderPricingService
{
    private readonly IApplicationDbContext _db;

    public OrderPricingService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PricedCart> PriceCartAsync(List<CartItemRequest> items, string? promoCode)
    {
        if (items.Count == 0)
            throw new ValidationException("Cart is empty.");

        var productIds = items.Select(i => i.ProductId).ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        var lineItems = new List<PricedLineItem>();
        foreach (var item in items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId)
                ?? throw new ValidationException($"Product {item.ProductId} not found.");
            if (item.Quantity < 1)
                throw new ValidationException("Quantity must be at least 1.");
            if (product.StockCount.HasValue && product.StockCount.Value < item.Quantity)
                throw new ValidationException($"'{product.Name}' has insufficient stock.");

            lineItems.Add(new PricedLineItem { Product = product, Quantity = item.Quantity });
        }

        var subtotal = lineItems.Sum(i => i.LineTotal);
        var shipping = subtotal >= 50 ? 0m : 5.00m;

        decimal discount = 0m;
        string? appliedPromo = null;
        if (!string.IsNullOrWhiteSpace(promoCode) && promoCode.Trim().ToUpperInvariant() == "NATURE10")
        {
            discount = Math.Round(subtotal * 0.10m, 2);
            appliedPromo = "NATURE10";
        }

        var total = subtotal + shipping - discount;

        return new PricedCart
        {
            Items = lineItems,
            Subtotal = subtotal,
            Shipping = shipping,
            Discount = discount,
            Total = total,
            PromoCode = appliedPromo
        };
    }

    public static OrderDto ToDto(Order order) => new()
    {
        Id = order.Id.ToString(),
        OrderNumber = order.OrderNumber,
        CustomerId = order.UserId,
        CustomerName = order.CustomerName,
        CustomerEmail = order.CustomerEmail,
        Items = order.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductImage = i.ProductImage,
            ProductSlug = i.ProductSlug,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity
        }).ToList(),
        ShippingAddress = new ShippingAddressDto
        {
            FullName = order.ShippingAddress.FullName,
            Email = order.ShippingAddress.Email,
            StreetAddress = order.ShippingAddress.StreetAddress,
            City = order.ShippingAddress.City,
            PostalCode = order.ShippingAddress.PostalCode,
            Country = order.ShippingAddress.Country
        },
        PaymentMethod = order.PaymentMethod,
        Status = order.Status.ToString().ToLowerInvariant(),
        Subtotal = order.Subtotal,
        Shipping = order.Shipping,
        Discount = order.Discount,
        Total = order.Total,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt,
        EstimatedDelivery = order.EstimatedDelivery
    };
}
