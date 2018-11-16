using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Core.Models;

namespace WebApplicationExercise.Infrastructure.Data
{
    public class OrdersRepository : IOrdersRepository, IDisposable
    {
        private readonly MainDataContext _dataContext;
        private readonly ILogger _logger;
        
        private bool _disposed = false;

        public OrdersRepository(MainDataContext dbContext,
            ILogger logger)
        {
            _dataContext = dbContext;
            _logger = logger;
        }

        public Task<Order> GetById(Guid id)
        {
            return _dataContext.Orders.AsNoTracking().Include(o => o.Products).FirstOrDefaultAsync(o => o.Id == id);
        }

        public Task<List<Order>> List(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            var orders = _dataContext.Orders
                .AsNoTracking()
                .Include(o => o.Products);

            if (from != null && to != null)
            {
                orders = FilterByDate(orders, from.Value, to.Value);
            }

            if (customerName != null)
            {
                orders = FilterByCustomer(orders, customerName);
            }

            return orders.ToListAsync();
        }

        public async Task<Order> Add(Order order)
        {
            _dataContext.Orders.Add(order);

            await _dataContext.SaveChangesAsync();

            return order;
        }

        public async Task<bool> Update(Order order)
        {
            Guid id = order.Id;

            Order orderFromDb = await _dataContext.Orders
                .Where(o => o.Id == id)
                .Include(p => p.Products)
                .FirstOrDefaultAsync();

            if (orderFromDb == null)
            {
                return false;
            }

            _dataContext.Entry(orderFromDb).CurrentValues.SetValues(order);
            if (order.Products != null)
            {
                _dataContext.Products.RemoveRange(orderFromDb.Products);
                orderFromDb.Products.AddRange(order.Products);
            }
            _dataContext.Set<Order>().AddOrUpdate(orderFromDb);

            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<Order> Delete(Guid id)
        {
            Order order = await _dataContext.Orders
                .Where(o => o.Id == id)
                .Include(p => p.Products)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            _dataContext.Set<Order>().Remove(order);

            await _dataContext.SaveChangesAsync();

            return order;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IQueryable<Order> FilterByCustomer(IQueryable<Order> orders, string customerName)
        {
            return orders.Where(o => o.Customer == customerName);
        }

        private IQueryable<Order> FilterByDate(IQueryable<Order> orders, DateTime from, DateTime to)
        {
            return orders.Where(o => o.CreatedDate >= from && o.CreatedDate < to);
        }

        ~OrdersRepository()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dataContext.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
