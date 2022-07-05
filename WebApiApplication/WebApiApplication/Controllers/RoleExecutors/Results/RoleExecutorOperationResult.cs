using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{
    public class RoleExecutorOperationResult<T> : IOperationResult<T>
    {
        public T Data { get; }

        public ResultCode Code { get; }

        public string Message { get; }

        public RoleExecutorOperationResult(T data, ResultCode code, string message = null)
        {
            Data = data;
            Code = code;
            Message = $"Role executor message: {message}";
        }

        public IOperationResult<object> GetInstanceWithObjectData()
        {
            return new RoleExecutorOperationResult<object>(Data, Code, Message);
        }
    }
}
