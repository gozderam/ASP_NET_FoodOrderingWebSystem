using Microsoft.Extensions.Configuration;
using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using  Common;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLib.Security;
using Common.DTO;

namespace RestaurantManagerApplication.Services
{
    public class OrderService :IOrderService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        private readonly string BASE_URL;
        private readonly string ALL_ORDERS_URL = @"/restaurant/order/all";
        private readonly string ALL_ORDERS_ARCHIVE_URL = @"/restaurant/order/archive";
        private readonly string ORDER_URL = @"/order";
        private readonly string REFUSE_ORDER_URL = @"/order/refuse";
        private readonly string ACCEPT_ORDER_URL = @"/order/accept";
        private readonly string REALIZE_ORDER_URL = @"/order/realized";

        public OrderService(IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.mapper = mapper;
            BASE_URL = this.configuration[Definitions.API_URL_CONFIG_KEY];
        }
        public async Task<OrderModel[]> GetAllMyPendingOrders(ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + ALL_ORDERS_URL);

            if (response.IsSuccessStatusCode)
            {               
                var orders = JsonSerializer.Deserialize<OrderRDTO[]>(await response.Content.ReadAsStringAsync());//.ToList().ConvertAll(o => mapper.Map<OrderModel>(o));
                
                var pendingOrders = new List<OrderModel>();
                foreach (OrderRDTO order in orders)
                {                    
                    if (order.discountcode == null) order.discountcode = new DiscountCodeDTO();
                    if (order.state == OrderStateDTO.pending && Authentication.GetLoggedUserId(session) == order.employee.id)
                    {
                        OrderModel pendingOrder = mapper.Map<OrderModel>(order);
                        pendingOrders.Add(pendingOrder); 
                    }
                }
                return pendingOrders.ToArray();
            }
            return null;
        }
        public async Task<OrderModel[]> GetAllUnrealisedOrders(ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + ALL_ORDERS_URL);

            if (response.IsSuccessStatusCode)
            {
                var orders = JsonSerializer.Deserialize<OrderRDTO[]>(await response.Content.ReadAsStringAsync());//.ToList().ConvertAll(o=>mapper.Map<OrderModel>(o));
                var unrealisedOrders = new List<OrderModel>();
                foreach(OrderRDTO order in orders)
                {
                    if (order.discountcode == null) order.discountcode = new DiscountCodeDTO();
                    if (order.employee == null) order.employee = new EmployeeDTO();
                    if (order.state == OrderStateDTO.unrealized) 
                    {
                        OrderModel unrealisedOrder = mapper.Map<OrderModel>(order);
                        unrealisedOrders.Add(unrealisedOrder); 
                    }
                }
                return unrealisedOrders.ToArray();
            }
            return null;
        }
        public async Task<OrderModel> GetOrder(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + ORDER_URL + $@"?id={id}");
            OrderRDTO order;
            if (response.IsSuccessStatusCode)
            {
                order = JsonSerializer.Deserialize<OrderRDTO>(await response.Content.ReadAsStringAsync());
                return mapper.Map<OrderModel>(order);
            }
            return null;
        }
        public async Task<bool> RefuseOrder(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.PostAsync(BASE_URL + REFUSE_ORDER_URL + $@"?id={id}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> AcceptOrder(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.PostAsync(BASE_URL + ACCEPT_ORDER_URL + $@"?id={id}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> MarkOrderRealised(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.PostAsync(BASE_URL + REALIZE_ORDER_URL + $@"?id={id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<RestaurantStatsModel> GetAllOrdersArchive(ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + ALL_ORDERS_ARCHIVE_URL);

            if (response.IsSuccessStatusCode)
            {
                var orders = JsonSerializer.Deserialize<OrderRDTO[]>(await response.Content.ReadAsStringAsync()).ToList().ConvertAll(o => mapper.Map<OrderModel>(o)).ToArray();
                var ret = new RestaurantStatsModel(orders);
                return ret;
            }
            return null;
        }
    }
}
