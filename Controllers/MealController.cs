using Spectre.Console;
using FoodApp.Services;
using Microsoft.Data.Sqlite;
namespace FoodApp.Controllers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FoodApp.Data;
using FoodApp.Models;
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

        if (!meals.Any())
        {
            AnsiConsole.MarkupLine("[red]No meals found![/]");
            return;
        }

        var prompt = new SelectionPrompt<string>()
            .Title("Select a [OrangeRed1]meal[/]")
            .PageSize(15);

        var groupedMeals = meals
            .GroupBy(m => m.strCategory)
            .OrderBy(g => g.Key);

        foreach (var group in groupedMeals)
        {
            prompt.AddChoiceGroup(group.Key, group.Select(m => m.strMeal));
        }

        var selectedMealName = AnsiConsole.Prompt(prompt);
        var meal = meals.FirstOrDefault(m => m.strMeal == selectedMealName);
        var url = meal?.strMealThumb;
        var mealName = meal?.strMeal;
        if (!string.IsNullOrEmpty(url))
        {
            var localPath = await _mealService.SaveImageAsync(url,mealName);
            var image = new CanvasImage(localPath)
            .MaxWidth(100)
            .BicubicResampler();
            AnsiConsole.Write(image);
        }
        if(meal is null)
        {
            AnsiConsole.MarkupLine("[red]Error: Could not retrieve meal details.[/]");
            return;
        }
        var ingredientsText = string.Join("\n",
    meal.GetIngredients().Select(i =>
        string.IsNullOrWhiteSpace(i.Measure)
            ? $"- {i.Name}"
            : $"- {i.Name} ({i.Measure})"
    ));
    
        var panel = new Panel($@"[OrangeRed1]Meal:[/] {meal.strMeal}
[OrangeRed1]Category:[/] {meal.strCategory}
[OrangeRed1]Area:[/] {meal.strArea}

[OrangeRed1]Ingredients:[/]
{ingredientsText}

[OrangeRed1]Instructions:[/]
{meal.strInstructions}
        ")
        {
            Header = new PanelHeader("Meal Details"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };
        
        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine("[gray]Press shift + s to add this meal to favorite[/]");
        AnsiConsole.MarkupLine("[gray]Press any other key to continue[/]");
        var key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.S && key.Modifiers.HasFlag(ConsoleModifiers.Shift))
        {
            var mealDb = new MealDb
            {
                Name = meal.strMeal,
                Category = meal.strCategory,
                Area = meal.strArea,
                Image = meal.strMeal + ".png",
                Instructions = meal.strInstructions,
                Tags = meal.strTags,
                YoutubeLink = meal.strYoutube,
                IngredientsJson = JsonSerializer.Serialize(meal.GetIngredients())
            };
            await _repo.AddMealAsync(mealDb);
            AnsiConsole.MarkupLine("[green]Saved successfully![/]");
        }
    }
    public async Task GetMeal(MealDbContext db) 
    { 
        var prompt = new SelectionPrompt<string>()
            .Title("Select a [OrangeRed1]meal[/]") 
            .PageSize(15); 
        var meals = await db.Meals.ToListAsync();
        
        var groupedMeals = meals 
            .Where(m => m.Category != null && m.Name != null) 
            .GroupBy(m => m.Category!)
            .OrderBy(g => g.Key);
        foreach (var group in groupedMeals) 
        { 
            prompt.AddChoiceGroup(group.Key, group.Select(m => m.Name!)); 
        } 
        var selectedMealName = AnsiConsole.Prompt(prompt);

        var meal = meals.FirstOrDefault(m => m.Name == selectedMealName);
        if (meal == null)
        {
            AnsiConsole.MarkupLine("[red]Error: Could not retrieve meal details.[/]");
            return;
        }
        var ingredients = JsonSerializer.Deserialize<List<Ingredient>>(meal.IngredientsJson ?? "Unknown")
                        ?? new List<Ingredient>();

        var ingredientsText = string.Join("\n",
            ingredients.Select(i =>
                string.IsNullOrWhiteSpace(i.Measure)
                    ? $"- {i.Name}"
                    : $"- {i.Name} ({i.Measure})"
            )
        );
        if (!string.IsNullOrEmpty(meal.Name))
        {
            var localPath = await _mealService.SaveImageAsync(" ",meal.Name);
            var image = new CanvasImage(localPath)
            .MaxWidth(100)
            .BicubicResampler();
            AnsiConsole.Write(image);
        }
        var panel = new Panel($@"
[OrangeRed1]Meal:[/] {Markup.Escape(meal.Name ?? "Unknown")}
[OrangeRed1]Category:[/] {Markup.Escape(meal.Category ?? "Unknown")}
[OrangeRed1]Area:[/] {Markup.Escape(meal.Area ?? "Unknown")}

[OrangeRed1]Ingredients:[/]
{Markup.Escape(ingredientsText)}

[OrangeRed1]Instructions:[/]
{Markup.Escape(meal.Instructions ?? "Unknown")}
        ")
        {
            Header = new PanelHeader("Meal Details"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };
            AnsiConsole.Write(panel);
            AnsiConsole.MarkupLine("[gray]Press any other key to continue[/]");
            Console.ReadKey(true);
            AnsiConsole.Clear();
    }
}