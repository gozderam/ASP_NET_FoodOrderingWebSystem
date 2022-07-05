using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApplication.Abstracts
{
    public interface IUserService
    {
        public Task<OrderViewModel[]> GetAllOrders(ISession session);
        
        public Task<bool> AddNewUser(ISession session, NewCustomerViewModel newCustomer);
    }
}
