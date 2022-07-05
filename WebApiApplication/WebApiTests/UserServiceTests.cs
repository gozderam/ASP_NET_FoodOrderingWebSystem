using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
using Xunit;

namespace WebApiTests
{
    public class UserServiceTests : BaseTests
    {
        //helps to keep ids in test database unique
        private static int id = 1;


        [Theory]
        [InlineData("customer.mail@gmail.com")]
        public async void LoginTests_CustomerNotFound(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var serviceResult = await serviceToTest.LoginCustomer(email);
                Assert.NotNull(serviceResult);
                Assert.Equal(ResultCode.NotFound, serviceResult.Code);
                Assert.Null(serviceResult.Data);
            }
        }

        [Theory]
        [InlineData("restaurateur.mail@gmail.com")]
        [InlineData("employee.mail@gmail.com")]
        [InlineData("admin.mail@gmail.com")]
        public async void LoginTests_EmployeeNotFound(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var serviceResult = await serviceToTest.LoginRestaurantEmployee(email);
                Assert.NotNull(serviceResult);
                Assert.Equal(ResultCode.NotFound, serviceResult.Code);
                Assert.Null(serviceResult.Data);
            }
        }

        [Theory]
        [InlineData("admin.mail@gmail.com")]
        public async void LoginTests_AdminNotFound(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var serviceResult = await serviceToTest.LoginAdmin(email);
                Assert.NotNull(serviceResult);
                Assert.Equal(ResultCode.NotFound, serviceResult.Code);
                Assert.Null(serviceResult.Data);
            }
        }

        [Theory]
        [InlineData("admin.mail@gmail.com")]
        public async void AdminLoginTests_AdminLoggedIn(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var toAdd = new Admin() { Id = currId, Email = email };
                context.Admins.Add(toAdd);
                context.SaveChanges();

                var serviceResult = await serviceToTest.LoginAdmin(email);
                AssertSuccessServiceResult(serviceResult);
                Assert.Equal($"{currId},admin", serviceResult.Data);

                context.Admins.Remove(toAdd);
                context.SaveChanges();
            }
        }

        [Theory]
        [InlineData("customer.mail@gmail.com")]
        public async void CustomerLoginTests_CustomerLoggedIn(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var toAdd = new Client() { Id = currId, Email = email };
                context.Clients.Add(toAdd);
                context.SaveChanges();

                var serviceResult = await serviceToTest.LoginCustomer(email);
                AssertSuccessServiceResult(serviceResult);
                Assert.Equal($"{currId},customer", serviceResult.Data);

                context.Clients.Remove(toAdd);
                context.SaveChanges();
            }
        }

        [Theory]
        [InlineData("restaurateur.mail@gmail.com")]
        public async void RestaurateurLoginTests_RestaurateurLoggedIn(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var toAdd = new RestaurantEmployee() { Id = currId, Email = email, IsRestaurateur = true };
                context.RestaurantEmployees.Add(toAdd);
                context.SaveChanges();

                var serviceResult = await serviceToTest.LoginRestaurantEmployee(email);
                AssertSuccessServiceResult(serviceResult);

                Assert.Equal($"{currId},restaurateur", serviceResult.Data);

                context.RestaurantEmployees.Remove(toAdd);
                context.SaveChanges();
            }
        }

        [Theory]
        [InlineData("employee.mail@gmail.com")]
        public async void EmployeeLoginTests_EmployeeLoggedIn(string email)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            var currId = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var toAdd = new RestaurantEmployee() { Id = currId, Email = email, IsRestaurateur = false };
                context.RestaurantEmployees.Add(toAdd);
                context.SaveChanges();

                var serviceResult = await serviceToTest.LoginRestaurantEmployee(email);
                AssertSuccessServiceResult(serviceResult);
                Assert.Equal($"{currId},employee", serviceResult.Data);

                context.RestaurantEmployees.Remove(toAdd);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void DeleteClientTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                      .UseInMemoryDatabase(databaseName: "user_tests_db")
                      .Options;
            int currid = id++;
            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                context.Clients.Add(new Client { Id = currid });
                context.SaveChanges();

                var serviceResult = await serviceToTest.DeleteUser(currid, UserTypes.Client);
                AssertSuccessServiceResult(serviceResult);

                Assert.Null(context.Clients.Find(currid));

                context.SaveChanges();
            }

        }

        [Fact]
        public async void DeleteEmployeeTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                      .UseInMemoryDatabase(databaseName: "user_tests_db")
                      .Options;

            int currid = id++;
            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                context.RestaurantEmployees.Add(new RestaurantEmployee { Id = currid });
                context.SaveChanges();

                var serviceResult = await serviceToTest.DeleteUser(currid, UserTypes.Employee);
                AssertSuccessServiceResult(serviceResult);

                Assert.Null(context.RestaurantEmployees.Find(currid));

                context.SaveChanges();
            }

        }

        [Fact]
        public async void DeleteAdminTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                      .UseInMemoryDatabase(databaseName: "user_tests_db")
                      .Options;

            int currid = id++;
            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                context.Admins.Add(new Admin { Id = currid });
                context.SaveChanges();

                var serviceResult = await serviceToTest.DeleteUser(currid, UserTypes.Admin);
                AssertSuccessServiceResult(serviceResult);

                Assert.Null(context.Admins.Find(currid));

                context.SaveChanges();
            }

        }

        [Theory]
        [InlineData(UserTypes.Client)]
        [InlineData(UserTypes.Employee)]
        [InlineData(UserTypes.Admin)]
        public async void DeleteNonExistingUser(UserTypes type)
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                .UseInMemoryDatabase(databaseName: "user_tests_db")
                .Options;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var serviceResult = await serviceToTest.DeleteUser(-1, type);
                Assert.NotNull(serviceResult);
                Assert.Equal(ResultCode.NotFound, serviceResult.Code);
                Assert.False(serviceResult.Data);
            }

        }

        [Fact]
        public async void GetAllAdminsTests()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            int currid1 = id++;
            int currid2 = id++;
            int currid3 = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                Admin[] admins = new Admin[] { new Admin { Id = currid1 }, new Admin { Id = currid2 }, new Admin { Id = currid3 } };

                context.Admins.AddRange(admins);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetAllAdmins();
                AssertSuccessServiceResult(serviceResult);
                Assert.IsType<AdministratorDTO[]>(serviceResult.Data);
                Assert.Equal(3, serviceResult.Data.Length);

                context.Admins.RemoveRange(admins);
                context.SaveChanges();
            }
        }
        [Fact]
        public async void GetAllEmployeesTests()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            int currid1 = id++;
            int currid2 = id++;
            int currid3 = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                Restaurant[] restaurants = new Restaurant[] { new Restaurant { RestaurantEmployees = new List<RestaurantEmployee>() }, new Restaurant { RestaurantEmployees = new List<RestaurantEmployee>() }, new Restaurant { RestaurantEmployees = new List<RestaurantEmployee>() } };
                RestaurantEmployee[] employees = new RestaurantEmployee[] { new RestaurantEmployee { Id = currid1 }, new RestaurantEmployee { Id = currid2 }, new RestaurantEmployee { Id = currid3 } };

                restaurants[0].RestaurantEmployees.Add(employees[0]);
                restaurants[1].RestaurantEmployees.Add(employees[1]);
                restaurants[2].RestaurantEmployees.Add(employees[2]);

                context.Restaurants.AddRange(restaurants);
                context.RestaurantEmployees.AddRange(employees);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetAllEmployees();
                AssertSuccessServiceResult(serviceResult);

                Assert.IsType<EmployeeDTO[]>(serviceResult.Data);
                Assert.Equal(3, serviceResult.Data.Length);

                context.RestaurantEmployees.RemoveRange(employees);
                context.Restaurants.RemoveRange(restaurants);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetAllCustomersTests()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_test_db")
          .Options;

            int currid1 = id++;
            int currid2 = id++;
            int currid3 = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                Client[] clients = new Client[] { new Client { Id = currid1 }, new Client { Id = currid2 }, new Client { Id = currid3 } };

                context.Clients.AddRange(clients);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetAllClients();
                AssertSuccessServiceResult(serviceResult);
                Assert.IsType<CustomerADTO[]>(serviceResult.Data);
                Assert.Equal(3, serviceResult.Data.Length);

                context.Clients.RemoveRange(clients);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void AddNewCustomerTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                .UseInMemoryDatabase(databaseName: "user_tests_db")
                .Options;

            int currid1 = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                NewCustomerDTO toAdd = new NewCustomerDTO();
                var serviceResult = await serviceToTest.AddNewCustomer(toAdd);
                AssertSuccessServiceResult(serviceResult);

                var foundClient = await context.Clients.FindAsync(serviceResult.Data);
                Assert.NotNull(foundClient);

                context.Clients.Remove(foundClient);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void AddNewEmployeeTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                .UseInMemoryDatabase(databaseName: "user_tests_db")
                .Options;

            int currid1 = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                NewEmployeeDTO toAdd = new NewEmployeeDTO();
                var serviceResult = await serviceToTest.AddNewEmployee(toAdd);
                AssertSuccessServiceResult(serviceResult);

                var foundEmployee = await context.RestaurantEmployees.FindAsync(serviceResult.Data);
                Assert.NotNull(foundEmployee);

                context.RestaurantEmployees.Remove(foundEmployee);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void AddNewAdminTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                .UseInMemoryDatabase(databaseName: "user_tests_db")
                .Options;

            int currid1 = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                NewAdministratorDTO toAdd = new NewAdministratorDTO();
                var serviceResult = await serviceToTest.AddNewAdmin(toAdd);
                AssertSuccessServiceResult(serviceResult);

                var foundAdmin = await context.Admins.FindAsync(serviceResult.Data);
                Assert.NotNull(foundAdmin);

                context.Admins.Remove(foundAdmin);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetCustomerTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            int currid = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                Client client = new Client() { Id = currid, Name = "Jan", Surname = "Kowalski", Email = "jan.kowalski@gmail.com" };

                context.Clients.Add(client);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetCustomerA(currid);
                AssertSuccessServiceResult(serviceResult);
                Assert.IsType<CustomerADTO>(serviceResult.Data);
                Assert.Equal(currid, serviceResult.Data.id);
                Assert.Equal("Jan", serviceResult.Data.name);
                Assert.Equal("Kowalski", serviceResult.Data.surname);
                Assert.Equal("jan.kowalski@gmail.com", serviceResult.Data.email);

                context.Clients.Remove(client);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetEmployeeTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db1")
          .Options;

            int currid = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                Restaurant restaurant = new Restaurant() { RestaurantEmployees = new List<RestaurantEmployee>() };
                RestaurantEmployee employee = new RestaurantEmployee() { Id = currid, Name = "Jakub", Surname = "Nowak", Email = "jakub.nowak@gmail.com" };
                restaurant.RestaurantEmployees.Add(employee);

                context.Restaurants.Add(restaurant);
                context.RestaurantEmployees.Add(employee);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetEmployee(currid);
                AssertSuccessServiceResult(serviceResult);
                Assert.IsType<EmployeeDTO>(serviceResult.Data);
                Assert.Equal(currid, serviceResult.Data.id);
                Assert.Equal("Jakub", serviceResult.Data.name);
                Assert.Equal("Nowak", serviceResult.Data.surname);
                Assert.Equal("jakub.nowak@gmail.com", serviceResult.Data.email);

                context.RestaurantEmployees.Remove(employee);
                context.Restaurants.Remove(restaurant);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetAdminTest()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
          .UseInMemoryDatabase(databaseName: "user_tests_db")
          .Options;

            int currid = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                Admin admin = new Admin() { Id = currid, Name = "Jerzy", Surname = "Krętek", Email = "jerzy.kretek@gmail.com" };

                context.Admins.Add(admin);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetAdministrator(currid, currid);
                AssertSuccessServiceResult(serviceResult);
                Assert.IsType<AdministratorDTO>(serviceResult.Data);
                Assert.Equal(currid, serviceResult.Data.id);
                Assert.Equal("Jerzy", serviceResult.Data.name);
                Assert.Equal("Krętek", serviceResult.Data.surname);
                Assert.Equal("jerzy.kretek@gmail.com", serviceResult.Data.email);

                context.Admins.Remove(admin);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetAllOrdersTest_Succes()
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                  .UseInMemoryDatabase(databaseName: "user_order_tests_db")
                  .Options;

            int currid = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                ClientAddress address = new ClientAddress() { City = "Warsaw", Street = "Koszykowa", PostalCode = "00-000" };
                context.ClientAddresses.Add(address);
                context.SaveChanges();

                MenuPosition position = new MenuPosition() { Name = "Ice Cream", Price = 15, Description = "Chocolate" };
                context.MenuPositions.Add(position);
                context.SaveChanges();

                List<MenuPosition> menu = new List<MenuPosition> { position };

                RestaurantAddress restAddress = new RestaurantAddress() { City = "Warsaw", Street = "Koszykowa" };
                context.RestaurantAddresses.Add(restAddress);
                context.SaveChanges();

                Restaurant restaurant = new Restaurant() { Name = "Lodziarnia", Address = restAddress };
                context.Restaurants.Add(restaurant);
                context.SaveChanges();

                Client client = new Client { Id = 5 };
                context.Clients.Add(client);
                context.SaveChanges();

                Order order = new Order
                {
                    Id = currid,
                    PaymentMethod = PaymentMethod.Card,
                    OrderState = OrderState.Pending,
                    Date = DateTime.Parse("01/01/2001"),
                    OriginalPrice = 22.99,
                    FinalPrice = 19.99,
                    Address = address,
                    Restaurant = restaurant,
                    Client = client
                };
                order.OrdersMenuPositions = menu.ConvertAll(mp => new Order_MenuPosition() { Order = order, MenuPosition = mp, PositionsInOrder = 1});

                context.Orders.Add(order);
                context.SaveChanges();

                Order order1 = new Order
                {
                    Id = currid+1,
                    PaymentMethod = PaymentMethod.Transfer,
                    OrderState = OrderState.Completed,
                    Date = DateTime.Parse("12/08/2012"),
                    OriginalPrice = 35.49,
                    FinalPrice = 19.99,
                    Address = address,
                    Restaurant = restaurant,
                    Client = client
                };
                order1.OrdersMenuPositions = menu.ConvertAll(mp => new Order_MenuPosition() { Order = order1, MenuPosition = mp, PositionsInOrder = 1 });
                context.Orders.Add(order1);
                context.SaveChanges();

                var serviceResult = await serviceToTest.GetCustomerOrdersByCustomer(5);
                AssertSuccessServiceResult(serviceResult);
                Assert.IsType<OrderCDTO[]>(serviceResult.Data);
                Assert.Equal(PaymentMethodDTO.card, serviceResult.Data[0].paymentMethod);
                Assert.Equal(OrderStateDTO.pending, serviceResult.Data[0].state);
                Assert.Equal(DateTime.Parse("01/01/2001"), serviceResult.Data[0].date);
                Assert.Equal(22.99, serviceResult.Data[0].originalPrice);
                Assert.Equal(19.99, serviceResult.Data[0].finalPrice);
                Assert.Equal("Warsaw", serviceResult.Data[0].address.city);
                Assert.Equal("Koszykowa", serviceResult.Data[0].address.street);
                Assert.Equal("00-000", serviceResult.Data[0].address.postcode);
                Assert.Equal("Ice Cream", serviceResult.Data[0].positions[0].name);
                Assert.Equal("Chocolate", serviceResult.Data[0].positions[0].description);
                Assert.Equal(15, serviceResult.Data[0].positions[0].price);
                Assert.Equal("Lodziarnia", serviceResult.Data[0].restaurant.name);

                Assert.Equal(PaymentMethodDTO.transfer, serviceResult.Data[1].paymentMethod);
                Assert.Equal(OrderStateDTO.completed, serviceResult.Data[1].state);
                Assert.Equal(DateTime.Parse("12/08/2012"), serviceResult.Data[1].date);
                Assert.Equal(35.49, serviceResult.Data[1].originalPrice);
                Assert.Equal(19.99, serviceResult.Data[1].finalPrice);
                Assert.Equal("Warsaw", serviceResult.Data[1].address.city);
                Assert.Equal("Koszykowa", serviceResult.Data[1].address.street);
                Assert.Equal("00-000", serviceResult.Data[1].address.postcode);
                Assert.Equal("Ice Cream", serviceResult.Data[1].positions[0].name);
                Assert.Equal("Chocolate", serviceResult.Data[1].positions[0].description);
                Assert.Equal(15, serviceResult.Data[1].positions[0].price);
                Assert.Equal("Lodziarnia", serviceResult.Data[1].restaurant.name);

                context.Orders.Remove(order);
                context.Orders.Remove(order1);
                context.SaveChanges();
            }
        }

        [Fact]
        public async void GetAllOrdersTest_Null() //There is no customer in database
        {
            DbContextOptions<FoodDeliveryDbContext> options = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                  .UseInMemoryDatabase(databaseName: "user_tests_db")
                  .Options;

            int currid = id++;

            using (var context = new FoodDeliveryDbContext(options))
            {
                var serviceToTest = new UserService(context, Mapper);

                var serviceResult = await serviceToTest.GetCustomerOrdersByCustomer(-1);
                Assert.NotNull(serviceResult);
                Assert.Equal(ResultCode.NotFound, serviceResult.Code);
                Assert.Null(serviceResult.Data);
            }
        }
    }
}
