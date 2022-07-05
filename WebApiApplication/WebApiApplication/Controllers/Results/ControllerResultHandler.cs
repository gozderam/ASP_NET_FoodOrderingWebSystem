using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.Results
{
    public class ControllerResultHandler<T>
    {
        public ActionResult<T> Handle(IOperationResult<T> serviceResult, HttpContext context)
        {
            context.Response.StatusCode = (int)serviceResult.Code;
            return new ActionResult<T>(serviceResult.Data);
        }
    }
}
