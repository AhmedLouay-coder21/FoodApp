using System.Net.Http.Json;
using System.Text.RegularExpressions;
using FoodApp.Models;
using FoodApp.Services;
using Spectre.Console;
namespace FoodApp;

public class MealService : IMealService
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
        var response = await _httpClient
            .GetFromJsonAsync<MealWrapper>($"search.php?s={name}");
        var meals = response?.meals ?? new List<Meal>();

        return response?.meals ?? new List<Meal>();
    }
}