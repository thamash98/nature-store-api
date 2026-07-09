using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CeyloneNature.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _products;

    public ProductsController(IProductsService products)
    {
        _products = products;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedProductsDto>> GetProducts(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? popularity,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 6)
        => Ok(await _products.GetProductsAsync(category, minPrice, maxPrice, popularity, search, sortBy, page, pageSize));

    [HttpGet("featured")]
    public async Task<ActionResult<List<ProductDto>>> GetFeatured([FromQuery] int count = 4)
        => Ok(await _products.GetFeaturedAsync(count));

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProductDto>> GetBySlug(string slug)
    {
        var product = await _products.GetBySlugAsync(slug);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpGet("{id:int}/related")]
    public async Task<ActionResult<List<ProductDto>>> GetRelated(int id, [FromQuery] int count = 4)
    {
        var related = await _products.GetRelatedAsync(id, count);
        return related == null ? NotFound() : Ok(related);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductDto>> Create(ProductUpsertRequest request)
    {
        var product = await _products.CreateAsync(request);
        return CreatedAtAction(nameof(GetBySlug), new { slug = product.Slug }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ProductDto>> Update(int id, ProductUpsertRequest request)
    {
        var product = await _products.UpdateAsync(id, request);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
        => await _products.DeleteAsync(id) ? NoContent() : NotFound();
}
