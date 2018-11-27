using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Core.Models;
using WebApplicationExercise.Infrastructure.Errors.Exceptions;

namespace WebApplicationExercise.Infrastructure.Data
{
    public class OrdersRepository : IOrdersRepository, IDisposable
    {
        private readonly MainDataContext _dataContext;
        private readonly ILogger _logger;
        
        private bool _disposed = false;

        private const int DefaultPageRecordCount = 25;

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

        public Task<List<Order>> List(int? page, int? perPage, DateTime? from = null, DateTime? to = null, 
            string customerName = null, string sortField = null, bool ascending = true)
        {
            var takePage = page ?? 1;
            var takeCount = perPage ?? DefaultPageRecordCount;

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

            orders = sortField != null ?
                OrderByPropertyName(orders, sortField, ascending) : orders.OrderBy(o => o.Id);

            orders = orders.Skip((takePage - 1) * takeCount)
                .Take(takeCount);

            return orders.ToListAsync();
        }

        private IQueryable<Order> OrderByPropertyName(IQueryable<Order> q, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(Order), "p");
            MemberExpression prop;
            try
            {
                prop = Expression.Property(param, sortField);
            }
            catch (ArgumentException)
            {
                throw new BusinessException($"Field '{sortField}' doesn't exist in the Order object");
            }
            
            var exp = Expression.Lambda(prop, param);
            string method = ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var rs = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<Order>(rs);
        }

        public async Task<Order> Add(Order order)
        {
            _dataContext.Orders.Add(order);

            await _dataContext.SaveChangesAsync();

            return order;
        }

        public async Task<bool> Update(Order order)
        {
            _dataContext.UpdateGraph<Order>(order,
                map => map.OwnedCollection(x => x.Products));

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
