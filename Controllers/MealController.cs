using Spectre.Console;
using FoodApp.Services;
using Microsoft.Data.Sqlite;
namespace FoodApp.Controllers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FoodApp.Data;
using FoodApp.Models;
using FoodApp.Mappers;

public class MealController
{
    private readonly IMealService _mealService;
    private readonly IMealRepository _repo;

    public MealController(IMealService mealService, IMealRepository repo)
    {
        _mealService = mealService;
        _repo = repo;
    }

    public async Task SearchByName(string name)
    {
        var meals = await _mealService.SearchByName(name);

        if (meals.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No meals found![/]");
            return;
        }
        var vm = meals.Select(m => MealMapper.FromApi(m)).ToList();
        var meal = await DisplayMeal(vm);
        AnsiConsole.MarkupLine("[gray]Press shift + s to add this meal to favorite[/]");
        AnsiConsole.MarkupLine("[gray]Press any other key to continue[/]");
        var key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.S && key.Modifiers.HasFlag(ConsoleModifiers.Shift))
        {
            var mealDb = new MealDb
            {
                Name = meal.Name,
                Category = meal.Category,
                Area = meal.Area,
                Image = meal.Image,
                Instructions = meal.Instructions,
                Tags = meal.Tags,
                YoutubeLink = meal.YoutubeLink,
                IngredientsJson = JsonSerializer.Serialize(meal.Ingredients)
            };
            await _repo.AddMealAsync(mealDb);
            AnsiConsole.MarkupLine("[green]Saved successfully![/]");
        }
    }
    public async Task GetMeal(MealDbContext db) 
    { 
        var meals = await db.Meals.ToListAsync();
        var vm = meals.Select(m => MealMapper.FromDb(m)).ToList();
        await DisplayMeal(vm);
        AnsiConsole.MarkupLine("[gray]Press any other key to continue[/]");
        Console.ReadKey(true);
        AnsiConsole.Clear();
    }
    private async Task<MealViewModel> DisplayMeal(List<MealViewModel> meals)
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Select a [OrangeRed1]meal[/]") 
            .PageSize(15);
        var groupedMeals = meals
            .GroupBy(m => m.Category!)
            .OrderBy(g => g.Key);

        foreach (var group in groupedMeals) 
        { 
            prompt.AddChoiceGroup(group.Key, group.Select(m => m.Name!)); 
        }
        
        var selectedMealName = AnsiConsole.Prompt(prompt);
        var meal = meals.FirstOrDefault(m => m.Name == selectedMealName);
        if (meals.Count == 0 && meal != null)
        {
            AnsiConsole.MarkupLine("[red]No meals found![/]");
            return meal;
        }

        if (!string.IsNullOrEmpty(meal?.Image))
        {
            var localPath = await _mealService.SaveImageAsync(meal.Image, meal.Name);
            var image = new CanvasImage(localPath)
                .MaxWidth(100)
                .BicubicResampler();

            AnsiConsole.Write(image);
        }

        var ingredientsText = string.Join("\n",
            meal?.Ingredients?.Select(i =>
                string.IsNullOrWhiteSpace(i.Measure)
                    ? $"- {i.Name}"
                    : $"- {i.Name} ({i.Measure})"
            ) ?? new List<string>()
        );

        var panel = new Panel($@"
    [OrangeRed1]Meal:[/] {Markup.Escape(meal.Name)}
    [OrangeRed1]Category:[/] {Markup.Escape(meal.Category)}
    [OrangeRed1]Area:[/] {Markup.Escape(meal.Area)}

    [OrangeRed1]Ingredients:[/]
    {Markup.Escape(ingredientsText)}

    [OrangeRed1]Instructions:[/]
    {Markup.Escape(meal.Instructions)}
        ")
        {
            Header = new PanelHeader("Meal Details"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1)
        };

        AnsiConsole.Write(panel);
        return meal;
    }  
}