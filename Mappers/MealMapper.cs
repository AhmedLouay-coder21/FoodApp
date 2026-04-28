namespace FoodApp.Mappers;

using FoodApp.Models;
using System.Text.Json;

public static class MealMapper
{
    // API → ViewModel
    public static MealViewModel FromApi(Meal meal)
    {
        return new MealViewModel
        {
            Name = meal.strMeal,
            Category = meal.strCategory,
            Area = meal.strArea,
            Instructions = meal.strInstructions,
            Image = meal.strMealThumb,
            Ingredients = meal.GetIngredients()
        };
    }

    // DB → ViewModel
    public static MealViewModel FromDb(MealDb meal)
    {
        return new MealViewModel
        {
            Name = meal.Name ?? "Unknown",
            Category = meal.Category ?? "Unknown",
            Area = meal.Area ?? "Unknown",
            Image = meal.Image ?? "",
            Instructions = meal.Instructions ?? "Unknown",
            Tags = meal.Tags ?? "Unknown",
            YoutubeLink = meal.YoutubeLink ?? "Unknown",
            Ingredients = JsonSerializer.Deserialize<List<Ingredient>>(meal.IngredientsJson ?? "[]") 
                          ?? new List<Ingredient>()
        };
    }

    // API → DB
    public static MealDb ToDb(Meal meal)
    {
        return new MealDb
        {
            Id = int.Parse(meal.idMeal),
            Name = meal.strMeal,
            Category = meal.strCategory,
            Area = meal.strArea,
            Instructions = meal.strInstructions,
            Tags = meal.strTags,
            YoutubeLink = meal.strYoutube,
            Image = meal.strMealThumb,
            IngredientsJson = JsonSerializer.Serialize(meal.GetIngredients())
        };
    }
}