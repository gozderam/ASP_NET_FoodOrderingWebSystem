using RestaurantManagerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IRegisterService
    {
        Task<bool> RegisterRestaurateur(NewRestaurantEmployeeModel newRestaurantEmployee);
        Task<bool> RegisterEmployee(NewRestaurantEmployeeModel newEmployee, ISession callingUserSession);
    }
}
