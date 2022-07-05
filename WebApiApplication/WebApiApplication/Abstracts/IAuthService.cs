using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Authorization;
using static Common.Definitions;

namespace WebApiApplication.Abstracts
{
    public interface IAuthService
    {
        Task<bool> AreRoleAndIdConsistent(Role role, int id);
    }
}
