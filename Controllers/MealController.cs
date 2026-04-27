using Spectre.Console;
using FoodApp.Models;
using FoodApp.Services;
using System.Text.RegularExpressions;

namespace FoodApp.Controllers;

public class MealController
{
    private readonly IMealService _mealService;

    public MealController(IMealService mealService)
    {
        _mealService = mealService;
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
    }
}