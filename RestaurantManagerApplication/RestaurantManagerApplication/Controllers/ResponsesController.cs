using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class ResponsesController : AppControllerBase
    {
        public IActionResult NotAuthorized()
        {
            return View();
        }
        public IActionResult RequestFailure(string message, string actionToGoBackTo, string controllerToGoBackTo)
        {
            return View(new ResponseModel(){Message =  message, ActionToGoBackTo =  actionToGoBackTo,ControllerToGoBackTo = controllerToGoBackTo });
        }
        public IActionResult FailedNoLayout(string message, string actionToGoBackTo, string controllerToGoBackTo)
        {
            return View(new ResponseModel() { Message = message, ActionToGoBackTo = actionToGoBackTo, ControllerToGoBackTo = controllerToGoBackTo });
        }
        public IActionResult RequestSuccess(string message, string actionToGoBackTo, string controllerToGoBackTo)
        {
            return View(new ResponseModel() { Message = message, ActionToGoBackTo = actionToGoBackTo, ControllerToGoBackTo = controllerToGoBackTo });
        }
    }
}
