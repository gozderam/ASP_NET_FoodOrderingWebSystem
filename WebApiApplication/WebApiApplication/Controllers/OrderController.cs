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

namespace WebApiApplication.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly Dictionary<Role, Func<IOrderRoleExecutor>> roleExecutors = new()
        {
            { Role.Admin, () => new AdminOrderExecutor() },
            { Role.Customer, () => new CustomerOrderExecutor() },
            { Role.Employee, () => new EmployeeOrderExecutor() },
            { Role.Restaurateur, () => new RestaurateurOrderExecutor() }
        };

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet("order")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Employee, Role.Restaurateur, Role.Customer })]
        public async Task<ActionResult<object>> GetOrder([FromQuery] int id)
        {
            return new ControllerResultHandler<object>()
                .Handle(
                await roleExecutors[ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).role]().GetOrder(orderService, id),
                HttpContext);
        }

        [HttpPost("order")]
        [ApiKeyAuth(new Role[] { Role.Customer })]
        public async Task<ActionResult<int?>> AddOrder(NewOrderDTO newOrder)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await orderService.AddOrder(newOrder),
                HttpContext);
        }

        [HttpPost("order/refuse")]
        [ApiKeyAuth(new Role[] { Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<int>> RefuseOrder([FromQuery] int id)
        {
            return new ControllerResultHandler<int>()
                .Handle(
                await orderService.RefuseOrder(id),
                HttpContext);
        }

        [HttpPost("order/accept")]
        [ApiKeyAuth(new Role[] { Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<int>> AcceptOrder([FromQuery] int id)
        {
            return new ControllerResultHandler<int>()
                .Handle(
                await orderService.AcceptOrder(id, ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }


        [HttpPost("order/realized")]
        [ApiKeyAuth(new Role[] { Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<int>> RealizeOrder([FromQuery] int id)
        {
            return new ControllerResultHandler<int>()
                .Handle(
                await orderService.RealizeOrder(id),
                HttpContext);
        }

        [HttpGet("order/archive")]
        [ApiKeyAuth(new Role[] { Role.Admin })]
        public async Task<ActionResult<OrderADTO[]>> GetAllOrdersAdmin()
        {
            return new ControllerResultHandler<OrderADTO[]>()
                .Handle(
                await orderService.GetAllOrdersAdmin(ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }
    }
}
