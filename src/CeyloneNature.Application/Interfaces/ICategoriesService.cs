using CeyloneNature.Application.Dtos;

namespace CeyloneNature.Application.Interfaces;

public interface ICategoriesService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
}
