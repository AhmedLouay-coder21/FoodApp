using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using FoodApp;

namespace FoodApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var MainMenu = new MainMenu();
            while(true)
            {
                await MainMenu.StartProgram();
            }
        }
    }
}