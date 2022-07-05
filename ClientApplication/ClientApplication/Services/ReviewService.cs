using AutoMapper;
using ClientApplication.Abstracts;
using ClientApplication.Models.ViewModels;
using Common;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApplication.Services
{
    public class ReviewService : IReviewService
    {
        private readonly string addReviewUrl;

        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;
        private IMapper mapper;

        public ReviewService(IConfiguration configuration, IMapper mapper, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.clientFactory = clientFactory;

            addReviewUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/review";
        }

        public async Task<bool> AddNewReview(NewReviewViewModel newReview, ISession session)
        {
            NewReviewDTO toAdd = mapper.Map<NewReviewDTO>(newReview);
            var content = new StringContent(
                JsonSerializer.Serialize(toAdd),
                Encoding.UTF8,
                "application/json");

            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            var response = await httpClient.PostAsync(addReviewUrl, content);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }
    }
}
