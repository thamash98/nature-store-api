using CeyloneNature.Application.Common;
using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CeyloneNature.Application.Services;

public class CategoriesService : ICategoriesService
{
    private readonly IApplicationDbContext _db;

    public CategoriesService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var categories = await _db.Categories.ToListAsync();
        var counts = await _db.Products
            .GroupBy(p => p.CategorySlug)
            .Select(g => new { Slug = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Slug, g => g.Count);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Image = c.Image,
            Featured = c.Featured,
            ProductCount = counts.TryGetValue(c.Slug, out var n) ? n : 0
        }).ToList();
    }
}
