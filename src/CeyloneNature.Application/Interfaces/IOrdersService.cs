using CeyloneNature.Application.Dtos;

namespace CeyloneNature.Application.Interfaces;

public interface IOrdersService
{
    Task<List<OrderDto>> GetMineAsync(int userId);
    Task<List<OrderDto>> GetAllAsync(string? status);
    Task<OrderDto> GetByIdAsync(string id, bool isAdmin, int? userId);
    Task<OrderDto> UpdateStatusAsync(string id, string status);
}
