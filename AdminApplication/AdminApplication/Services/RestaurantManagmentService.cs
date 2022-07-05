using AdminApplication.Abstracts;
using AdminApplication.Models;
using Common.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static AdminApplication.Models.RestaurantDataModel;

namespace AdminApplication.Services
{
    public class RestaurantManagmentService : IRestaurantManagmentService
    {
        private readonly string getAllRestaurantsUrl;
        private readonly string changeRestaurantStateUrl;

        private readonly IConfiguration Configuration;

        public RestaurantManagmentService(IConfiguration configuration)
        {
            Configuration = configuration;
            getAllRestaurantsUrl = configuration["ApiURL"] + @"/restaurant/all";
            changeRestaurantStateUrl = configuration["ApiURL"] + @"/restaurant";
        }

        public async Task<RestaurantDTO[]> GetAllRestaurants(HttpClient httpClient)
        {
            HttpResponseMessage response = await httpClient.GetAsync(getAllRestaurantsUrl);
            RestaurantDTO[] restaurantData = null;
            if (response.IsSuccessStatusCode)
            {
               restaurantData = JsonSerializer.Deserialize<RestaurantDTO[]>(await response.Content.ReadAsStringAsync());
            }
            return restaurantData;
        }

        public async Task<bool> DeleteRestaurant(HttpClient httpClient, int id)
        {
            string url = changeRestaurantStateUrl + "?" + $"id={id}";

            HttpResponseMessage response = await httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;
            return true;
        }

        public async Task<bool> RestaurantActivate(HttpClient httpClient, int id)
        {
            string url = changeRestaurantStateUrl + "/activate" + "?" + $"id={id}";

            var content = new StringContent(
                JsonSerializer.Serialize(id),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(url,content);
            if (!response.IsSuccessStatusCode)
                return false;
            return true;
        }
        public async Task<bool> RestaurantBlock(HttpClient httpClient, int id)
        {
            string url = changeRestaurantStateUrl + "/block" + "?" + $"id={id}";

            var content = new StringContent(
                JsonSerializer.Serialize(id),
                Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
                return false;
            return true;
        }

        public async Task<bool> RestaurantUnblock(HttpClient httpClient, int id)
        {
            string url = changeRestaurantStateUrl + "/unblock" + "?" + $"id={id}";

            var content = new StringContent(
                JsonSerializer.Serialize(id),
                Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
                return false;
            return true;
        }

    }
}
