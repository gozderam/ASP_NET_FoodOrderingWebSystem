using RestaurantManagerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IRestaurantService
    {
        Task<(bool success, int restaurantId)> AddNewRestaurant(NewRestaurantModel restaurantModel, ISession session);
        Task<EmployeeStatusModel> GetEmployeeShortInfo(int id, ISession callingUserSession);
        Task<bool> DeleteRestaurant(int id, ISession callingUserSession);
        Task<bool> DeactivateRestaurant(ISession callingUserSession);
        Task<bool> ReactivateRestaurant(ISession callingUserSession);
    }
}
