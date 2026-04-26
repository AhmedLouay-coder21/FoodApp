namespace FoodApp.Models;

public record MealWrapper(List<Meal>? meals);
public record Ingredient(string Name, string Measure);

public record Meal(
    string idMeal,
    string strMeal,
    string strCategory,
    string strArea,
    string strInstructions,
    string strMealThumb,
    string strTags,
    string strYoutube,

    string? strIngredient1,
    string? strIngredient2,
    string? strIngredient3,
    string? strIngredient4,
    string? strIngredient5,
    string? strIngredient6,
    string? strIngredient7,
    string? strIngredient8,
    string? strIngredient9,
    string? strIngredient10,
    string? strIngredient11,
    string? strIngredient12,
    string? strIngredient13,
    string? strIngredient14,
    string? strIngredient15,
    string? strIngredient16,
    string? strIngredient17,
    string? strIngredient18,
    string? strIngredient19,
    string? strIngredient20,

    string? strMeasure1,
    string? strMeasure2,
    string? strMeasure3,
    string? strMeasure4,
    string? strMeasure5,
    string? strMeasure6,
    string? strMeasure7,
    string? strMeasure8,
    string? strMeasure9,
    string? strMeasure10,
    string? strMeasure11,
    string? strMeasure12,
    string? strMeasure13,
    string? strMeasure14,
    string? strMeasure15,
    string? strMeasure16,
    string? strMeasure17,
    string? strMeasure18,
    string? strMeasure19,
    string? strMeasure20
)
{
    public List<Ingredient> GetIngredients()
    {
        var list = new List<Ingredient>();

        for (int i = 1; i <= 20; i++)
        {
            var ingredient = GetType().GetProperty($"strIngredient{i}")?.GetValue(this) as string;
            var measure = GetType().GetProperty($"strMeasure{i}")?.GetValue(this) as string;

            if (!string.IsNullOrWhiteSpace(ingredient))
            {
                list.Add(new Ingredient(
                    ingredient.Trim(),
                    measure?.Trim() ?? ""
                ));
            }
        }

        return list;
    }
}