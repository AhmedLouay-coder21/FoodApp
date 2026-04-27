using FoodApp.Models;

namespace FoodApp.Data;

public interface IMealRepository
{
    Task AddMealAsync(MealDb meal);
    Task<List<MealDb>> GetAllMealsAsync();
}