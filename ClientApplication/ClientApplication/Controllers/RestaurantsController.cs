using ClientApplication.Abstracts;
using ClientApplication.Controllers.Base;
using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApplication.Controllers
{
    public class RestaurantsController : AppControllerBase
    {
        private readonly IRestaurantService restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }
        public async Task<IActionResult> Index()
        {
            RestaurantViewModel[] restaurants;
            restaurants = await restaurantService.GetAllRestaurants(HttpContext.Session);

            return View(restaurants);
        }


        public async Task<IActionResult> Restaurant(int id)
        {
            return View(await restaurantService.GetRestaurantMenu(id, HttpContext.Session));
        }

        [HttpPost]
        public async Task<IActionResult> Restaurant(NewOrderViewModel newOrder)
        {
            int clientID = Authentication.GetLoggedUserId(HttpContext.Session);

            bool response = await restaurantService.PostNewOrder(clientID, newOrder, HttpContext.Session);
            if (response)
                return RedirectToAction("Restaurant", new { id = newOrder.restaurantId });
            else
                return Json(new { success = false, responseText = "No such discount code!" });
        }

        [HttpGet]
        public async Task<bool> CheckDiscountCode(string discountCode, int restaurantId)
        {
            return await restaurantService.CheckDiscountCode(discountCode, restaurantId, HttpContext.Session);
        }

        [HttpGet]
        public async Task<IActionResult> FavouriteRestaurants()
        {
            RestaurantViewModel[] restaurants;
            restaurants = await restaurantService.GetFavouriteRestaurants(HttpContext.Session);

            return View(restaurants);
        }

        public async Task<IActionResult> AddToFavourites(int id)
        {
            if(await restaurantService.AddToFavourites(id, HttpContext.Session))
                TempData["Message"] = "Restaurant added to favourites successfully!";
            else
                TempData["Message"] = "Something went wrong, try again!";
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> RestaurantReviews(int restaurantId)
        {
            return View(await restaurantService.GetAllReviews(restaurantId, HttpContext.Session));
        }
    }
}
