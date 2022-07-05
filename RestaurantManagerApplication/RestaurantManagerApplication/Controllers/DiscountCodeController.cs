using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using RestaurantManagerApplication.Services;
using RestaurantManagerApplication.Abstracts;
using System.Net.Http;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class DiscountCodeController : AppControllerBase
    {
        private IDiscountCodeService dCodeService;
        private IHttpClientFactory clientFactory;
        public DiscountCodeController(IDiscountCodeService dCodeService, IHttpClientFactory clientFactory)
        {
            this.dCodeService = dCodeService;
            this.clientFactory = clientFactory;
        }
        // GET: MenuController
        public async Task<IActionResult> Index()
        {
            return View(await dCodeService.GetAllDiscountCodes(HttpContext.Session));
        }
        // GET: MenuController/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: MenuController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCodeModel discountCode)
        {
            if (await dCodeService.AddDiscountCode(discountCode, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Discount code created successfully",
                    actionToGoBackTo = "Index",
                    controllerToGoBackTo = "Home"
                });
            else return RedirectToAction("RequestFailure","Responses",new 
                { 
                    message = "Failed to create Discount Code",
                    actionToGoBackTo = "Create",
                    controllerToGoBackTo = "DiscountCode"
                }); 
        }

        // POST: MenuController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (await dCodeService.DeleteDiscountCode(id, HttpContext.Session))
                return RedirectToAction("Index", "DiscountCode");
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to delete Discount Code",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Home"
            });
        }
    }
}
