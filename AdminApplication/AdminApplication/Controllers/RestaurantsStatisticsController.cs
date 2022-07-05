using AdminApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using SharedLib.Security;
using AdminApplication.Services;
using AdminApplication.Abstracts;
using AdminApplication.Controllers.Base;

namespace AdminApplication.Controllers
{
    public class RestaurantsStatisticsController : AppControllerBase
    {
        private readonly IOrderService orderService;

        public RestaurantsStatisticsController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<IActionResult> RestaurantsStatistics()
        {
            var orders = await orderService.GetAllOrders(HttpContext.Session);
            RestaurantsStatisticsModel restaurantsStatisticsModel = new RestaurantsStatisticsModel(orders);

            return View(restaurantsStatisticsModel);
        }
    }
}
