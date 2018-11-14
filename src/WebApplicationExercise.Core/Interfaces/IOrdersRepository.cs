﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Models;

namespace WebApplicationExercise.Core.Interfaces
{
    public interface IOrdersRepository
    {
        Task<Order> GetById(Guid id);
        Task<List<Order>> List(DateTime? from = null, DateTime? to = null, string customerName = null);
        Task<Order> Add(Order order);
        Task<bool> Update(Order order);
        Task<Order> Delete(Guid id);
    }
}
