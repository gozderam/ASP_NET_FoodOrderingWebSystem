using AdminApplication.Abstracts;
using AdminApplication.Controllers.Base;
using AdminApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApplication.Controllers
{
    public class ComplaintsController : AppControllerBase
    {
        private readonly IComplaintService complaintService;
        public ComplaintsController(IComplaintService complaintService)
        {
            this.complaintService = complaintService;
        }

        [HttpGet("Complaints/UserComplaints/{id:int?}")]
        public async Task<IActionResult> UserComplaints(int id = -1)
        {
            var complants = id == -1 ? new() : await complaintService.GetComplaintsForUser(id, HttpContext.Session);
            return View(new UserComplaintViewModel(complants, id));
        }

        [HttpGet("Complaints/RestaurantComplaints/{id:int?}")]
        public async Task<IActionResult> RestaurantComplaints(int id = -1)
        {
            var complants = id == -1 ? new() : await complaintService.GetComplaintsForRestaurant(id, HttpContext.Session);
            return View(new RestaurantComplaintViewModel(complants, id));
        }

    }
}
