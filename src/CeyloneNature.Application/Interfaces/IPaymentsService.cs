using CeyloneNature.Application.Dtos;

namespace CeyloneNature.Application.Interfaces;

public interface IPaymentsService
{
    Task<CreatePayPalOrderResponse> CreateOrderAsync(CreatePayPalOrderRequest request);
    Task<OrderDto> CaptureOrderAsync(string paypalOrderId, CapturePayPalOrderRequest request, int? userId);
}
