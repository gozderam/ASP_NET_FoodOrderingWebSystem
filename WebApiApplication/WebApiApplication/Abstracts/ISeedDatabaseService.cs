using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Abstracts
{
    public interface ISeedDatabaseService
    {
        Task<IOperationResult<bool>> Seed();
    }
}
