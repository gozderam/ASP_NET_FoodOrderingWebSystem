using Microsoft.AspNetCore.Http;
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
using static Common.Definitions;
using SharedLib.Security;
using Common.DTO;
using Common;
using AutoMapper;

namespace RestaurantManagerApplication.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        private readonly string ADD_RESTAURANT_URL = @"/restaurant";
        private readonly string getEmployeeUrl;
        private readonly string deleteRestaurantUrl;
        private readonly string deactivateRestaurantUrl;
        private readonly string reactivateRestaurantUrl;

        public RestaurantService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.clientFactory = httpClientFactory;
            this.mapper = mapper;

            getEmployeeUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/employee";
            deleteRestaurantUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant";
            deactivateRestaurantUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/deactivate";
            reactivateRestaurantUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/reactivate";
        }

        public async Task<(bool success, int restaurantId)> AddNewRestaurant(NewRestaurantModel newRestaurantModel, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            var content = new StringContent(
                   JsonSerializer.Serialize(mapper.Map<NewRestaurantDTO>(newRestaurantModel)),
                   Encoding.UTF8,
                   "application/json");

            var response = await httpClient.PostAsync(configuration[API_URL_CONFIG_KEY] + ADD_RESTAURANT_URL, content);

            if (!response.IsSuccessStatusCode)
                return (false, -1);

            return (true, int.Parse(await response.Content.ReadAsStringAsync()));
        }

        public async Task<EmployeeStatusModel> GetEmployeeShortInfo(int id, ISession callingUserSession)
        {
            var httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getEmployeeUrl + "?" + $"id={id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return mapper.Map<EmployeeStatusModel>(JsonSerializer.Deserialize<EmployeeDTO>(await response.Content.ReadAsStringAsync()));
        }

        public async Task<bool> DeleteRestaurant(int id, ISession callingUserSession)
        {
            var httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.DeleteAsync(deleteRestaurantUrl + @$"?id={id}");

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeactivateRestaurant(ISession callingUserSession)
        {
            var httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.PostAsync(deactivateRestaurantUrl, null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReactivateRestaurant(ISession callingUserSession)
        {
            var httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.PostAsync(reactivateRestaurantUrl, null);

            return response.IsSuccessStatusCode;
        }
    }
}
