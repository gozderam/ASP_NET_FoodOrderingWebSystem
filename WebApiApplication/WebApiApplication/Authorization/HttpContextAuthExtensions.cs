using Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Authorization
{
    public static class HttpContextAuthExtensions
    {
        public static string GetApiKey(this HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.TryGetValue(Definitions.API_KEY_HEADER_NAME, out var apiKey))
            {
                return $"{-2},empty";
            }

            return apiKey;
        }
    }
}
