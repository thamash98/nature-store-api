using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using CeyloneNature.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CeyloneNature.Application.Services;

public class AdminService : IAdminService
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminService(IApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var now = DateTime.UtcNow;
        var periodStart = now.AddDays(-30);
        var priorPeriodStart = now.AddDays(-60);

        var nonCancelled = _db.Orders.Where(o => o.Status != OrderStatus.Cancelled);

        var currentRevenue = await nonCancelled.Where(o => o.CreatedAt >= periodStart).SumAsync(o => (decimal?)o.Total) ?? 0m;
        var priorRevenue = await nonCancelled.Where(o => o.CreatedAt >= priorPeriodStart && o.CreatedAt < periodStart).SumAsync(o => (decimal?)o.Total) ?? 0m;
        var totalRevenue = await nonCancelled.SumAsync(o => (decimal?)o.Total) ?? 0m;

        var currentOrders = await _db.Orders.CountAsync(o => o.CreatedAt >= periodStart);
        var priorOrders = await _db.Orders.CountAsync(o => o.CreatedAt >= priorPeriodStart && o.CreatedAt < periodStart);
        var totalOrders = await _db.Orders.CountAsync();

        var customers = await _userManager.GetUsersInRoleAsync("customer");
        var totalCustomers = customers.Count;
        var currentCustomers = customers.Count(u => u.CreatedAt >= periodStart);
        var priorCustomers = customers.Count(u => u.CreatedAt >= priorPeriodStart && u.CreatedAt < periodStart);

        static double PercentChange(decimal current, decimal prior) =>
            prior == 0 ? (current == 0 ? 0 : 100) : (double)Math.Round((current - prior) / prior * 100, 1);

        return new AdminStatsDto
        {
            TotalRevenue = totalRevenue,
            RevenueChange = PercentChange(currentRevenue, priorRevenue),
            TotalOrders = totalOrders,
            OrdersChange = PercentChange(currentOrders, priorOrders),
            ActiveCustomers = totalCustomers,
            CustomersChange = PercentChange(currentCustomers, priorCustomers)
        };
    }

    public async Task<List<RevenueDataPointDto>> GetRevenueAsync()
    {
        var now = DateTime.UtcNow;
        var start = now.AddDays(-7 * 8);

        var orders = await _db.Orders
            .Where(o => o.CreatedAt >= start && o.Status != OrderStatus.Cancelled)
            .Select(o => new { o.CreatedAt, o.Total })
            .ToListAsync();

        var points = new List<RevenueDataPointDto>();
        for (int week = 0; week < 8; week++)
        {
            var weekStart = start.AddDays(week * 7);
            var weekEnd = weekStart.AddDays(7);
            var revenue = orders.Where(o => o.CreatedAt >= weekStart && o.CreatedAt < weekEnd).Sum(o => o.Total);
            points.Add(new RevenueDataPointDto { Week = $"WK {week + 1}", Revenue = revenue });
        }

        return points;
    }

    public async Task<List<InventoryAlertDto>> GetInventoryAlertsAsync()
    {
        var products = await _db.Products
            .Where(p => p.StockCount != null && p.StockCount < 15)
            .OrderBy(p => p.StockCount)
            .ToListAsync();

        return products.Select(p => new InventoryAlertDto
        {
            Id = p.Id,
            ProductName = p.Name,
            Image = p.Image,
            StockCount = p.StockCount ?? 0,
            Status = p.StockCount == 0 ? "out-of-stock" : p.StockCount < 5 ? "critical" : "low"
        }).ToList();
    }

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        var customers = await _userManager.GetUsersInRoleAsync("customer");
        var userIds = customers.Select(u => u.Id).ToList();

        var orderAggregates = await _db.Orders
            .Where(o => o.UserId != null && userIds.Contains(o.UserId.Value))
            .GroupBy(o => o.UserId!.Value)
            .Select(g => new { UserId = g.Key, Count = g.Count(), Total = g.Sum(o => o.Total) })
            .ToDictionaryAsync(g => g.UserId);

        return customers.Select(u =>
        {
            orderAggregates.TryGetValue(u.Id, out var agg);
            var initials = string.Join("", u.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w[0]).Take(2)).ToUpperInvariant();
            return new CustomerDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? "",
                Initials = initials,
                TotalOrders = agg?.Count ?? 0,
                TotalSpent = agg?.Total ?? 0,
                JoinedDate = u.CreatedAt,
                Status = (agg?.Count ?? 0) > 0 ? "active" : "inactive"
            };
        }).ToList();
    }
}
