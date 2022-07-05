using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Controllers.Results;
using WebApiApplication.Services.Results;

namespace WebApiApplication.Controllers
{
    [ApiController]
    public class DatabaseSeedController : Controller
    {
        private readonly ISeedDatabaseService databaseSeedService;
        private readonly IHostEnvironment hostingEnvironment;
        public DatabaseSeedController(ISeedDatabaseService srv, IHostEnvironment env)
        {
            this.databaseSeedService = srv;
            this.hostingEnvironment = env;
        }
        [HttpGet("database/seed")]
        public async Task<ActionResult<bool>> Seed()
        {
            if (!hostingEnvironment.IsDevelopment())
                return new ControllerResultHandler<bool>()
                    .Handle(
                    new ServiceOperationResult<bool>(false, ResultCode.NotFound),
                    HttpContext);
            return new ControllerResultHandler<bool>()
                .Handle(
                await databaseSeedService.Seed(),
                HttpContext);
        }
    }
}
