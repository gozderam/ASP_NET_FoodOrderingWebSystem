using ClientApplication.Abstracts;
using ClientApplication.Controllers.Base;
using ClientApplication.Models;
using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApplication.Controllers
{
    public class ComplaintsController : AppControllerBase
    {
        private readonly IComplaintService complaintService;
        public ComplaintsController(IComplaintService complaintService)
        {
            this.complaintService = complaintService;
        }

        public async Task<IActionResult> Complaints()
        {
            List<ComplaintModel> complants =  await complaintService.GetComplaintsForUser(HttpContext.Session);
            return View(new ComplaintViewModel(complants));
        }

        public IActionResult NewComplaint(int orderId)
        {
            return View(new NewComplaintViewModel() { orderId = orderId });
        }

        [HttpPost]
        public async Task<IActionResult> NewComplaint(int orderId, NewComplaintViewModel newComplaint)
        {
            newComplaint.orderId = orderId;

            if (ModelState.IsValid)
            {
                var res = await complaintService.AddNewComplaint(newComplaint, HttpContext.Session);
                if (!res)
                {
                    TempData["Message"] = "Your already made complaint to this order!";
                }
                else
                {
                    TempData["Message"] = "Complaint sent successfully!";
                }
                return RedirectToAction("NewComplaint", new { id = newComplaint.orderId});
            }
            return View(newComplaint);
        }
    }
}
