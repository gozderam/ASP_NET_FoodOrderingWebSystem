using AdminApplication.Abstracts;
using AdminApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.DTO;
using AdminApplication.Controllers.Base;

namespace AdminApplication.Controllers
{
    public class RestaurantsManagementController : AppControllerBase
    {
        private readonly IRestaurantManagmentService restaurantService;
        private readonly IHttpClientFactory clientFactory;
        public RestaurantsManagementController(IRestaurantManagmentService restaurantService, IHttpClientFactory clientFactory)
        {
            this.restaurantService = restaurantService;
            this.clientFactory = clientFactory;
        }
        public async Task<IActionResult> RestaurantsManagement()
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(HttpContext.Session);
            RestaurantDataModel restaurantDataModel = new RestaurantDataModel();
            var restaurants = await restaurantService.GetAllRestaurants(httpClient);
            restaurantDataModel.restaurants = restaurants == null ? new List<RestaurantDTO>().ToArray() : await restaurantService.GetAllRestaurants(httpClient);

            return View(restaurantDataModel);
        }

        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(HttpContext.Session);
            await restaurantService.DeleteRestaurant(httpClient, id);

            return RedirectToAction("RestaurantsManagement");
        }

        public async Task<IActionResult> ActivateRestaurant(int id)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(HttpContext.Session);
            await restaurantService.RestaurantActivate(httpClient, id);

            return RedirectToAction("RestaurantsManagement");
        }

        public async Task<IActionResult> BlockRestaurant(int id)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(HttpContext.Session);
            await restaurantService.RestaurantBlock(httpClient, id);

            return RedirectToAction("RestaurantsManagement");
        }

        public async Task<IActionResult> UnblockRestaurant(int id)
        {
            var httpClient = clientFactory.CreateClient();
            httpClient.AddApiKeyHeader(HttpContext.Session);
            await restaurantService.RestaurantUnblock(httpClient, id);

            return RedirectToAction("RestaurantsManagement");
        }
    }
}
