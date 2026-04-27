using FoodApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Data;

public class MealRepository : IMealRepository
{
    private readonly MealDbContext _db;

    public MealRepository(MealDbContext db)
    {
        _db = db;
    }

    public async Task AddMealAsync(MealDb meal)
    {
        _db.Meals.Add(meal);
        await _db.SaveChangesAsync();
    }

    public async Task<List<MealDb>> GetAllMealsAsync()
    {
        return await _db.Meals.ToListAsync();
    }
}