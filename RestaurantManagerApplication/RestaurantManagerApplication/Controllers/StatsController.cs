using Microsoft.AspNetCore.Mvc;
using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Controllers
{
    public class StatsController : AppControllerBase
    {
        private IOrderService orderService;
        public StatsController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            var stats = await orderService.GetAllOrdersArchive(HttpContext.Session);
            return View(stats);
        }
    }
}
