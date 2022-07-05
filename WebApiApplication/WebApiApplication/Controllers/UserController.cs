using Common.DTO;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Authorization;
using WebApiApplication.Controllers.Results;
using WebApiApplication.Controllers.RoleExecutors;
using static Common.Definitions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiApplication.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IComplaintService complaintService;
        private readonly Dictionary<Role, Func<int, IUserRoleExecutor>> roleExecutors = new()
        {
            { Role.Admin, (requestUserId) => new AdminUserExecutor(requestUserId) },
            { Role.Restaurateur, (requestUserId) => new RestaurateurUserExecutor(requestUserId) },
            { Role.Employee, (requestUserId) => new EmployeeUserExecutor(requestUserId) },
            { Role.Customer, (requestUserId) => new CustomerUserExecutor(requestUserId) }
        };

        public UserController(IUserService userService, IComplaintService complaintService)
        {
            this.userService = userService;
            this.complaintService = complaintService;
        }

        #region login
        [HttpGet("user/customer/login")]
        public async Task<ActionResult<string>> LoginCustomer([FromQuery]string email)
        {
            return new ControllerResultHandler<string>()
                .Handle(
                await userService.LoginCustomer(email),
                HttpContext);
        }

        [HttpGet("user/employee/login")]
        public async Task<ActionResult<string>> LoginRestaurantEmployee([FromQuery] string email)
        {
            return new ControllerResultHandler<string>()
                .Handle(
                await userService.LoginRestaurantEmployee(email),
                HttpContext);
        }

        [HttpGet("user/admin/login")]
        public async Task<ActionResult<string>> LoginAdmin([FromQuery] string email)
        {
            return new ControllerResultHandler<string>()
                .Handle(
                await userService.LoginAdmin(email),
                HttpContext);
        }
        #endregion

        #region DELETE
        [HttpDelete("user/customer")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<bool>> DeleteClient([FromQuery]int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await userService.DeleteUser(id, UserTypes.Client),
                HttpContext);
        }

        [HttpDelete("user/employee")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<bool>> DeleteEmployee(int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await userService.DeleteUser(id, UserTypes.Employee),
                HttpContext);
        }
        
        [HttpDelete("user/admin")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<bool>> DeleteAdmin(int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await userService.DeleteUser(id, UserTypes.Admin),
                HttpContext);
        }

        #endregion

        [HttpGet("user/customer/all")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<CustomerADTO[]>> GetAllClients()
        {
            return new ControllerResultHandler<CustomerADTO[]>()
                .Handle(
                await userService.GetAllClients(),
                HttpContext);
        }

        [HttpGet("user/employee/all")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<EmployeeDTO[]>> GetAllEmployees()
        {
            return new ControllerResultHandler<EmployeeDTO[]>()
                .Handle(
                await userService.GetAllEmployees(),
                HttpContext);
        }

        [HttpGet("user/admin/all")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<AdministratorDTO[]>> GetAllAdmins()
        {
            return new ControllerResultHandler<AdministratorDTO[]>()
                .Handle(
                await userService.GetAllAdmins(),
                HttpContext);
        }

        #region POST

        [HttpPost("user/customer")]
        public async Task<ActionResult<int?>> AddNewClient(NewCustomerDTO newCustomer)
        {
            
            return new ControllerResultHandler<int?>()
                .Handle(
                await userService.AddNewCustomer(newCustomer),
                HttpContext);
        }

        [HttpPost("user/employee")]
        public async Task<ActionResult<int?>> AddNewEmployee(NewEmployeeDTO newEmployee)
        {

            return new ControllerResultHandler<int?>()
                .Handle(
                await userService.AddNewEmployee(newEmployee),
                HttpContext);
        }

        [HttpPost("user/admin")]
        public async Task<ActionResult<int?>> AddNewAdmin(NewAdministratorDTO newAdmin)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await userService.AddNewAdmin(newAdmin),
                HttpContext);
        }

        #endregion

        [HttpGet("user/customer")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Customer })]
        public async Task<ActionResult<object>> GetCustomer(int id = -1)
        {
            var (requestUserId, role) = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<object>()
                .Handle(await roleExecutors[role](requestUserId).GetCustomer(userService, id),
                HttpContext);
        }

        [HttpGet("user/admin")]
        [ApiKeyAuth(new Role[] { Role.Admin })]
        public async Task<ActionResult<AdministratorDTO>> GetAdmin(int id = -1)
        {
            int requestUserId = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id;

            return new ControllerResultHandler<AdministratorDTO>()
               .Handle(
               await userService.GetAdministrator(id, requestUserId),
               HttpContext);
        }

        [HttpGet("user/employee")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<object>> GetEmployee(int id = -1)
        {
            var (requestUserId, role) = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<object>()
                .Handle(
                await roleExecutors[role](requestUserId).GetEmployee(userService, id),
                HttpContext);
        }

        [HttpGet("user/customer/complaint/all")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Customer })]
        public async Task<ActionResult<object>> GetComplaints([FromQuery] int id = -1)
        {
            var (requestUserId, role) = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<object>()
                .Handle(await roleExecutors[role](requestUserId).GetAllComplaints(complaintService, id),
                HttpContext);
        }

        [HttpGet("user/customer/order/all")]
        [ApiKeyAuth(new Role[] { Role.Customer })]
        public async Task<ActionResult<OrderCDTO[]>> GetCustomerOrdersByCustomer()
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<OrderCDTO[]>()
                .Handle(
                await userService.GetCustomerOrdersByCustomer(res.id),
                HttpContext);
        }

    }
}
