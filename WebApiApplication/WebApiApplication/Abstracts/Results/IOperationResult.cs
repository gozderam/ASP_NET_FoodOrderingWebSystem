using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Abstracts.Results
{
    public interface IOperationResult<T> 
    {
        T Data { get; }

        ResultCode Code { get; }

        string Message { get; }

        bool IsSuccessful { get => ((int)Code)%100 == 2; }

        /// <returns>A copy of the current instance with Data casted do object</returns>
        IOperationResult<object> GetInstanceWithObjectData();

    }

    /// <summary>
    /// Enum specifying the result type of service operation. Int Enum values correspond to appropriate http response codes.
    /// </summary>
    /// <remarks>NOTE: Unexpected exceptions (like lost database connection) should be handled by 
    /// ErrorHandlingMiddleware. Errors resulting from business logic (like object not found in database) should be handled by ResultCode and returned from controller. </remarks>
    public enum ResultCode
    {
        // Success: 2xx
        Success = 200,

        // Bad Request 4xx
        BadRequest = 400,
        Unauthorized = 401,
        Forbidded = 403,
        NotFound = 404,

        // Server Error 5xx
        
    }
}
