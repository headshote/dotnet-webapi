using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationExercise.Web.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Date of the order creation
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Customer's name
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// List of products, purchase by the customer in this order
        /// </summary>
        public List<ProductDTO> Products { get; set; }
    }
}