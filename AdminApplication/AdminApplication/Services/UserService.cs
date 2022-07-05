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
using AutoMapper;
using Common;
using Common.DTO;
using System.Text;

namespace AdminApplication.Services
{
    public class UserService : IUserService
    {
        private readonly string getAllClientsUrl;
        private readonly string deleteUserUrl;
        private readonly string addNewAdminUrl;

        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public UserService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;

            getAllClientsUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer/all";
            deleteUserUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer";
            addNewAdminUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/admin";
        }
        public async Task<UserDataModel[]> GetAllClients(ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllClientsUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<List<CustomerADTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(c => mapper.Map<UserDataModel>(c)).ToArray();
        }

        public async Task<bool> DeleteClient(int id, ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.DeleteAsync(deleteUserUrl + "?" + $"id={id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddNewAdmin(NewAdminModel newAdminModel)
        {
            var httpClient = httpClientFactory.CreateClient();
            var content = new StringContent(
                JsonSerializer.Serialize(mapper.Map<NewAdministratorDTO>(newAdminModel)),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(addNewAdminUrl, content);

            return response.IsSuccessStatusCode;
        }
    }
}
