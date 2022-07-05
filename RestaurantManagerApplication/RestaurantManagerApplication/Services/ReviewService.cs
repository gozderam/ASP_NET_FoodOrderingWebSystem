using AutoMapper;
using Common;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Models;
using SharedLib.Security;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Services
{
    public class ReviewService : IReviewService
    {
        private readonly string getReviewUrl;

        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;
        private IMapper mapper;

        public ReviewService(IConfiguration configuration, IMapper mapper, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration;
            this.clientFactory = clientFactory;
            this.mapper = mapper;

            getReviewUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/review/all";
        }

        public async Task<ReviewViewModel[]> GetReviews(ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getReviewUrl);
            ReviewRDTO[] resReview = null;
            ReviewViewModel[] result = null;

            if (response.IsSuccessStatusCode)
            {
                resReview = JsonSerializer.Deserialize<ReviewRDTO[]>(await response.Content.ReadAsStringAsync());

                int i = 0;
                result = new ReviewViewModel[resReview.Length];
                foreach (var review in resReview)
                {
                    result[i] = mapper.Map<ReviewViewModel>(review);
                    i++;
                }
            }

            return result;
        }
    }
}
