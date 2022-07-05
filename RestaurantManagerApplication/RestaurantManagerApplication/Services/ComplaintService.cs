using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Models;
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

namespace RestaurantManagerApplication.Services
{
    public class ComplaintService : IComplaintService
    { 
        private readonly string getAllComplaintsUrl;
        private readonly string respondToComplaintUrl;

        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public ComplaintService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;

            respondToComplaintUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/complaint/respond";
            getAllComplaintsUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/complaint/all";
        }

        public async Task<List<ComplaintModel>> GetComplaints(ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllComplaintsUrl);
            if (!response.IsSuccessStatusCode)
                return null;
            return JsonSerializer.Deserialize<List<ComplaintRDTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(c => mapper.Map<ComplaintModel>(c));
        }
        public async Task<bool> RespondToComplaint(int complaintId, string responseToComplaint, ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var content = new StringContent(
                JsonSerializer.Serialize(responseToComplaint),
                System.Text.Encoding.UTF8,
                "application/json");
            var response = await httpClient.PostAsync(respondToComplaintUrl + $@"?id={complaintId}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
