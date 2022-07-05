using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using RestaurantManagerApplication.Abstracts;
using System.Net.Http;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class OrderController : AppControllerBase
    {
        private IOrderService orderService;
        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await orderService.GetAllUnrealisedOrders(HttpContext.Session));
        }
        public async Task<IActionResult> MyOrders()
        {
            return View(await orderService.GetAllMyPendingOrders(HttpContext.Session));
        }

        public async Task<IActionResult> Manage(int id)
        {
            return View(await orderService.GetOrder(id, HttpContext.Session)); 
        }
        public async Task<IActionResult> Details(int id)
        {
            return View(await orderService.GetOrder(id, HttpContext.Session));
        }

        public async Task<IActionResult> Refuse(int id)
        {
            if (await orderService.RefuseOrder(id, HttpContext.Session))
                return RedirectToAction("Index", "Order");
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to process Order",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Order"
            });
        }
        public async Task<IActionResult> Accept(int id)
        {
            if (await orderService.AcceptOrder(id, HttpContext.Session))
                return RedirectToAction("Index", "Order");
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to process Order",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Order"
            });
        }
        public async Task<IActionResult> MarkRealised(int id)
        {
            if (await orderService.MarkOrderRealised(id, HttpContext.Session))
                return RedirectToAction("Index", "Order");
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to process Order",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Order"
            });
        }
    }
}
