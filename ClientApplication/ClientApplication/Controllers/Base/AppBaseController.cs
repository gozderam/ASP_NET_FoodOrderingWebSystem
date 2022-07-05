using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Controllers.Base
{
    public abstract class AppControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Authentication.IsUserLogged(HttpContext.Session))
                filterContext.Result = RedirectToAction("Index", "Login");
        }

    }
}
