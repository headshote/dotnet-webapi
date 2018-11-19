using System;
using System.Collections.Generic;

namespace WebApplicationExercise.Core.Models
{
    /// <summary>
    /// Purchase order
    /// </summary>
    public class Order
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Date of the order creation
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Customer's name
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// List of products, purchase by the customer in this order
        /// </summary>
        public List<Product> Products { get; set; }
    }
}