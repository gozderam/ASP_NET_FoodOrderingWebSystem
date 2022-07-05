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
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly string getAllDiscountCodesUrl;
        private readonly string deleteDiscountCodeUrl;
        private readonly string addNewDiscountCodeUrl;

        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public DiscountCodeService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;

            getAllDiscountCodesUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/discountcode/all";
            deleteDiscountCodeUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/discountcode";
            addNewDiscountCodeUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/discountcode";
        }

        public async Task<DiscountCodeModel[]> GetAllDiscountCodes(ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllDiscountCodesUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<List<DiscountCodeDTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(d => mapper.Map<DiscountCodeModel>(d)).ToArray();
        }

        public async Task<bool> DeleteDiscountCode(int id, ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.DeleteAsync(deleteDiscountCodeUrl + "?" + $"id={id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddNewDiscountCode(NewDiscountCodeModel newDiscountCodeModel, ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var content = new StringContent(
                JsonSerializer.Serialize(mapper.Map<NewDiscountCodeDTO>(newDiscountCodeModel)),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(addNewDiscountCodeUrl, content);

            return response.IsSuccessStatusCode;
        }
    }
}
