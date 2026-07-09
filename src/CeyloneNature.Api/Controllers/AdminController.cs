using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeyloneNature.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminController(IAdminService admin)
    {
        _admin = admin;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetStats() => Ok(await _admin.GetStatsAsync());

    [HttpGet("revenue")]
    public async Task<ActionResult<List<RevenueDataPointDto>>> GetRevenue() => Ok(await _admin.GetRevenueAsync());

    [HttpGet("inventory-alerts")]
    public async Task<ActionResult<List<InventoryAlertDto>>> GetInventoryAlerts() => Ok(await _admin.GetInventoryAlertsAsync());

    [HttpGet("customers")]
    public async Task<ActionResult<List<CustomerDto>>> GetCustomers() => Ok(await _admin.GetCustomersAsync());
}
