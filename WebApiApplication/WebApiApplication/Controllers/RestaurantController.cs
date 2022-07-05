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
using WebApiApplication.Database.POCO;
using static Common.Definitions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiApplication.Controllers
{
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService restaurantService;
        private readonly IMenuService menuService;
        private readonly IOrderService orderService;
        private readonly IComplaintService complaintService;
        private readonly IReviewService reviewService;
        private readonly Dictionary<Role, Func<int, IRestaurantRoleExecutor>> executorByRole = new()
        {
            { Role.Admin, (id) => new AdminRestaurantExecutor(id) },
            { Role.Employee, (id) =>  new EmployeeRestaurantExecutor(id) },
            { Role.Restaurateur, (id) => new RestaurateurRestaurantExecutor(id) },
            { Role.Customer, (id) =>  new CustomerRestaurantExecutor(id) },
            { Role.Empty, (id) => new EmptyRestaurantExecutor(id) }
        };
        public RestaurantController(IRestaurantService restaurantService, IMenuService menuService, IReviewService reviewService, IComplaintService complaintService, IOrderService orderService)
        {
            this.restaurantService = restaurantService;
            this.menuService = menuService;
            this.reviewService = reviewService;
            this.complaintService = complaintService;
            this.orderService = orderService;
        }


        [HttpGet("restaurant")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Customer, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<object>> GetRestaurant([FromQuery] int id = -1)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());

            return new ControllerResultHandler<object>()
                .Handle(await executorByRole[res.role](res.id).GetRestaurant(id, restaurantService), 
                HttpContext);
        }

        [HttpPost("restaurant")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur })]
        public async Task<ActionResult<int?>> AddNewRestaurant([FromBody] NewRestaurantDTO newRestaurant)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await restaurantService.AddNewRestaurant(newRestaurant, ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }

        [HttpGet("restaurant/all")]
        [ApiKeyAuth(new Role[] { Role.Empty, Role.Admin, Role.Customer, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<object>> GetAllRestaurants()
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<object>()
                .Handle(await executorByRole[res.role](res.id).GetRestaurants(restaurantService),
                HttpContext);
        }

        [HttpDelete("restaurant")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur, Role.Admin })]
        public async Task<ActionResult<bool>> DeleteRestaurant([FromQuery] int id = -1)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<bool>()
                .Handle(await executorByRole[res.role](res.id).DeleteRestaurant(id, restaurantService),
                HttpContext);
        }

        [HttpGet("restaurant/menu/")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Customer, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<object>> GetMenu([FromQuery] int id = -1)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<object>()
                .Handle(await executorByRole[res.role](res.id).GetMenu(id, menuService),
                HttpContext);
        }

        [HttpPost("restaurant/menu/position")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur })]
        public async Task<ActionResult<int?>> AddNewMenuPosition([FromQuery] int id, [FromBody] NewPositionFromMenuDTO newPosition)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await menuService.AddNewMenuPosition(id, newPosition),
                HttpContext);
        }

        [HttpPatch("restaurant/menu/position")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur })]
        public async Task<ActionResult<int?>> EditMenuPosition([FromQuery] int id,[FromBody] NewPositionFromMenuDTO newPosition)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await menuService.EditMenuPosition(id,newPosition),
                HttpContext);
        }

        [HttpDelete("restaurant/menu/position")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur})]
        public async Task<ActionResult<bool>> DeleteMenuPosition([FromQuery] int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await menuService.DeleteMenuPosition(id),
                HttpContext);
        }

        [HttpPost("restaurant/menu/section")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur })]
        public async Task<ActionResult<int?>> AddNewMenuSection([FromQuery] string section)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<int?>()
                .Handle(
                await menuService.AddNewMenuSection(res.id, section),
                HttpContext);
        }

        [HttpPatch("restaurant/menu/section")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur })]
        public async Task<ActionResult<int?>> ChangeMenuSectionName([FromQuery] int id, [FromBody] string newName)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await menuService.ChangeMenuSectionName(id, newName),
                HttpContext);
        }

        [HttpDelete("restaurant/menu/section")]
        [ApiKeyAuth(new Role[] { Role.Restaurateur })]
        public async Task<ActionResult<bool>> DeleteMenuSection([FromQuery] int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await menuService.DeleteMenuSection(id),
                HttpContext);
        }

        [HttpPost("restaurant/favourite")]
        [ApiKeyAuth(new Role[] { Role.Customer })]
        public async Task<ActionResult<int?>> AddRestaurantToFavourites([FromQuery] int id)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<int?>()
                .Handle(
                await restaurantService.AddRestaurantToFavourites(res.id, id),
                HttpContext);
        }

        [HttpGet("restaurant/review/all")]
        [ApiKeyAuth(new Role[] { Role.Employee, Role.Admin, Role.Customer, Role.Restaurateur })]
        public async Task<ActionResult<object>> GetAllReviews([FromQuery] int id = -1)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<object>()
                .Handle(await executorByRole[res.role](res.id).GetAllReviews(reviewService,id),
                HttpContext);
        }

        [HttpGet("restaurant/order/all")]
        [ApiKeyAuth(new Role[] { Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<OrderRDTO[]>> GetAllWaitingOrders()
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<OrderRDTO[]>()
                .Handle(
                await orderService.GetAllOrders(res.id),
                HttpContext);
        }

        [HttpGet("restaurant/complaint/all")]
        [ApiKeyAuth(new Role[] { Role.Employee,Role.Restaurateur, Role.Admin })]
        public async Task<ActionResult<object>> GetAllComplaints([FromQuery] int id =-1)
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<object>()
                .Handle(
                await executorByRole[res.role](res.id).GetAllComplaints(id, complaintService),
                HttpContext);
        } 

        [HttpPost("restaurant/activate")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<RestaurantState?>> ActivateRestaurant([FromQuery] int id)
        {

            return new ControllerResultHandler<RestaurantState?>()
            .Handle(
            await restaurantService.ChangeRestaurantState(id, Database.POCO.RestaurantState.Active),
            HttpContext);
        }

        [HttpPost("restaurant/reactivate")]
        [ApiKeyAuth(new[] { Role.Restaurateur })]
        public async Task<ActionResult<RestaurantState?>> ReactivateRestaurant()
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<RestaurantState?>()
            .Handle(
            await restaurantService.RestaurantReactivate(res.id),
            HttpContext);
        }

        [HttpPost("restaurant/deactivate")]
        [ApiKeyAuth(new[] { Role.Restaurateur })]
        public async Task<ActionResult<RestaurantState?>> DeactivateRestaurant()
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<RestaurantState?>()
            .Handle(
            await restaurantService.RestaurantDeactivate(res.id),
            HttpContext);

        }

        [HttpPost("restaurant/block")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<RestaurantState?>> BlockRestaurant([FromQuery] int id)
        {
            return new ControllerResultHandler<RestaurantState?>()
            .Handle(
            await restaurantService.ChangeRestaurantState(id, Database.POCO.RestaurantState.Blocked),
            HttpContext);
        }

        [HttpPost("restaurant/unblock")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<RestaurantState?>> UnblockRestaurant([FromQuery] int id)
        {
            return new ControllerResultHandler<RestaurantState?>()
            .Handle(
            await restaurantService.ChangeRestaurantState(id, Database.POCO.RestaurantState.Active),
            HttpContext);
        }

        [HttpGet("restaurant/order/archive")]
        [ApiKeyAuth(new[] { Role.Restaurateur})]
        public async Task<ActionResult<OrderRDTO[]>> GetAllOrders()
        {
            var res = ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey());
            return new ControllerResultHandler<OrderRDTO[]>()
            .Handle(
            await restaurantService.GetAllOrders(res.id),
            HttpContext);
        }
    }
}
