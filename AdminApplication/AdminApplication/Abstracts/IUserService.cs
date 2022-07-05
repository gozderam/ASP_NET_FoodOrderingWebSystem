using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminApplication.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace AdminApplication.Abstracts
{
    public interface IUserService
    {
        Task<UserDataModel[]> GetAllClients(ISession callingUserSession);
        Task<bool> DeleteClient(int id, ISession callingUserSession);
        Task<bool> AddNewAdmin(NewAdminModel newAdminModel);
    }
}
