using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Services.Results
{
    public class ServiceOperationResult<T> : IOperationResult<T>
    {
        public T Data { get; }

        public ResultCode Code { get; }

        public string Message { get; }

        public ServiceOperationResult(T data, ResultCode code, string message = null)
        {
            Data = data;
            Code = code;
            Message = $"Service message: {message}";
        }

        public IOperationResult<object> GetInstanceWithObjectData()
        {
            return new ServiceOperationResult<object>(Data, Code, Message);
        }
    }
}
