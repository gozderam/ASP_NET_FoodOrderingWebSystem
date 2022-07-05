using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiApplication.Authorization;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
using Xunit;
using static Common.Definitions;

namespace WebApiTests
{
    public class AuthServiceTests : BaseTests
    {
        [Theory]
        [InlineData(Role.Customer, 1)]
        [InlineData(Role.Restaurateur, 2)]
        [InlineData(Role.Employee, 3)]
        [InlineData(Role.Admin, 4)]
        public async void AreRoleAndIdConsistentTests_NoUsers_FalseReturned_AddedUsers_TrueReturned(Role role, int id)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                      .UseInMemoryDatabase(databaseName: "auth_tests_db")
                      .Options;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new AuthService(context);

                Assert.False(await serviceToTest.AreRoleAndIdConsistent(role, id));

                switch (role)
                {
                    case Role.Customer:
                        context.Clients.Add(new Client { Id = id });
                        break;
                    case Role.Employee:
                        context.RestaurantEmployees.Add(new RestaurantEmployee { Id = id, IsRestaurateur = false });
                        break;
                    case Role.Restaurateur:
                        context.RestaurantEmployees.Add(new RestaurantEmployee { Id = id, IsRestaurateur = true });
                        break;
                    case Role.Admin:
                        context.Admins.Add(new Admin { Id = id });
                        break;

                }
                context.SaveChanges();
                Assert.True(await serviceToTest.AreRoleAndIdConsistent(role, id));
            }

        }
    }
}
