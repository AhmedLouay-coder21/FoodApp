using System;
using Spectre.Console;
using FoodApp;
using FoodApp.Controllers;
using FoodApp.Services;
using FoodApp.Data;

namespace FoodApp
{
    public class MainMenu
    {
        public static async Task StartProgram()
        {
            HttpClient client = new();
            var db = new MealDbContext();
            var mealService = new MealService(client);
            var mealRepository = new MealRepository(db);
            var mealController = new MealController(mealService,mealRepository);
            //App name
            var figlet = new FigletText("Food App")
            {
                Justification = Justify.Center,
                Color = Color.OrangeRed1
            };
            AnsiConsole.Write(figlet);

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an [OrangeRed1]option[/]:")
                .AddChoices("Search for a new recipe", "View favorite recipes", "Edit a recipes"));
        
            switch (choice)
            {
                case "Search for a new recipe":
                    var mealName = AnsiConsole.Ask<string>("What [OrangeRed1]meal[/] do you wanna discover?");
                    Console.Clear();
                        await mealController.SearchByName(mealName);
                    break;
                case "View favorite recipes":
                    await mealController.GetMeal(db);
                    break;
            }
        }
    }
}