using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace FoodApp
{
    public class MealDb
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Area{ get; set; }
        public string? Image { get; set; }
        public string? Instructions{ get; set; }
        public string? Tags{ get; set; }
        public string? YoutubeLink { get; set; }
        public string? IngredientsJson { get; set; }
    }
}