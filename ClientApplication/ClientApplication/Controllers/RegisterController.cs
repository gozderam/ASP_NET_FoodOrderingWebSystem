using ClientApplication.Abstracts;
using ClientApplication.Controllers.Base;
using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApplication.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUserService userService;

        public RegisterController(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
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
        public async Task<IActionResult> Index(NewCustomerViewModel newCustomer)
        {
            if (ModelState.IsValid)
            {
                var res = await userService.AddNewUser(HttpContext.Session, newCustomer);
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