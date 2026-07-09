using CeyloneNature.Application.Dtos;
using CeyloneNature.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CeyloneNature.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categories;

    public CategoriesController(ICategoriesService categories)
    {
        _categories = categories;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        => Ok(await _categories.GetCategoriesAsync());
}
