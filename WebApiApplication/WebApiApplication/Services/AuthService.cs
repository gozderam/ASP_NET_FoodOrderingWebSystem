using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Authorization;
using WebApiApplication.Database;
using static Common.Definitions;

namespace WebApiApplication.Services
{
    public class AuthService : IAuthService
    {
        private FoodDeliveryDbContext dbContext;
        public AuthService(FoodDeliveryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AreRoleAndIdConsistent(Role role, int id)
        {
            switch(role)
            {
                case Role.Customer:
                    var customer = await dbContext.Clients.FindAsync(id);
                    if(customer != null)
                        return true;
                    break;
                case Role.Restaurateur:
                    var restaurateur = await dbContext.RestaurantEmployees.FindAsync(id);
                    if (restaurateur != null && restaurateur.IsRestaurateur)
                        return true;
                    break;
                case Role.Employee:
                    var employee = await dbContext.RestaurantEmployees.FindAsync(id);
                    if (employee != null && !employee.IsRestaurateur)
                        return true;
                    break;
                case Role.Admin:
                    var admin = await dbContext.Admins.FindAsync(id);
                    if (admin != null)
                        return true;
                    break;
            }
            return false;
        }
    }
}
