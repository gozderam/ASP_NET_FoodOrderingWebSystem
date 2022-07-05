using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharedLib.Security;
using static Common.Definitions;
using Common.DTO;
using RestaurantManagerApplication.Abstracts;

namespace RestaurantManagerApplication.Models
{
    public class RestaurantStatusViewModel
    {
        public bool IsRestaurateur { get; }
        public string Message { get; }
        public RestaurantStateDTO RestaurantStatus { get; }
        public int RestaurantId { get; }

        public RestaurantStatusViewModel(ISession session, IRestaurantService restaurantService)
        {
            int employeeId = Authentication.GetLoggedUserId(session);
            Role employeeRole = Authentication.GetLoggedUserRole(session);
            EmployeeStatusModel employee = restaurantService.GetEmployeeShortInfo(employeeId, session).Result;

            this.IsRestaurateur = employee.isRestaurateur;
            this.Message = "Your restaurant is " + employee.restaurantStatus;
            this.RestaurantStatus = employee.restaurantStatus;
            this.RestaurantId = employee.restaurantId;
        }
    }

    public class EmployeeStatusModel
    {
        public bool isRestaurateur { get; set; }
        public RestaurantStateDTO restaurantStatus { get; set; }
        public int restaurantId { get; set; }
    }
}
