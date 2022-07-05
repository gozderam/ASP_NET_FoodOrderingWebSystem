using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using RestaurantManagerApplication.Abstracts;
using System.Net.Http;
using SharedLib.Security;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class MenuController : AppControllerBase
    {
        private IMenuService menuService;
        private IHttpClientFactory clientFactory;
        public MenuController(IMenuService menuService, IHttpClientFactory clientFactory)
        {
            this.menuService = menuService;
            this.clientFactory = clientFactory;
        }
        // GET: MenuController
        public async Task<IActionResult> Index()
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            var menu = await menuService.GetMenu(HttpContext.Session);
            return View(menu);
        }
        // GET: MenuController/Create
        public ActionResult Create(int sectionId)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            NewMenuPositionModel menuPos = new() { sectionID = sectionId };
            return View(menuPos);
        }

        public async Task<IActionResult> ChoseSection()
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            var menu = await menuService.GetMenu(HttpContext.Session);
            return View(menu);
        }

        public ActionResult CreateSectionWithPosition()
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            return View();
        }


        public ActionResult CreateSection(int whileCreatingMenuPosition = 0)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized","Responses");
            TempData["withNewSection"] = whileCreatingMenuPosition == 0?false:true;
            return View();
        }
        // POST: MenuController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewMenuPositionModel menuPosition)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.CreateMenuPosition(menuPosition, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu position created successfully",
                    actionToGoBackTo =  "Index",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to create Menu position",
                actionToGoBackTo = "Create",
                controllerToGoBackTo = "Menu"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSection(MenuSectionModel menuSection)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.AddMenuSection(menuSection, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu section created successfully",
                    actionToGoBackTo = "Index",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to create Menu section",
                actionToGoBackTo = "CreateSection",
                controllerToGoBackTo = "Menu"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSectionWithPosition(MenuSectionModel menuSection)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.AddMenuSection(menuSection, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu section created successfully",
                    actionToGoBackTo = "ChoseSection",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to create Menu section",
                actionToGoBackTo = "CreateSection",
                controllerToGoBackTo = "Menu"
            });
        }

        // GET: MenuController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id, int sectionId)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            var menu = await menuService.GetMenu(HttpContext.Session);
            foreach(MenuSectionModel sec in menu)
            {
                if (sec.Id == sectionId)
                {
                    foreach(MenuPositionModel menPos in sec.MenuPositions)
                    {
                        if (menPos.Id == id)
                            return View(menPos);
                    }
                }
            }
            return View();
        }
        public ActionResult EditSection(int id, string oldName)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            return View(new MenuSectionModel { Name = oldName, Id = id});
        }

        // POST: MenuController/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(MenuPositionModel menuPosition)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.EditMenuPosition(menuPosition, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu position updated successfully",
                    actionToGoBackTo = "Index",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to update Menu position",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Menu"
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSection(MenuSectionModel menuSection)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.EditMenuSection(menuSection.Id,menuSection.Name, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu section updated successfully",
                    actionToGoBackTo = "Index",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to update Menu section",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Menu"
            });
        }

        // POST: MenuController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.DeleteMenuPosition(id, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu position deleted successfully",
                    actionToGoBackTo = "Index",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to delete Menu position",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Menu"
            });
        }
        public async Task<IActionResult> DeleteSection(int id)
        {
            if (Authentication.GetLoggedUserRole(HttpContext.Session) != Common.Definitions.Role.Restaurateur) return RedirectToAction("NotAuthorized", "Responses");
            if (await menuService.DeleteMenuSection(id, HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Menu section deleted successfully",
                    actionToGoBackTo = "Index",
                    controllerToGoBackTo = "Menu"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to delete Menu section",
                actionToGoBackTo = "Index",
                controllerToGoBackTo = "Menu"
            });
        }
    }
}
