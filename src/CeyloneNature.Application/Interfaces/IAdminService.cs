using CeyloneNature.Application.Dtos;

namespace CeyloneNature.Application.Interfaces;

public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync();
    Task<List<RevenueDataPointDto>> GetRevenueAsync();
    Task<List<InventoryAlertDto>> GetInventoryAlertsAsync();
    Task<List<CustomerDto>> GetCustomersAsync();
}
