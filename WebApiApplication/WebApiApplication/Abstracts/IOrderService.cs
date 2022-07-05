using Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Abstracts
{
    public interface IOrderService
    {
        Task<IOperationResult<int?>> AddOrder(NewOrderDTO newOrder);
        Task<IOperationResult<OrderRDTO[]>> GetAllOrders(int restaurantID);
        Task<IOperationResult<OrderADTO>> GetOrderA(int orderID);
        Task<IOperationResult<OrderCDTO>> GetOrderC(int orderID);
        Task<IOperationResult<OrderRDTO>> GetOrderR(int orderID);
        Task<IOperationResult<int>> RefuseOrder(int orderID);
        Task<IOperationResult<int>> AcceptOrder(int orderID, int emoployeeID);
        Task<IOperationResult<int>> RealizeOrder(int orderID);
        Task<IOperationResult<OrderADTO[]>> GetAllOrdersAdmin(int adminId);
    }
}
