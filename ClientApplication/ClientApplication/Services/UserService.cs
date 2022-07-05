using AutoMapper;
using ClientApplication.Abstracts;
using ClientApplication.Models.ViewModels;
using Common;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApplication.Services
{
    public class UserService: IUserService
    {
        private readonly string addUserUrl;
        private readonly string getAllOrdersUrl;

        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IHttpClientFactory clientFactory;

        public UserService(IConfiguration configuration, IMapper mapper, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.clientFactory = clientFactory;

            getAllOrdersUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer/order/all";
            addUserUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer";
        }
        public async Task<bool> AddNewUser(ISession session, NewCustomerViewModel newCustomer)
        {
            NewCustomerDTO toAdd = mapper.Map<NewCustomerDTO>(newCustomer);
            var content = new StringContent(
                JsonSerializer.Serialize(toAdd),
                Encoding.UTF8,
                "application/json");

            var httpClient = clientFactory.CreateClient();

            var response = await httpClient.PostAsync(addUserUrl, content);
            if (response.IsSuccessStatusCode)
                return true;
            else
            {
                return false;
            }

        }

        public async Task<OrderViewModel[]> GetAllOrders(ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getAllOrdersUrl);
            List<OrderCDTO> orders = null;
            if (response.IsSuccessStatusCode)
            {
                orders = JsonSerializer.Deserialize<List<OrderCDTO>>(await response.Content.ReadAsStringAsync());
            }

            return orders.ConvertAll(o => mapper.Map<OrderViewModel>(o)).ToArray();
        }
    }
}
