using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantManagerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Services;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Common.DTO;
using SharedLib.Security;
using static Common.Definitions;

namespace RestaurantManagerApplication.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IRegisterService registerService;
        private readonly IConfiguration configuration;
        public RegisterController(IRegisterService registerService,IConfiguration configuration)
        {
            this.registerService = registerService;
            this.configuration = configuration;
        }
        [HttpGet]
        public IActionResult RegisterRestaurateur()
        {
            var restaurateurModel = new NewRestaurantEmployeeModel()
            {
                IsRestaurateur = true
            };
            return View(restaurateurModel);
        }

        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            var restaurateurModel = new NewRestaurantEmployeeModel()
            {
                IsRestaurateur = false
            };
            return View(restaurateurModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRestaurateur(NewRestaurantEmployeeModel newRestaurateur)
        {
            if (!(await registerService.RegisterRestaurateur(newRestaurateur)))
                return RedirectToAction("FailedNoLayout", "Responses", new
                {
                    message = "Failed To Register",
                    actionToGoBackTo = "RegisterRestaurateur",
                    controllerToGoBackTo = "Register"
                });
            if (!await Authentication.Login(newRestaurateur.Email, Application.RestaurantManagerApp, HttpContext.Session, configuration))
                return RedirectToAction("FailedNoLayout", "Responses", new
                {
                    message = "Login failed",
                    actionToGoBackTo = "LoginRestaurateur",
                    controllerToGoBackTo = "Login"
                });
            return RedirectToAction("Create", "Restaurant");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployee(NewRestaurantEmployeeModel newEmployee)
        {
            if (!(await registerService.RegisterEmployee(newEmployee, HttpContext.Session)))
                return RedirectToAction("FailedNoLayout", "Responses", new
                {
                    message = "Failed To Add Employee",
                    actionToGoBackTo = "RegisterEmployee",
                    controllerToGoBackTo = "Register"
                });

            return RedirectToAction("RequestSuccess", "Responses", new
            {
                message = "Employee added succesfully",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Order"
            });
        }

    }
}
