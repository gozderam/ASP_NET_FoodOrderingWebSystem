using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{
    public interface IOrderRoleExecutor
    {
        Task<IOperationResult<object>> GetOrder(IOrderService service, int orderID);
    }

    public class AdminOrderExecutor : IOrderRoleExecutor
    {
        public async Task<IOperationResult<object>> GetOrder(IOrderService service, int orderID)
            => (await service.GetOrderA(orderID)).GetInstanceWithObjectData();
    }

    public class RestaurateurOrderExecutor : IOrderRoleExecutor
    {
        public async Task<IOperationResult<object>> GetOrder(IOrderService service, int orderID)
            => (await service.GetOrderR(orderID)).GetInstanceWithObjectData();
    }

    public class EmployeeOrderExecutor : IOrderRoleExecutor
    {
        public async Task<IOperationResult<object>> GetOrder(IOrderService service, int orderID)
            => (await service.GetOrderR(orderID)).GetInstanceWithObjectData();
    }

    public class CustomerOrderExecutor : IOrderRoleExecutor
    {
        public async Task<IOperationResult<object>> GetOrder(IOrderService service, int orderID)
            => (await service.GetOrderC(orderID)).GetInstanceWithObjectData();
    }
}
