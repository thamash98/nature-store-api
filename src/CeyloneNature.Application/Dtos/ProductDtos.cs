namespace CeyloneNature.Application.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Category { get; set; } = "";
    public string CategorySlug { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int? Discount { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public string Image { get; set; } = "";
    public List<string>? Images { get; set; }
    public string Description { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string? Ingredients { get; set; }
    public List<string>? Benefits { get; set; }
    public string? Usage { get; set; }
    public string? Shipping { get; set; }
    public bool InStock { get; set; }
    public int? StockCount { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsBestSeller { get; set; }
    public List<string>? Tags { get; set; }
}

public class ProductUpsertRequest
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Category { get; set; } = "";
    public string CategorySlug { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int? Discount { get; set; }
    public string Image { get; set; } = "";
    public List<string>? Images { get; set; }
    public string Description { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string? Ingredients { get; set; }
    public List<string>? Benefits { get; set; }
    public string? Usage { get; set; }
    public string? Shipping { get; set; }
    public bool InStock { get; set; }
    public int? StockCount { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
    public List<string>? Tags { get; set; }
}

public class PaginatedProductsDto
{
    public List<ProductDto> Products { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Image { get; set; } = "";
    public int ProductCount { get; set; }
    public bool? Featured { get; set; }
}
