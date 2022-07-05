using AutoMapper;
using ClientApplication.Abstracts;
using ClientApplication.Models.ViewModels;
using Common;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApplication.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly string getAllRestaurantsUrl;
        private readonly string getRestaurantUrl;
        private readonly string getRestaurantMenuSectionsUrl;
        private readonly string getCustomerUrl;
        private readonly string getDiscountCodeUrl;
        private readonly string postNewOrderUrl;
        private readonly string postAddToFavouritesUrl;
        private readonly string getRestaurantReviewsUrl;

        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IHttpClientFactory clientFactory;

        public RestaurantService(IConfiguration configuration, IMapper mapper, IHttpClientFactory clientFactory)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.clientFactory = clientFactory;

            getDiscountCodeUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/discountcode";
            getAllRestaurantsUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/all";
            getRestaurantUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant";
            getRestaurantMenuSectionsUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/menu/";
            getRestaurantReviewsUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/review/all";
            getCustomerUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/user/customer";
            postNewOrderUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/order";
            postAddToFavouritesUrl = this.configuration[Definitions.API_URL_CONFIG_KEY] + @"/restaurant/favourite";
        }
        public async Task<RestaurantViewModel[]> GetAllRestaurants(ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getAllRestaurantsUrl);
            if (response.IsSuccessStatusCode)
            {
                var tmp = response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RestaurantCDTO>>(await response.Content.ReadAsStringAsync())
                    .ConvertAll(r => mapper.Map<RestaurantViewModel>(r)).ToArray();
            }
            return null;
        }

        public async Task<RestaurantMenuViewModel> GetRestaurantMenu(int id, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getRestaurantUrl + "?id=" + $"{id}");
            RestaurantViewModel resRestaurant = null;
            if (response.IsSuccessStatusCode)
            {
                resRestaurant = mapper.Map<RestaurantViewModel>(JsonSerializer.Deserialize<RestaurantCDTO>(await response.Content.ReadAsStringAsync()));
            }

            response = await httpClient.GetAsync(getRestaurantMenuSectionsUrl + "?id=" + $"{id}");
            MenuSectionViewModel[] resSections = null;
            if (response.IsSuccessStatusCode)
            {
                resSections = JsonSerializer.Deserialize<MenuSectionViewModel[]>(await response.Content.ReadAsStringAsync());
            }

            return new RestaurantMenuViewModel() { restaurant = resRestaurant, sections = resSections };
        }
        public async Task<bool> PostNewOrder(int clientID, NewOrderViewModel newOrder, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getCustomerUrl + "?id=" + $"{clientID}");
            CustomerADTO customer = null;
            if (response.IsSuccessStatusCode)
            {
                customer = JsonSerializer.Deserialize<CustomerADTO>(await response.Content.ReadAsStringAsync());
            }

            NewOrderDTO toAdd = mapper.Map<NewOrderDTO>(newOrder);
            toAdd.date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            toAdd.customerId = customer.id;

            if (newOrder.address == null)
                toAdd.address = customer.address;
            else
                toAdd.address = mapper.Map<AddressDTO>(newOrder.address);

            toAdd.discountcodeId = await GetDiscountCodeId(newOrder.discountcodeId, session);

            List<int> positions = new List<int>();
            for (int i = 0; i < newOrder.quantities.Length; i++)
            {
                for (int j = 0; j < newOrder.quantities[i]; j++)
                    positions.Add(newOrder.positions[i]);
            }

            toAdd.positionsIds = positions.ToArray();

            var content = new StringContent(
                JsonSerializer.Serialize(toAdd),
                Encoding.UTF8,
                "application/json");

            var responsePOST = await httpClient.PostAsync(postNewOrderUrl, content);
            return responsePOST.IsSuccessStatusCode;
        }
        private async Task<int?> GetDiscountCodeId(string discountCode, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getDiscountCodeUrl + "?code=" + discountCode);
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<DiscountCodeDTO>(await response.Content.ReadAsStringAsync()).id;
            }
            return null;
        }

        public async Task<bool> CheckDiscountCode(string discountCode, int restaurantId, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getDiscountCodeUrl + "?code=" + discountCode);
            if (response.IsSuccessStatusCode)
            {
                DiscountCodeDTO code = JsonSerializer.Deserialize<DiscountCodeDTO>(await response.Content.ReadAsStringAsync());

                return (code.restaurantId == null || code.restaurantId == restaurantId) && DateTime.Compare(DateTime.Parse(code.dateTo), DateTime.Now) > 0;
            }
            return false;
        }
        public async Task<RestaurantViewModel[]> GetFavouriteRestaurants(ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            int clientID = Authentication.GetLoggedUserId(session);

            HttpResponseMessage response = await httpClient.GetAsync(getCustomerUrl + "?id=" + $"{clientID}");
            CustomerCDTO customer = null;
            if (response.IsSuccessStatusCode)
            {
                customer = JsonSerializer.Deserialize<CustomerCDTO>(await response.Content.ReadAsStringAsync());
            }

            if (customer.favouriteRestaurants != null)
                return new List<RestaurantCDTO>(customer.favouriteRestaurants).ConvertAll(r => mapper.Map<RestaurantViewModel>(r)).Where(r => r.state == RestaurantStateModel.Active).ToArray();
            return null;
        }

        public async Task<bool> AddToFavourites(int restaurantId, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.PostAsync(postAddToFavouritesUrl + "?id=" + $"{restaurantId}", null);

            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<AllReviewsViewModel> GetAllReviews(int restaurantId, ISession session)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(session);

            HttpResponseMessage response = await httpClient.GetAsync(getRestaurantReviewsUrl + "?id=" + $"{restaurantId}");

            AllReviewsViewModel reviews = new AllReviewsViewModel();
            if (response.IsSuccessStatusCode)
            {
                reviews.Reviews = JsonSerializer.Deserialize<List<ReviewDTO>>(await response.Content.ReadAsStringAsync())
                    .ConvertAll(r => mapper.Map<ReviewViewModel>(r)).ToArray();
            }

            response = await httpClient.GetAsync(getRestaurantUrl + "?id=" + $"{restaurantId}");
            if (response.IsSuccessStatusCode)
            {
                reviews.Restaurant = mapper.Map<RestaurantViewModel>(JsonSerializer.Deserialize<RestaurantCDTO>(await response.Content.ReadAsStringAsync()));
            }

            return reviews;
        }
    }
}
