using AdminApplication.Models;
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
        public async Task<IActionResult> Login([FromBody]UserLoginModel userLoginModel)
        {
            var isSuccess = await Authentication.Login(userLoginModel.Email, Application.AdminApp, HttpContext.Session, configuration);
            if (isSuccess)
                return Ok();
            return Unauthorized();

        }

        [HttpGet]
        public IActionResult Logout()
        {
            Authentication.Logout(HttpContext.Session);
            return RedirectToAction("Index");

        }
    }
}
