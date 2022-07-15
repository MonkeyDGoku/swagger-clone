using Microsoft.AspNetCore.Mvc;
using Swagger.Clone.Interfaces;
using Swagger.Clone.Models;

namespace Swagger.Clone.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrder _order;
        public OrderController(IOrder order)
        {
            _order = order;
        }



        [HttpGet]
        public async Task<ActionResult<List<List<Order>>>> GetOrder()
        {
            List<List<Order>> orderList = new List<List<Order>>();
            var order = await _order.GetOrder();
            orderList.Add(new List<Order> { order });

            return await Task.FromResult(orderList);  

        }
    }
}
