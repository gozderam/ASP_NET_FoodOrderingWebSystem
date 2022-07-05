using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminApplication.Abstracts;
using AdminApplication.Models;
using Microsoft.AspNetCore.Http;
using SharedLib.Security;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Common;
using Common.DTO;
using AutoMapper;
using System.Text;

namespace AdminApplication.Services
{
    public class OrderService : IOrderService
    {
        private readonly string getAllOrdersUrl;

        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public OrderService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;

            getAllOrdersUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/order/archive";
        }

        public async Task<OrderModel[]> GetAllOrders(ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllOrdersUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<List<OrderADTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(o => mapper.Map<OrderModel>(o)).ToArray();
        }
    }
}
