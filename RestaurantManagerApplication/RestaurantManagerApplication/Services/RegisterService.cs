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
using AutoMapper;
using Common.DTO;
using static Common.Definitions;
using SharedLib.Security;
using Microsoft.AspNetCore.Http;

namespace RestaurantManagerApplication.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        private readonly string EMPLOYEE_URL = @"/user/employee";

        public RegisterService(IHttpClientFactory clientFactory, IMapper mapper, IConfiguration configuration)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<bool> RegisterRestaurateur(NewRestaurantEmployeeModel newRestaurateur)
        {
            var httpClient = clientFactory.CreateClient();

            if (newRestaurateur.Name == null || newRestaurateur.Name == string.Empty || newRestaurateur.Surname == null
                || newRestaurateur.Surname == string.Empty || newRestaurateur.Email == null || newRestaurateur.Email == string.Empty) return false; 

            var content = new StringContent(
                   JsonSerializer.Serialize(mapper.Map<NewEmployeeDTO>(newRestaurateur)),
                   Encoding.UTF8,
                   "application/json");
            var response = await httpClient.PostAsync(configuration[API_URL_CONFIG_KEY] + EMPLOYEE_URL, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterEmployee(NewRestaurantEmployeeModel newEmployee, ISession callingUserSession)
        {
            var httpClient = clientFactory.CreateClient();

            if (newEmployee.Name == null || newEmployee.Name == string.Empty || newEmployee.Surname == null
                || newEmployee.Surname == string.Empty || newEmployee.Email == null || newEmployee.Email == string.Empty) return false;

            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(configuration[API_URL_CONFIG_KEY] + EMPLOYEE_URL + "?" + $"id={Authentication.GetLoggedUserId(callingUserSession)}");

            if (!response.IsSuccessStatusCode)
                return false;

            newEmployee.RestaurantId = JsonSerializer.Deserialize<EmployeeDTO>(await response.Content.ReadAsStringAsync()).restaurant.id;

            var content = new StringContent(
                   JsonSerializer.Serialize(mapper.Map<NewEmployeeDTO>(newEmployee)),
                   Encoding.UTF8,
                   "application/json");
            response = await httpClient.PostAsync(configuration[API_URL_CONFIG_KEY] + EMPLOYEE_URL, content);

            return response.IsSuccessStatusCode;
        }
    }
}
