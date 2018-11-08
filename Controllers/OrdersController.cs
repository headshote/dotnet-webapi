﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApplicationExercise.Core;
using WebApplicationExercise.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace WebApplicationExercise.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private MainDataContext _dataContext = new MainDataContext();
        private CustomerManager _customerManager = new CustomerManager();

        [HttpGet]
        [Route("order")]
        public Order GetOrder(Guid orderId)
        {
            return _dataContext.Orders.Include(o => o.Products).Single(o => o.Id == orderId);
        }

        [HttpGet]
        [Route("orders")]
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

        [HttpPost]
        [Route("order")]
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
    }
}