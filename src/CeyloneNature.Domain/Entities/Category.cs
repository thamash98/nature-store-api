namespace CeyloneNature.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Image { get; set; } = "";
    public bool Featured { get; set; }
}
