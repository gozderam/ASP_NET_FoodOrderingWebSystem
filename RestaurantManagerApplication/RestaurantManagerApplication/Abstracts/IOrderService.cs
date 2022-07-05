using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using Microsoft.AspNetCore.Http;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IOrderService
    {
        Task<OrderModel[]> GetAllUnrealisedOrders(ISession session);
        Task<OrderModel[]> GetAllMyPendingOrders(ISession session);
        Task<OrderModel> GetOrder(int id, ISession session);
        Task<bool> RefuseOrder(int id, ISession session);
        Task<bool> AcceptOrder(int id, ISession session);
        Task<bool> MarkOrderRealised(int id, ISession session);

        Task<RestaurantStatsModel> GetAllOrdersArchive(ISession session);
    }
}
