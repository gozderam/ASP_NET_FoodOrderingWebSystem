using AdminApplication.Abstracts;
using AdminApplication.Models;
using AutoMapper;
using Common;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminApplication.Services
{
    public class ComplaintService : IComplaintService
    { 
        private readonly string getAllComplaintsForUserUrl;
        private readonly string getAllComplaintsForRestaurantUrl;

        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public ComplaintService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;

            getAllComplaintsForUserUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer/complaint/all";
            getAllComplaintsForRestaurantUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant​/complaint​/all";
        }

        public async Task<List<ComplaintModel>> GetComplaintsForUser(int userId, ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllComplaintsForUserUrl + @$" ? id={userId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<List<ComplaintDTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(c => mapper.Map<ComplaintModel>(c));
        }

        public async Task<List<ComplaintModel>> GetComplaintsForRestaurant(int restaurantId, ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllComplaintsForRestaurantUrl + @$"?id={restaurantId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<List<ComplaintDTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(c => mapper.Map<ComplaintModel>(c));
        }

    }
}
