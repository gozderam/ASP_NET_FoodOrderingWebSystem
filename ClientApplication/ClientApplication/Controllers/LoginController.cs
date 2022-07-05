using ClientApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Common.Definitions;

namespace AdminApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration configuration;

        public LoginController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserLoginModel userLoginModel)
        {
            var isSuccess = await Authentication.Login(userLoginModel.Email, Application.CustomerApp, HttpContext.Session, configuration);
            if (isSuccess)
                return RedirectToAction("Index", "Restaurants");
            ViewBag.Message = "There is no user with this email!";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Authentication.Logout(HttpContext.Session);
            return RedirectToAction("Index");
        }
    }
}
