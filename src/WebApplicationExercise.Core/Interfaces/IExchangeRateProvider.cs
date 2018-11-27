using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationExercise.Core.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<decimal> GetExchangeRate(string from, string to);
    }
}
