using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationExercise.Core
{
    public interface ICustomerManager
    {
        bool IsCustomerVisible(string customerName);
    }
}
