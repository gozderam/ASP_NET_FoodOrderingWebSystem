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
    public class DiscountCodesController : AppControllerBase
    {
        private readonly IDiscountCodeService discountCodeService;

        public DiscountCodesController(IDiscountCodeService discountCodeService)
        {
            this.discountCodeService = discountCodeService;
        }

        public async Task<IActionResult> DiscountCodes()
        {
            DiscountCodesViewModel discountCodesViewModel = new DiscountCodesViewModel(await discountCodeService.GetAllDiscountCodes(HttpContext.Session));

            return View(discountCodesViewModel);
        }

        public async Task<IActionResult> DeleteDiscountCode(int id)
        {
            await discountCodeService.DeleteDiscountCode(id, HttpContext.Session);

            return RedirectToAction("DiscountCodes");
        }

        public async Task<IActionResult> Index(NewDiscountCodeModel newDiscountCodeModel)
        {
            if (ModelState.IsValid)
            {
                if (!newDiscountCodeModel.AreDatesValid())
                {
                    return RedirectToAction("AddingFailed");
                }

                var res = await discountCodeService.AddNewDiscountCode(newDiscountCodeModel, HttpContext.Session);
                if (!res)
                {
                    return RedirectToAction("AddingFailed");
                }
                else
                {
                    return RedirectToAction("SuccessfullyAdded");
                }
            }
            return View();
        }

        public IActionResult SuccessfullyAdded()
        {
            return View();
        }

        public IActionResult AddingFailed()
        {
            return View();
        }
    }
}
