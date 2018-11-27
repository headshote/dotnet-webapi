using System;

namespace WebApplicationExercise.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal PriceUSD { get; set; }

        public Order Order { get; set; }
    }
}