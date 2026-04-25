using System.Text.RegularExpressions;
using FoodApp.Models;
using Spectre.Console;
namespace FoodApp;

public class MealService
{
    private readonly HttpClient _httpClient;

    public MealService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Repository Reporter");
    }

    public async Task<List<Meal>> SearchByName(string name)
    {
        var response = await _httpClient.GetFromJsonAsync<MealWrapper>($"search.php?s={name}");
        var meals = response?.meals ?? new List<Meal>();
        if(!meals.Any())
        {
            AnsiConsole.MarkupLine("[red]No meals found![/]");
            return response?.meals ?? new List<Meal>();
        }
        var prompt = new SelectionPrompt<string>()
        .Title("Select a [OrangeRed1]meal[/] by category")
        .PageSize(15)
        .MoreChoicesText("[grey](Move up and down to reveal more meals)[/]");

        var groupedMeals = meals
        .GroupBy(m => m.strCategory)
        .OrderBy(g => g.Key);
        foreach(var category in groupedMeals)
        {
                prompt.AddChoiceGroup(category.Key, category.Select(m => m.strMeal));
                // AnsiConsole.Status() // disable to test purposes
                // .Start("Processing...", ctx =>
                // {
                //     Thread.Sleep(2500);
                // });
        }
        AnsiConsole.MarkupLine("[green]Done![/]");
        var selectedMealName = AnsiConsole.Prompt(prompt);

        return response?.meals ?? new List<Meal>();
    }
}