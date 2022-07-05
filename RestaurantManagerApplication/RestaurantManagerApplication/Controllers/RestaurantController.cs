using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using SharedLib.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RestaurantManagerApplication.Abstracts;
using Microsoft.Extensions.Configuration;
using static Common.Definitions;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class RestaurantController : AppControllerBase
    {
        private readonly IRegisterService registerService;
        private readonly IConfiguration configuration;
        private readonly IRestaurantService restaurantService;

        public RestaurantController(IRegisterService registerService, IRestaurantService restaurantService, IConfiguration configuration)
        {
            this.registerService = registerService;
            this.restaurantService = restaurantService;
            this.configuration = configuration;
        }

        // GET: RestaurantController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RestaurantController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RestaurantController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RestaurantController/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewRestaurantModel restaurant)
        {
            (bool addRestaurantSuccess, int newRestaurantId) = await restaurantService.AddNewRestaurant(restaurant, HttpContext.Session);
            if (!addRestaurantSuccess)
                return RedirectToAction("RequestFailure", "Responses", new
                {
                    message = "Failed to create Restaurant",
                    actionToGoBackTo = "Create",
                    controllerToGoBackTo = "Restaurant"
                });

            return RedirectToAction("Index", "Home");
        }
    }
}
