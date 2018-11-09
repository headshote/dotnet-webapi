using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApplicationExercise.Core;
using WebApplicationExercise.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Net;
using System.Threading.Tasks;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private MainDataContext _dataContext = new MainDataContext();
        private CustomerManager _customerManager = new CustomerManager();

        // GET: api/Orders/5
        [HttpGet]
        public Order GetOrder(Guid id)
        {
            return _dataContext.Orders.Include(o => o.Products).Single(o => o.Id == id);
        }

        // GET: api/Orders
        [HttpGet]
        public IEnumerable<Order> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            IEnumerable<Order> orders = _dataContext.Orders.Include(o => o.Products);

            if (from != null && to != null)
            {
                orders = FilterByDate(orders, from.Value, to.Value);
            }

            if (customerName != null)
            {
                orders = FilterByCustomer(orders, customerName);
            }

            return orders.Where(o => _customerManager.IsCustomerVisible(o.Customer));
        }

        // POST: api/Orders/5
        [HttpPost]
        public async Task<IHttpActionResult> SaveOrder([FromBody]Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync();

            return Ok(order);
        }

        // PUT: api/Orders/5
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrder(Guid id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }
            
            _dataContext.Set<Order>().AddOrUpdate(order);

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Orders/5
        public async Task<IHttpActionResult> DeleteOrder(Guid id)
        {
            Order order = await _dataContext.Orders
                .Where(x => x.Id == id)
                .Include(s => s.Products)
                .FirstAsync();

            if (order == null)
            {
                return NotFound();
            }

            _dataContext.Orders.Remove(order);
            foreach (var s in order.Products.ToList())
            {
                _dataContext.Products.Remove(s);
            }

            await _dataContext.SaveChangesAsync();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private IEnumerable<Order> FilterByCustomer(IEnumerable<Order> orders, string customerName)
        {
            return orders.Where(o => o.Customer == customerName);
        }

        private IEnumerable<Order> FilterByDate(IEnumerable<Order> orders, DateTime from, DateTime to)
        {
            return orders.Where(o => o.CreatedDate >= from && o.CreatedDate < to);
        }

        private bool OrderExists(Guid id)
        {
            return _dataContext.Orders.Count(e => e.Id == id) > 0;
        }
    }
}
