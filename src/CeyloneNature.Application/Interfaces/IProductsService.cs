using CeyloneNature.Application.Dtos;

namespace CeyloneNature.Application.Interfaces;

public interface IProductsService
{
    Task<PaginatedProductsDto> GetProductsAsync(string? category, decimal? minPrice, decimal? maxPrice,
        string? popularity, string? search, string? sortBy, int page, int pageSize);
    Task<List<ProductDto>> GetFeaturedAsync(int count);
    Task<ProductDto?> GetBySlugAsync(string slug);
    Task<List<ProductDto>?> GetRelatedAsync(int id, int count);
    Task<ProductDto> CreateAsync(ProductUpsertRequest request);
    Task<ProductDto?> UpdateAsync(int id, ProductUpsertRequest request);
    Task<bool> DeleteAsync(int id);
}
