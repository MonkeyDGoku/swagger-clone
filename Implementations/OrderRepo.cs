using Swagger.Clone.Interfaces;
using Swagger.Clone.Models;

namespace Swagger.Clone.Implementations
{
    public class OrderRepo : IOrder
    {
        public Task<Order> GetOrder()
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                ProductName = "Iphone",
                ProductCategory = "Mobile",
                ProductDateTime = DateTime.Now,
                Address = new Address()
                {
                    Id = Guid.NewGuid(),
                    City = "Bangalore",
                    Country = "India",
                    Phone = "1234",

                },

               Users = new List<User>()
               {
                    new User() { Id = 1, Name = "ANup", Alias = "An", Email = "anup@gmail.com" },
                    new User() { Id = 1, Name = "ANup", Alias = "An", Email = "anup@gmail.com" }
               }

            };

            return Task.FromResult(order);
        }
    }
}
