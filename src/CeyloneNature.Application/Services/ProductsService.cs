using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using CeyloneNature.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CeyloneNature.Application.Services;

public class ProductsService : IProductsService
{
    private readonly IApplicationDbContext _db;

    public ProductsService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedProductsDto> GetProductsAsync(string? category, decimal? minPrice, decimal? maxPrice,
        string? popularity, string? search, string? sortBy, int page, int pageSize)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.CategorySlug == category);
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Description.ToLower().Contains(term) ||
                p.Category.ToLower().Contains(term));
        }

        if (popularity == "best-sellers")
            query = query.Where(p => p.IsBestSeller);
        else if (popularity == "new-arrivals")
            query = query.Where(p => p.IsNew);
        else if (popularity == "top-rated")
            query = query.OrderByDescending(p => p.Rating);

        query = sortBy switch
        {
            "price-asc" => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating),
            "newest" => query.OrderByDescending(p => p.IsNew).ThenByDescending(p => p.CreatedAt),
            _ => query
        };

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedProductsDto
        {
            Products = items.Select(p => p.ToDto()).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<List<ProductDto>> GetFeaturedAsync(int count)
    {
        var items = await _db.Products.Where(p => p.IsBestSeller || p.IsNew).Take(count).ToListAsync();
        return items.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProductDto?> GetBySlugAsync(string slug)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Slug == slug);
        return product?.ToDto();
    }

    public async Task<List<ProductDto>?> GetRelatedAsync(int id, int count)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        var related = await _db.Products
            .Where(p => p.CategorySlug == product.CategorySlug && p.Id != product.Id)
            .Take(count)
            .ToListAsync();

        return related.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProductDto> CreateAsync(ProductUpsertRequest request)
    {
        if (await _db.Products.AnyAsync(p => p.Slug == request.Slug))
            throw new ValidationException("A product with this slug already exists.");

        var product = new Product
        {
            Name = request.Name,
            Slug = request.Slug,
            Category = request.Category,
            CategorySlug = request.CategorySlug,
            Price = request.Price,
            OriginalPrice = request.OriginalPrice,
            Discount = request.Discount,
            Image = request.Image,
            Images = request.Images ?? new List<string> { request.Image },
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Ingredients = request.Ingredients,
            Benefits = request.Benefits ?? new(),
            Usage = request.Usage,
            Shipping = request.Shipping,
            InStock = request.InStock,
            StockCount = request.StockCount,
            IsNew = request.IsNew,
            IsBestSeller = request.IsBestSeller,
            Tags = request.Tags ?? new(),
            Rating = 0,
            ReviewCount = 0,
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return product.ToDto();
    }

    public async Task<ProductDto?> UpdateAsync(int id, ProductUpsertRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        product.Name = request.Name;
        product.Slug = request.Slug;
        product.Category = request.Category;
        product.CategorySlug = request.CategorySlug;
        product.Price = request.Price;
        product.OriginalPrice = request.OriginalPrice;
        product.Discount = request.Discount;
        product.Image = request.Image;
        product.Images = request.Images ?? new List<string> { request.Image };
        product.Description = request.Description;
        product.ShortDescription = request.ShortDescription;
        product.Ingredients = request.Ingredients;
        product.Benefits = request.Benefits ?? new();
        product.Usage = request.Usage;
        product.Shipping = request.Shipping;
        product.InStock = request.InStock;
        product.StockCount = request.StockCount;
        product.IsNew = request.IsNew;
        product.IsBestSeller = request.IsBestSeller;
        product.Tags = request.Tags ?? new();

        await _db.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }
}
