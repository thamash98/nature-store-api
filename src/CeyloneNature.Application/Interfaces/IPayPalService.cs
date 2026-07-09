namespace CeyloneNature.Application.Interfaces;

public class PayPalCaptureResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = "";
    public string? CaptureId { get; set; }
}

public interface IPayPalService
{
    Task<string> CreateOrderAsync(decimal total, string currency = "USD");
    Task<PayPalCaptureResult> CaptureOrderAsync(string paypalOrderId);
}
