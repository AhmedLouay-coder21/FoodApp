using FoodApp.Models;

namespace FoodApp.Services;

public interface IMealService
{
    Task<List<Meal>> SearchByName(string name);
    Task<string> SaveImageAsync(string url, string name);
}