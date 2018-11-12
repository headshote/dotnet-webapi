﻿using System;
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
using WebApplicationExercise.Logging;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly MainDataContext _dataContext = new MainDataContext();
        private readonly CustomerManager _customerManager = new CustomerManager();
        private readonly ILogger _logger = new Logger(); 

        // GET: api/Orders/5
        /// <summary>
        /// Returns an Order.
        /// </summary>
        /// <param name="id">Guid of the order</param>
        /// <returns>An Order, which mathecs the id</returns>
        [HttpGet]
        [LoggingExecutionTimeFilter]
        public async Task<IHttpActionResult> GetOrder(Guid id)
        {
            var order = await _dataContext.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/Orders
        /// <summary>
        /// Returns a list of Orders, which match the filtering criteria.
        /// </summary>
        /// <param name="from">A date, starting from which, orders to be returned</param>
        /// <param name="to">A date, up to which, orders to be returned</param>
        /// <param name="customerName">Name of the customer n the order</param>
        /// <returns>a list of Orders, which match the filtering criteria</returns>
        [HttpGet]
        [LoggingExecutionTimeFilter]
        public async Task<IEnumerable<Order>> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            var orders = _dataContext.Orders
                         .Include(o => o.Products);

            if (from != null && to != null)
            {
                orders = FilterByDate(orders, from.Value, to.Value);
            }

            if (customerName != null)
            {
                orders = FilterByCustomer(orders, customerName);
            }

            var ordersList = await orders.ToListAsync();            

            return ordersList.Where(o => _customerManager.IsCustomerVisible(o.Customer));
        }

        // POST: api/Orders/5
        /// <summary>
        /// Creates an Order.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /
        ///     {
        ///         "CreatedDate" : "09/11/2001",
        ///         "Customer" : "BigGuy4U",
        ///         "Products" : [{
        ///             "Name" : "Product911",
        ///             "Price" : 2.55
        ///
        ///         }, {
        ///             "Name" : "Product420",
        ///             "Price" : 1.55
        ///         }, ]
        ///     }
        ///
        /// </remarks>
        /// <param name="order">Order object</param>
        /// <returns>A newly created Order</returns>
        /// <response code="201">Returns the newly created order</response>
        /// <response code="400">Bad request if input model is invalid</response>
        [HttpPost]
        [LoggingExecutionTimeFilter]
        public async Task<IHttpActionResult> SaveOrder([FromBody]Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return Ok(order);
        }

        // PUT: api/Orders/5
        /// <summary>
        /// Modifies the Order.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /7b8890db-70e3-e811-844f-186024db30bb
        ///     {
        ///         "Id" : "7b8890db-70e3-e811-844f-186024db30bb",
        ///         "CreatedDate" : "06/16/2016",
        ///         "Customer" : "NOT REMOTELY SmallGuy4U2",
        ///         "Products" : [{
        ///             "Name" : "Totally NonProduct902",
        ///             "Price" : 12.55
        ///
        ///         }, {
        ///             "Name" : "Totally NonProduct912",
        ///             "Price" : 11.55
        ///         }, ]
        ///     }
        ///
        /// </remarks>
        /// <param name="id">id of the Order object to modify</param>
        /// <param name="order">Order object</param>
        /// <response code="205">No content upon succesful Order update</response>
        /// <response code="404">If the order doen's exist</response>
        /// <response code="400">Bad request if input model is not valid</response>
        [HttpPut]
        [LoggingExecutionTimeFilter]
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

            if (!OrderExists(id))
            {
                return NotFound();
            }

            if (order.Products != null)
            {
                Order orderFromDb = await _dataContext.Orders
                    .Where(o => o.Id == id)
                    .Include(p => p.Products)
                    .FirstAsync();
                orderFromDb.Products.RemoveRange(0, orderFromDb.Products.Count);

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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Orders/5
        /// <summary>
        /// Deletes the Order.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /7b8890db-70e3-e811-844f-186024db30bb
        ///
        /// </remarks>
        /// <param name="id">Order Guid</param>
        /// <returns>The deleted Order</returns>
        /// <response code="201">Returns the deleted order</response>
        /// <response code="404">Not found if the order doen't exist</response>
        [HttpDelete]
        [LoggingExecutionTimeFilter]
        public async Task<IHttpActionResult> DeleteOrder(Guid id)
        {
            Order order = await _dataContext.Orders
                .Where(o => o.Id == id)
                .Include(p => p.Products)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            _dataContext.Orders.Remove(order);
            foreach (var product in order.Products)
            {
                _dataContext.Products.Remove(product);
            }

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.Error(exception, "During DELETE request.");
                throw;
            }

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
    }
}
