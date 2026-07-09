using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using CeyloneNature.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CeyloneNature.Application.Services;

public class PaymentsService : IPaymentsService
{
    private readonly IApplicationDbContext _db;
    private readonly OrderPricingService _pricing;
    private readonly IPayPalService _payPal;
    private readonly UserManager<ApplicationUser> _userManager;

    public PaymentsService(IApplicationDbContext db, OrderPricingService pricing, IPayPalService payPal, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _pricing = pricing;
        _payPal = payPal;
        _userManager = userManager;
    }

    public async Task<CreatePayPalOrderResponse> CreateOrderAsync(CreatePayPalOrderRequest request)
    {
        var priced = await _pricing.PriceCartAsync(request.Items, request.PromoCode);

        string paypalOrderId;
        try
        {
            paypalOrderId = await _payPal.CreateOrderAsync(priced.Total);
        }
        catch (Exception ex)
        {
            throw new PaymentGatewayException($"Could not create PayPal order: {ex.Message}");
        }

        return new CreatePayPalOrderResponse
        {
            PayPalOrderId = paypalOrderId,
            Subtotal = priced.Subtotal,
            Shipping = priced.Shipping,
            Discount = priced.Discount,
            Total = priced.Total
        };
    }

    public async Task<OrderDto> CaptureOrderAsync(string paypalOrderId, CapturePayPalOrderRequest request, int? userId)
    {
        var priced = await _pricing.PriceCartAsync(request.Items, request.PromoCode);

        var capture = await _payPal.CaptureOrderAsync(paypalOrderId);
        if (!capture.Success)
            throw new ValidationException($"Payment was not completed (status: {capture.Status}).");

        var orderNumber = $"#CNS-{8295 + await _db.Orders.CountAsync()}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = userId,
            CustomerName = request.ShippingAddress.FullName,
            CustomerEmail = request.ShippingAddress.Email,
            ShippingAddress = new ShippingAddress
            {
                FullName = request.ShippingAddress.FullName,
                Email = request.ShippingAddress.Email,
                StreetAddress = request.ShippingAddress.StreetAddress,
                City = request.ShippingAddress.City,
                PostalCode = request.ShippingAddress.PostalCode,
                Country = request.ShippingAddress.Country
            },
            PaymentMethod = "paypal",
            Status = OrderStatus.Processing,
            Subtotal = priced.Subtotal,
            Shipping = priced.Shipping,
            Discount = priced.Discount,
            Total = priced.Total,
            PromoCode = priced.PromoCode,
            PayPalOrderId = paypalOrderId,
            PayPalCaptureId = capture.CaptureId,
            EstimatedDelivery = DateTime.UtcNow.AddDays(5),
        };

        foreach (var line in priced.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = line.Product.Id,
                ProductName = line.Product.Name,
                ProductImage = line.Product.Image,
                ProductSlug = line.Product.Slug,
                UnitPrice = line.Product.Price,
                Quantity = line.Quantity
            });

            if (line.Product.StockCount.HasValue)
                line.Product.StockCount = Math.Max(0, line.Product.StockCount.Value - line.Quantity);
        }

        if (userId.HasValue)
        {
            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user != null)
            {
                user.LoyaltyPoints += (int)Math.Floor(priced.Total);
                await _userManager.UpdateAsync(user);
            }
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return OrderPricingService.ToDto(order);
    }
}
