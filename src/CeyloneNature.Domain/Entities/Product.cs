namespace CeyloneNature.Domain.Entities;

public class Product
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
    public List<string> Images { get; set; } = new();
    public string Description { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string? Ingredients { get; set; }
    public List<string> Benefits { get; set; } = new();
    public string? Usage { get; set; }
    public string? Shipping { get; set; }
    public bool InStock { get; set; }
    public int? StockCount { get; set; }
    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
