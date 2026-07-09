using CeyloneNature.Application.Dtos;
using CeyloneNature.Domain.Entities;

namespace CeyloneNature.Application.Services;

public static class MappingExtensions
{
    public static ProductDto ToDto(this Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Category = p.Category,
        CategorySlug = p.CategorySlug,
        Price = p.Price,
        OriginalPrice = p.OriginalPrice,
        Discount = p.Discount,
        Rating = p.Rating,
        ReviewCount = p.ReviewCount,
        Image = p.Image,
        Images = p.Images,
        Description = p.Description,
        ShortDescription = p.ShortDescription,
        Ingredients = p.Ingredients,
        Benefits = p.Benefits,
        Usage = p.Usage,
        Shipping = p.Shipping,
        InStock = p.InStock,
        StockCount = p.StockCount,
        IsNew = p.IsNew,
        IsBestSeller = p.IsBestSeller,
        Tags = p.Tags,
    };
}
