using System.Text.Json;
namespace FoodApp.Models;
public static class MealMapper
{
    public static MealDb ToDb(Meal meal)
    {
        return new MealDb
        {
            Id = Int32.Parse(meal.idMeal),
            Name = meal.strMeal,
            Category = meal.strCategory,
            Area = meal.strArea,
            Instructions = meal.strInstructions,
            Tags = meal.strTags,
            YoutubeLink = meal.strYoutube,

            IngredientsJson = JsonSerializer.Serialize(meal.GetIngredients())
        };
    }
}