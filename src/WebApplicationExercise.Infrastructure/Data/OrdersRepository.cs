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

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.Error(exception, "During POST request.");
                throw;
            }

            return order;
        }

        public async Task<bool> Update(Order order)
        {
            if (!OrderExists(order.Id))
            {
                return false;
            }

            if (order.Products != null)
            {
                Guid id = order.Id;

                Order orderFromDb = await _dataContext.Orders
                    .Where(o => o.Id == id)
                    .Include(p => p.Products)
                    .FirstAsync();
                _dataContext.Products.RemoveRange(orderFromDb.Products);

                foreach (var p in order.Products)
                {
                    orderFromDb.Products.Add(p);
                }
                orderFromDb.CreatedDate = order.CreatedDate;
                orderFromDb.Customer = order.Customer;

                order = orderFromDb;
            }

            _dataContext.Set<Order>().AddOrUpdate(order);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.Error(exception, "During PUT request.");
                throw;
            }

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

            _dataContext.Products.RemoveRange(order.Products);
            _dataContext.Orders.Remove(order);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.Error(exception, "During DELETE request.");
                throw;
            }

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

        private bool OrderExists(Guid id)
        {
            return _dataContext.Orders.Count(e => e.Id == id) > 0;
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
