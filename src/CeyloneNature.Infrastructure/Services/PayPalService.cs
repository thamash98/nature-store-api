using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CeyloneNature.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CeyloneNature.Infrastructure.Services;

public class PayPalService : IPayPalService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public PayPalService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
        var mode = _config["PayPal:Mode"] ?? "sandbox";
        _http.BaseAddress = new Uri(mode == "live" ? "https://api-m.paypal.com" : "https://api-m.sandbox.paypal.com");
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = _config["PayPal:ClientId"];
        var secret = _config["PayPal:ClientSecret"];
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials"
        });

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }

    public async Task<string> CreateOrderAsync(decimal total, string currency = "USD")
    {
        var token = await GetAccessTokenAsync();

        var body = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = currency,
                        value = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v2/checkout/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"PayPal create order failed: {json}");

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("id").GetString()!;
    }

    public async Task<PayPalCaptureResult> CaptureOrderAsync(string paypalOrderId)
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/v2/checkout/orders/{paypalOrderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return new PayPalCaptureResult { Success = false, Status = "FAILED" };

        using var doc = JsonDocument.Parse(json);
        var status = doc.RootElement.GetProperty("status").GetString() ?? "";
        string? captureId = null;
        try
        {
            captureId = doc.RootElement
                .GetProperty("purchase_units")[0]
                .GetProperty("payments")
                .GetProperty("captures")[0]
                .GetProperty("id")
                .GetString();
        }
        catch { /* structure not present, leave null */ }

        return new PayPalCaptureResult
        {
            Success = status == "COMPLETED",
            Status = status,
            CaptureId = captureId
        };
    }
}
