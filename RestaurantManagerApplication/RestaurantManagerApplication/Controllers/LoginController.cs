using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Common.Definitions;
using RestaurantManagerApplication.Models;

namespace RestaurantManagerApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration configuration;

        public LoginController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        [HttpGet]
        public IActionResult LoginRestaurateur()
        {
            return View(new RestaurantEmployeeLoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> LoginRestaurateur(RestaurantEmployeeLoginModel userLoginModel)
        {   
            var isSuccess = await Authentication.Login(userLoginModel.Email, Application.RestaurantManagerApp, HttpContext.Session, configuration);
            if (isSuccess)
                return RedirectToAction("Index", "Order");
            return RedirectToAction("FailedNoLayout", "Responses", new
            {
                message = "Incorrect Email",
                actionToGoBackTo = "LoginRestaurateur",
                controllerToGoBackTo = "Login"
            });
        }
        [HttpGet]
        public IActionResult Logout()
        {
            Authentication.Logout(HttpContext.Session);
            return RedirectToAction("LoginRestaurateur", "Login");
        }
    }
}
