using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplication.Exceptions
{
    /// <summary>
    /// Use e.g. when requested resource is not in te db:
    /// IN SERVICE: 
    /// if(not in db)
    ///     throw new NotFoundException();
    /// </summary>
    public class UnauthorizedRequestException : Exception
    {
        public UnauthorizedRequestException(string message) : base(message)
        { }
    }
}
