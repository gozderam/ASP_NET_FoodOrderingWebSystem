using AutoMapper;
using ClientApplication.Abstracts;
using ClientApplication.Models;
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
    public class ComplaintService : IComplaintService
    { 
        private readonly string getAllComplaintsUrl;
        private readonly string postNewComplaintUrl;

        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public ComplaintService(IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;

            getAllComplaintsUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer/complaint/all";
            postNewComplaintUrl = configuration[Definitions.API_URL_CONFIG_KEY] + @"/complaint";
        }

        public async Task<List<ComplaintModel>> GetComplaintsForUser(ISession callingUserSession)
        {
            var httpClient = httpClientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, callingUserSession);
            var response = await httpClient.GetAsync(getAllComplaintsUrl);
            if (!response.IsSuccessStatusCode)
                return null;
            return JsonSerializer.Deserialize<List<ComplaintDTO>>(await response.Content.ReadAsStringAsync())
                .ConvertAll(c => mapper.Map<ComplaintModel>(c));
        }

        public async Task<bool> AddNewComplaint(NewComplaintViewModel newComplaint, ISession session)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            NewComplaintDTO toAdd = mapper.Map<NewComplaintDTO>(newComplaint);

            var content = new StringContent(
                JsonSerializer.Serialize(toAdd),
                Encoding.UTF8,
                "application/json");

            var responsePOST = await httpClient.PostAsync(postNewComplaintUrl, content);

            return responsePOST.IsSuccessStatusCode;
        }

    }
}
