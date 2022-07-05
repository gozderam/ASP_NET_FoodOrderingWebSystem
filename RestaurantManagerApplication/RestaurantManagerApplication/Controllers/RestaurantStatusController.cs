using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using RestaurantManagerApplication.Abstracts;
using SharedLib.Security;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class RestaurantStatusController : AppControllerBase
    {
        private readonly IRestaurantService restaurantService;

        public RestaurantStatusController(IRestaurantService restaurantService)
        {
            this.restaurantService = restaurantService;
        }

        public IActionResult RestaurantStatus()
        {
            RestaurantStatusViewModel viewModel = new RestaurantStatusViewModel(HttpContext.Session, restaurantService);

            return View(viewModel);
        }

        public async Task<IActionResult> DeactivateRestaurant()
        {
            await restaurantService.DeactivateRestaurant(HttpContext.Session);

            return RedirectToAction("RestaurantStatus");
        }

        public async Task<IActionResult> ReactivateRestaurant()
        {
            await restaurantService.ReactivateRestaurant(HttpContext.Session);

            return RedirectToAction("RestaurantStatus");
        }

        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            await restaurantService.DeleteRestaurant(id, HttpContext.Session);

            return RedirectToAction("Index", "Home");
        }
    }
}
