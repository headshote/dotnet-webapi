using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using AutoMapper;
using Microsoft.Web.Http;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Core.Models;
using WebApplicationExercise.Web.DTO;
using WebApplicationExercise.Web.Filters;

namespace WebApplicationExercise.Web.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/orders")]
    [LoggingExecutionTimeFilter]
    [ControllerExceptionFilterAttribute]
    public class OrdersController : ApiController
    {
        private readonly ICustomerManager _customerManager;
        private readonly IOrdersRepository _ordersRepository;

        public OrdersController(ICustomerManager customerManager,
            IOrdersRepository ordersRepository)
        {
            _customerManager = customerManager;
            _ordersRepository = ordersRepository;
        }

        // GET: api/Orders/5
        /// <summary>
        /// Returns an Order with UTC creation date, by its id.
        /// </summary>
        /// <param name="id">Guid of the order</param>
        /// <returns>An Order, which mathecs the id, with UTC creation date</returns>
        [HttpGet]
        [ResponseType(typeof(OrderDTO))]
        public async Task<IHttpActionResult> GetOrder(Guid id)
        {
            var order = await _ordersRepository.GetById(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<OrderDTO>(order));
        }

        // GET: api/Orders
        /// <summary>
        /// Returns a list of Orders with UTC creation dates, which match the filtering criteria.
        /// </summary>
        /// <param name="from">A UTC date, starting from which, orders to be returned</param>
        /// <param name="to">A UTC date, up to which, orders to be returned</param>
        /// <param name="customerName">Name of the customer in the order</param>
        /// <returns>a list of Orders, which match the filtering criteria</returns>
        [HttpGet]
        [ResponseType(typeof(List<OrderDTO>))]
        [InputDateToUtcConversionFilter]
        public async Task<IHttpActionResult> GetOrders(DateTime? from = null, DateTime? to = null, string customerName = null)
        {
            var ordersList = await _ordersRepository.List(from, to, customerName);

            ordersList = ordersList.Where(o => _customerManager.IsCustomerVisible(o.Customer)).ToList();

            return Ok(Mapper.Map<List<Order>, List<OrderDTO>>(ordersList));
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
        /// <param name="order">Order object, CreatedDate is in your local time</param>
        /// <returns>A newly created Order</returns>
        /// <response code="201">Returns the newly created order</response>
        /// <response code="400">Bad request if input model is invalid</response>
        [HttpPost]
        [ResponseType(typeof(OrderDTO))]
        public async Task<IHttpActionResult> SaveOrder([FromBody]OrderDTO order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(Mapper.Map<OrderDTO>(await _ordersRepository.Add(Mapper.Map<Order>(order))));
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
        /// <param name="order">Order object, CreatedDate is in your local time</param>
        /// <response code="205">No content upon succesful Order update</response>
        /// <response code="404">If the order doen's exist</response>
        /// <response code="400">Bad request if input model is not valid</response>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrder(Guid id, OrderDTO order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (id != order.Id)
            {
                return BadRequest();
            }

            if (!await _ordersRepository.Update(Mapper.Map<Order>(order)))
            {
                return NotFound();
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
        [ResponseType(typeof(OrderDTO))]
        public async Task<IHttpActionResult> DeleteOrder(Guid id)
        {
            var order = await _ordersRepository.Delete(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<OrderDTO>(order));
        }
    }
}
