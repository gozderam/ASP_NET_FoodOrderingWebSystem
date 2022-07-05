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
    public class DiscountCodeController : ControllerBase
    {
        private readonly IDiscountCodeService discountCodeService;
        private readonly Dictionary<Role, Func<IDiscountCodeRoleExecutor>> roleExecutors = new()
        {
            { Role.Admin, () => new AdminDiscountCodeExecutor() },
            { Role.Restaurateur, () => new EmployeeDiscountCodeExecutor() },
            { Role.Employee, () => new EmployeeDiscountCodeExecutor() }
        };

        public DiscountCodeController(IDiscountCodeService discountCodeService)
        {
            this.discountCodeService = discountCodeService;
        }

        [HttpPost("discountcode")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<int?>> AddDiscountCode([FromBody] NewDiscountCodeDTO newDiscountCode)
        {
            return new ControllerResultHandler<int?>()
                .Handle(
                await roleExecutors[ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).role]()
                    .AddDiscountCode(discountCodeService, newDiscountCode, ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }

        [HttpGet("discountcode")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Employee, Role.Customer, Role.Restaurateur })]
        public async Task<ActionResult<DiscountCodeDTO>> GetDiscountCode([FromQuery] string code)
        {
            return new ControllerResultHandler<DiscountCodeDTO>()
                .Handle(
                await discountCodeService.GetDiscountCode(code),
                HttpContext);
        }

        [HttpDelete("discountcode")]
        [ApiKeyAuth(new Role[] { Role.Admin, Role.Employee, Role.Restaurateur })]
        public async Task<ActionResult<bool>> DeleteDiscountCode([FromQuery] int id)
        {
            return new ControllerResultHandler<bool>()
                .Handle(
                await roleExecutors[ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).role]()
                    .DeleteDiscountCode(discountCodeService, id, ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }

        [HttpGet("discountcode/all")]
        [ApiKeyAuth(new[] { Role.Admin })]
        public async Task<ActionResult<DiscountCodeDTO[]>> GetAllDiscountCodes()
        {
            return new ControllerResultHandler<DiscountCodeDTO[]>()
                .Handle(
                await roleExecutors[ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).role]()
                    .GetAllDiscountCodes(discountCodeService, ApiKeyConverter.GetIdAndRole(HttpContext.GetApiKey()).id),
                HttpContext);
        }
    }
}
