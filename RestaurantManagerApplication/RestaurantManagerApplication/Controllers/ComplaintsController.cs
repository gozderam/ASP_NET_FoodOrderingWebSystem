using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Controllers.Base;

namespace RestaurantManagerApplication.Controllers
{
    public class ComplaintsController : AppControllerBase
    {
        private readonly IComplaintService complaintService;
        public ComplaintsController(IComplaintService complaintService)
        {
            this.complaintService = complaintService;
        }

        [HttpGet("Complaints/Complaints/{id:int?}")]
        public async Task<IActionResult> Complaints()
        {
            var complants = await complaintService.GetComplaints(HttpContext.Session);
            foreach(ComplaintModel comp in complants)
            {
                if (comp.AttendingEmployee == null)
                    comp.AttendingEmployee = new EmployeeModel { Name = "", Surname = "" };
            }
            return View(new ComplaintViewModel(complants));
        }
        [HttpGet]
        public ActionResult Respond(int id)
        {
            return View(new RespondToComplaintViewModel { complaintId = id, text = ""});
        }
        [HttpPost]
        public async Task<IActionResult> Respond(RespondToComplaintViewModel response)
        {
            if(await complaintService.RespondToComplaint(response.complaintId,response.text,HttpContext.Session))
                return RedirectToAction("RequestSuccess", "Responses", new
                {
                    message = "Responded successfully",
                    actionToGoBackTo = "Complaints",
                    controllerToGoBackTo = "Complaints"
                });
            else return RedirectToAction("RequestFailure", "Responses", new
            {
                message = "Failed to respond",
                actionToGoBackTo = "Complaints",
                controllerToGoBackTo = "Complaints"
            });
        }

    }
}
