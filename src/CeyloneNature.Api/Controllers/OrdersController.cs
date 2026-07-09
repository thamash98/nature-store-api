using System.Security.Claims;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeyloneNature.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _orders;

    public OrdersController(IOrdersService orders)
    {
        _orders = orders;
    }

    private int? CurrentUserId
    {
        get
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return sub != null && int.TryParse(sub, out var id) ? id : null;
        }
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<ActionResult<List<OrderDto>>> GetMine()
    {
        var userId = CurrentUserId;
        if (userId == null) return Unauthorized();
        return Ok(await _orders.GetMineAsync(userId.Value));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<OrderDto>>> GetAll([FromQuery] string? status)
        => Ok(await _orders.GetAllAsync(status));

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDto>> GetById(string id)
        => Ok(await _orders.GetByIdAsync(id, User.IsInRole("admin"), CurrentUserId));

    [HttpPut("{id}/status")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(string id, UpdateOrderStatusRequest request)
        => Ok(await _orders.UpdateStatusAsync(id, request.Status));
}
