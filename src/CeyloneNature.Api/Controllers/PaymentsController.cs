using System.Security.Claims;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CeyloneNature.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentsService _payments;

    public PaymentsController(IPaymentsService payments)
    {
        _payments = payments;
    }

    [HttpPost("create-order")]
    public async Task<ActionResult<CreatePayPalOrderResponse>> CreateOrder(CreatePayPalOrderRequest request)
        => Ok(await _payments.CreateOrderAsync(request));

    [HttpPost("capture-order/{paypalOrderId}")]
    public async Task<ActionResult<OrderDto>> CaptureOrder(string paypalOrderId, CapturePayPalOrderRequest request)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        int? userId = sub != null && int.TryParse(sub, out var parsedId) ? parsedId : null;

        return Ok(await _payments.CaptureOrderAsync(paypalOrderId, request, userId));
    }
}
