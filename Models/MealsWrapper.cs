namespace FoodApp.Models;

public record MealWrapper(List<Meal>? meals);

public record Meal(
    string idMeal, 
    string strMeal,
    string strCategory,
    string strArea, 
    string strInstructions, 
    string strMealThumb
);