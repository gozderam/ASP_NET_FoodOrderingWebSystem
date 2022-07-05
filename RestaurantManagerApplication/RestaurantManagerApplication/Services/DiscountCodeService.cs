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
using AutoMapper;
using Common.DTO;
using Common;
using Microsoft.AspNetCore.Http;
using SharedLib.Security;

namespace RestaurantManagerApplication.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private IMapper mapper;

        private readonly string BASE_URL;
        private readonly string GET_ALL_CODES_URL = @"/discountcode/all";
        private readonly string CODE_URL = @"/discountcode";



        public DiscountCodeService(IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.mapper = mapper;
            BASE_URL = this.configuration[Definitions.API_URL_CONFIG_KEY];
        }
        public async Task<DiscountCodeModel[]> GetAllDiscountCodes(ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);

            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + GET_ALL_CODES_URL);
            DiscountCodeDTO[] codes = null;
            if (response.IsSuccessStatusCode)
            {
                codes = JsonSerializer.Deserialize<DiscountCodeDTO[]>(await response.Content.ReadAsStringAsync());
            }
            if (codes == null) return new DiscountCodeModel[0];
            return codes.ToList().ConvertAll(c=>mapper.Map<DiscountCodeModel>(c)).ToArray();
        } 
        public async Task<bool> AddDiscountCode(DiscountCodeModel dCode, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);

            var content = new StringContent(
                JsonSerializer.Serialize(mapper.Map<NewDiscountCodeDTO> (dCode)),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(BASE_URL + CODE_URL, content);
            return response.IsSuccessStatusCode;
        }
        public async Task<DiscountCodeModel> GetDiscountCode(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + CODE_URL + $@"?id={id}");
            DiscountCodeModel code = null;
            if (response.IsSuccessStatusCode)
            {
                code = JsonSerializer.Deserialize<DiscountCodeModel>(await response.Content.ReadAsStringAsync());
            }
            return code;
        }
        public async Task<bool> DeleteDiscountCode(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.DeleteAsync(BASE_URL + CODE_URL + $@"?id={id}");
            return response.IsSuccessStatusCode; 
        }
    }
}
