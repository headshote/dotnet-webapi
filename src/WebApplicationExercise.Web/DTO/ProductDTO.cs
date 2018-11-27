using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplicationExercise.Web.DTO
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Price of the product, in USD, unless currency is specified
        /// </summary>
        [Range(0, 100000, ErrorMessage = "Price must be between $0 and $100,000")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}