using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationExercise.Core
{
    public class CustomerManager : ICustomerManager
    {
        public bool IsCustomerVisible(string customerName)
        {
            return customerName != "Hidden Joe";
        }
    }
}