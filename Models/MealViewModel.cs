using FoodApp.Models;

public class MealViewModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Area { get; set; } = "";
    public string Image { get; set; } = "";
    public string Instructions { get; set; } = "";
    public string Tags{ get; set; } = "";
    public string YoutubeLink { get; set; } = "";
    public List<Ingredient> Ingredients { get; set; } = new();
}