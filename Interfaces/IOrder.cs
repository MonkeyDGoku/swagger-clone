using System.Threading.Tasks;
using Swagger.Clone.Models;


namespace Swagger.Clone.Interfaces
{
    public interface IOrder
    {
        Task<Order> GetOrder();
    }
}
