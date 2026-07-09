using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using CeyloneNature.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CeyloneNature.Application.Services;

public class OrdersService : IOrdersService
{
    private readonly IApplicationDbContext _db;

    public OrdersService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderDto>> GetMineAsync(int userId)
    {
        var orders = await _db.Orders.Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(OrderPricingService.ToDto).ToList();
    }

    public async Task<List<OrderDto>> GetAllAsync(string? status)
    {
        var query = _db.Orders.Include(o => o.Items).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            query = query.Where(o => o.Status == parsedStatus);

        var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        return orders.Select(OrderPricingService.ToDto).ToList();
    }

    public async Task<OrderDto> GetByIdAsync(string id, bool isAdmin, int? userId)
    {
        if (!int.TryParse(id, out var orderId))
            throw new NotFoundException("Order not found.");

        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new NotFoundException("Order not found.");

        if (!isAdmin && order.UserId != userId)
            throw new ForbiddenException("You do not have access to this order.");

        return OrderPricingService.ToDto(order);
    }

    public async Task<OrderDto> UpdateStatusAsync(string id, string status)
    {
        if (!int.TryParse(id, out var orderId))
            throw new NotFoundException("Order not found.");
        if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
            throw new ValidationException("Invalid status.");

        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new NotFoundException("Order not found.");

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return OrderPricingService.ToDto(order);
    }
}
