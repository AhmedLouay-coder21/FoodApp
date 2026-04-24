using System;
using Spectre.Console;
using FoodApp;

namespace FoodApp
{
    internal class MainMenu
    {
        public static async Task StartProgram()
        {
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
                    HttpClient client = new();
                        var mealService = new MealService(client);
                        await mealService.SearchByName(mealName);
                    break;
            }
        }
    }
}