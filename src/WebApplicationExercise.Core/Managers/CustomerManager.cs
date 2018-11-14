using WebApplicationExercise.Core.Interfaces;

namespace WebApplicationExercise.Core.Managers
{
    public class CustomerManager : ICustomerManager
    {
        public bool IsCustomerVisible(string customerName)
        {
            return customerName != "Hidden Joe";
        }
    }
}