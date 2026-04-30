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
            AnsiConsole.MarkupLine("[gray]Press any other key to continue[/]");
            Console.ReadKey();
        }
    }
    public async Task GetMeal(MealDbContext db) 
    { 
        var meals = await db.Meals.ToListAsync();
        var vm = meals.Select(m => MealMapper.FromDb(m)).ToList();
        await DisplayMeal(vm);
        AnsiConsole.MarkupLine("[gray]Press any key to continue[/]");
        Console.ReadKey(true);
        AnsiConsole.Clear();
    }
     public async Task EditMeal(MealDbContext db)
    {
        var meals = await db.Meals.ToListAsync();
        var vm = meals.Select(m => MealMapper.FromDb(m)).ToList();
        var meal = await DisplayMeal(vm);

        var fieldsToEdit = new SelectionPrompt<string>()
            .Title("Choose the fields you want to edit")
            .PageSize(15);
        string[] columnNames = typeof(MealDb).GetProperties()
                .Select(x => x.Name)
                .Where(x => x != "Image" && x != "Id" && x != "Instructions")
                .ToArray();

        fieldsToEdit.AddChoices(columnNames);
        var UserChoice = AnsiConsole.Prompt(fieldsToEdit);

        var mealToUpdate =  meals.FirstOrDefault(m => m.Name == meal.Name);
        if (UserChoice == "IngredientsJson")
        {
            var ingredients = JsonSerializer.Deserialize<List<Ingredient>>(mealToUpdate?.IngredientsJson ?? "[]") 
                        ?? new List<Ingredient>();

            var ingredientToEdit = AnsiConsole.Prompt(
            new SelectionPrompt<Ingredient>()
                .Title("Select an ingredient to modify")
                .UseConverter(i => $"{i.Name} ({i.Measure})")
                .AddChoices(ingredients)
            );

            var partToEdit = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Editing {ingredientToEdit.Name}: What do you want to change?")
                    .AddChoices("Name", "Measure")
            );

            string newValueForPart = AnsiConsole.Ask<string>($"Enter new {partToEdit}:");

            int index = ingredients.IndexOf(ingredientToEdit);
            // Records are immutable, so use 'with'[cite: 2]
            ingredients[index] = partToEdit == "Name" 
                ? ingredientToEdit with { Name = newValueForPart } 
                : ingredientToEdit with { Measure = newValueForPart };

            mealToUpdate?.IngredientsJson = JsonSerializer.Serialize(ingredients);
        }
        else
        {
            var property = mealToUpdate?.GetType().GetProperty(UserChoice);
            if (property != null && property.CanWrite)
            {
                var newValue = AnsiConsole.Ask<string>($"Enter new value for {UserChoice}:");
                property.SetValue(mealToUpdate, newValue);
            }
        }
        AnsiConsole.MarkupLine("[green]Updated and saved successfully![/]");
        AnsiConsole.MarkupLine("[gray]Press any key to continue[/]");
        Console.ReadKey();
        await db.SaveChangesAsync();
    }
    private async Task<MealViewModel> DisplayMeal(List<MealViewModel> meals)
    {
        if (meals == null || meals.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No meals found![/]");
            return null;
        }
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
            Padding = new Padding(0)
        };

        AnsiConsole.Write(panel);
        return meal;
    }  
}