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
    public class UsersManagementController : AppControllerBase
    {
        private readonly IUserService userService;

        public UsersManagementController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> UsersManagement()
        {
            UsersDataViewModel usersDataViewModel = new UsersDataViewModel();
            var users = await userService.GetAllClients(HttpContext.Session);
            usersDataViewModel.users = users == null ? new List<UserDataModel>().ToArray() : users;

            return View(usersDataViewModel);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            await userService.DeleteClient(id, HttpContext.Session);

            return RedirectToAction("UsersManagement");
        }
    }
}
