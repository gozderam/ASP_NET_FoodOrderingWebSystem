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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;
        private readonly Dictionary<Role, Func<IReviewRoleExecutor>> roleExecutors = new ()
            {
                { Role.Admin, () => new AdminReviewExecutor()},
                { Role.Customer, () => new CustomerReviewExecutor()},
                { Role.Employee, () => new EmployeeReviewExecutor()},
                { Role.Restaurateur, () => new RestaurateurReviewExecutor()}
            };

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpPost("review")]
        [ApiKeyAuth(new Role[] { Role.Customer })]
        public async Task<ActionResult<int?>> AddReview(NewReviewDTO newReview)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await reviewService.AddReview(newReview, ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }

        [HttpGet("review")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Customer, Role.Employee, Role.Restaurateur})]
        public async Task<ActionResult<object>> GetReview([FromQuery] int id)
        {
            return new ControllerResultHandler<object>()
                .Handle(
                await roleExecutors[ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).role]().GetReview(reviewService, id),
                HttpContext);
        }

        [HttpDelete("review")]
        [ApiKeyAuth(new Role[] { Role.Admin })]
        public async Task<ActionResult<bool>> DeleteReview([FromQuery] int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await reviewService.DeleteReview(id),
                HttpContext);
        }

    }
}
