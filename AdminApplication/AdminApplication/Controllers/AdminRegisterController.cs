using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminApplication.Models;
using AdminApplication.Abstracts;
using AdminApplication.Controllers.Base;

namespace AdminApplication.Controllers
{
    public class AdminRegisterController : Controller
    {
        private readonly IUserService userService;

        public AdminRegisterController(IUserService userService)
        {
            this.userService = userService;
        }
        public IActionResult Index()
        {
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

        [HttpPost]
        public async Task<IActionResult> Index(NewAdminModel newAdminModel)
        {
            if (ModelState.IsValid)
            {
                var res = await userService.AddNewAdmin(newAdminModel);
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
    }
}
