using Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Exceptions;
using static Common.Definitions;

namespace WebApiApplication.Authorization
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private Role[] roles;

        public ApiKeyAuthAttribute(Role[] roles)
        {
            this.roles = roles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(Definitions.API_KEY_HEADER_NAME, out var potentialApiKey))
            {
                if(roles.Contains(Role.Empty))
                {
                    await next();
                    return;
                }
                else
                    throw new UnauthorizedRequestException("Unauthorized request: no api key in header.");
            }
               
            var apiKeyParts = potentialApiKey[0]?.Split(ApiKeySplitStrings, StringSplitOptions.None);
            if (apiKeyParts == null || apiKeyParts.Length != 2)
                throw new UnauthorizedRequestException("Unauthorized request: api key in incorrect format.");

            if (!int.TryParse(apiKeyParts[0], out int id))
                throw new UnauthorizedRequestException("Unauthorized request: api key in incorrect format.");

            if (!(new Regex("^[a-z]+$")).IsMatch(apiKeyParts[1]))
                throw new UnauthorizedRequestException("Unauthorized request: api key in incorrect format.");

            if (!Enum.TryParse(apiKeyParts[1], true, out Role role))
                throw new UnauthorizedRequestException("Unauthorized request: api key in incorrect format.");

            if (!roles.Contains(role))
                throw new UnauthorizedRequestException("Unauthorized request: user has no permissions to call this endpoint.");

            var authService = (IAuthService)context.HttpContext.RequestServices.GetService(typeof(IAuthService));

            if (!(await authService.AreRoleAndIdConsistent(role, id)))
                throw new UnauthorizedRequestException("Unauthorized request: user id and user role are not consistent.");

            await next();
        }
    }
}
