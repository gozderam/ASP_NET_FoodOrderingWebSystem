using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantManagerApplication.Controllers.Base;
using RestaurantManagerApplication.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace RestaurantManagerApplication.Controllers
{
    public class HomeController : AppControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        //public static IList<Restaurant> restaurants;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region pages
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Utilitiesanimation()
        {
            return View();
        }
        public IActionResult utilitiesother()
        {
            return View();
        }
        public IActionResult utilitiesborder()
        {
            return View();
        }
        public IActionResult utilitiescolor()
        {
            return View();
        }
        public IActionResult tables()
        {
            return View();
        }
        public IActionResult buttons()
        {
            return View();
        }
        public IActionResult cards()
        {
            return View();
        }
        public IActionResult charts()
        {
            return View();
        }
        public IActionResult blank()
        {
            return View();
        }
        public IActionResult view404()
        {
            return View();
        }
        public IActionResult forgotpassword()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
