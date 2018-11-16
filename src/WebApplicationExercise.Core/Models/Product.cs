using System;

namespace WebApplicationExercise.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public Order Order { get; set; }
    }
}