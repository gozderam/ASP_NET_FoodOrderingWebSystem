using Common.DTO;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Authorization;
using WebApiApplication.Controllers.Results;
using WebApiApplication.Controllers.RoleExecutors;
using static Common.Definitions;

namespace WebApiApplication.Controllers
{
    public class ComplaintController : Controller
    {
        private IComplaintService complaintService;
        private Dictionary<Role, Func<int, IComplaintRoleExecutor>> executorByRole = new()
        {
            { Role.Admin, (id) => { return new AdminComplaintExecutor(id); } },
            { Role.Employee, (id) => { return new EmployeeComplaintExecutor(id); } },
            { Role.Restaurateur, (id) => { return new RestaurateurComplaintExecutor(id); } },
            { Role.Customer, (id) => { return new CustomerComplaintExecutor(id); } }
        };
        public ComplaintController(IComplaintService dServ)
        {
            complaintService = dServ;
        }

        //Customer can only get complaint regarding his or her order
        //Employee can only get complaint regarding his or her restaurant's orders
        [HttpGet("complaint")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Customer, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<object>> GetComplaint(int id)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<object>()
                .Handle(await executorByRole[res.role](res.id).GetComplaint(id, complaintService),
                HttpContext);
        }

        //Customer can only submit complaint regarding his or her orders
        [HttpPost("complaint")]
        [ApiKeyAuth(new Role[] { Role.Customer })]
        public async Task<ActionResult<int>> AddNewComplaint([FromBody]NewComplaintDTO complaint)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<int>()
                .Handle(await executorByRole[res.role](res.id).PostComplaint(complaint, complaintService),
                HttpContext);
        }

        [HttpDelete("complaint")]
        [ApiKeyAuth(new Role[] { Role.Admin })]
        public async Task<ActionResult<bool>> DeleteComplaint(int id)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<bool>()
                .Handle(await executorByRole[res.role](res.id).DeleteComplaint(id, complaintService),
                HttpContext);
        }

        [HttpPost("complaint/respond")]
        [ApiKeyAuth(new Role[] { Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<bool>> RespondToComplaint([FromQuery] int id, [FromBody]string response)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<bool>()
                .Handle(await executorByRole[res.role](res.id).RespondToComplaint(id, response, complaintService),
                HttpContext);
        }


    }
}
