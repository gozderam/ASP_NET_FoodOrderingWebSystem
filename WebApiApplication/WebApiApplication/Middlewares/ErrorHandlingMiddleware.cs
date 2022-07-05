using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Exceptions;

namespace WebApiApplication.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException e)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Resource not found");
            }
            catch (UnauthorizedRequestException e)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized request");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 400; // Should be 500
                if(e.InnerException!= null)
                    await context.Response.WriteAsync(e.Message + e.InnerException);
                else
                    await context.Response.WriteAsync(e.Message);
            }
        }
    }
}
