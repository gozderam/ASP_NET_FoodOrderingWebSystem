using ClientApplication.Abstracts;
using ClientApplication.Controllers.Base;
using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApplication.Controllers
{
    public class UserController : AppControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task<IActionResult> Orders()
        {
            OrderViewModel[] orders;
            orders = await userService.GetAllOrders(HttpContext.Session);

            return View(orders);
        }
    }
}
