using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
using WebApiApplication.Abstracts.Results;
using Xunit;
using System.Collections.Generic;
using WebApiTests.Helpers;

/*
 * Xunit 
 * Xunit creates a new test class instance for each test method run (ReviewServiceTests per test method).
 * Xunit does not run tests from one test class in parallel, but async tests run alternately - not one by one (await consequence).
 * We use Seed() method to initialize the in memory database. Seed() ensures that database is cerated, Dispose() takes care of deleting a database after a test.
 * Different database (different names) is used for each test (a cosequence of the fact that one test can be interrupted by await and another test starts then).
 * Because Seed() is invoked for each test method (as described above), each test method gets the database in a well-known state.
 * According to docs.microsoft linked below, it shouldn't affect tests performance significantly.
 * 
 * Creating dbContext instances
 * It is advised to create different dbContext for different operations.
 * For instance one instance for saving data to db, another to retrieve saved data (even in one test method).
 * 
 * The above info based on: https://docs.microsoft.com/en-gb/ef/core/testing/testing-sample
 */
namespace WebApiTests
{
    public class OrderServiceTests : BaseTests, IDisposable
    {
        #region seeding and disposeing
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptions { get; }
        private Restaurant inDbRestaurant;
        private readonly int notInDbRestaurantId = -2;

        private Client inDbCustomer;
        private readonly int notInDbCustomerId = -2;

        private Review inDbReview;
        private readonly int notInDbReviewId = -2;

        private Order inDbOrder;

        private RestaurantEmployee inDbEmployee;

        private Admin inDbAdmin;
        private readonly int notInDbAdminId = -1;

        private MenuPosition inDbMenuPosition1;
        private MenuPosition inDbMenuPosition2;

        public OrderServiceTests()
        {
            ContextOptions = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            Seed();
        }

        private void Seed()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);

            // recreating in-memory database
            recreateContext.Database.EnsureCreated();

            using var context = new FoodDeliveryDbContext(ContextOptions);
            // seeding data
            inDbRestaurant = new Restaurant()
            {
                Name = "ReviewServiceTests_Restaurant",
                Address = new RestaurantAddress(),
            };
            context.Restaurants.Add(inDbRestaurant);
            context.SaveChanges();

            inDbCustomer = new Client()
            {
                Name = "ReviewServiceTests_CustomerName",
                Surname = "ReviewServiceTests_CustomerSurname",
                Address = new ClientAddress { City = "CityA"},
                Email = "ReviewServiceTests_customer@gmail.com",
            };
            context.Clients.Add(inDbCustomer);
            context.SaveChanges();
            inDbEmployee = new RestaurantEmployee()
            {
                Name = "Adam",
                IsRestaurateur = true,
                Restaurant = inDbRestaurant,
            };
            context.RestaurantEmployees.Add(inDbEmployee);
            inDbMenuPosition1 = new MenuPosition() { Name = "Sausage", Description = "100% meat", Price = 3.00 };
            inDbMenuPosition2 = new MenuPosition() { Name = "Ice Cream", Description = "Chocolate", Price = 5.55 };
            context.MenuPositions.Add(inDbMenuPosition1);
            context.MenuPositions.Add(inDbMenuPosition2);
            context.SaveChanges();

            inDbOrder = new()
            {             
                PaymentMethod = PaymentMethod.Card,
                OrderState = OrderState.Unrealized,
                Date = DateTime.Now,
                OriginalPrice = 100,
                FinalPrice = 90,
                Address = inDbCustomer.Address,
                Client = inDbCustomer,
                Restaurant = inDbRestaurant,
                ResponsibleEmployee = inDbEmployee,
            };
            inDbOrder.OrdersMenuPositions = new List<Order_MenuPosition> {
                    new() { Order =  inDbOrder, MenuPosition = inDbMenuPosition1, PositionsInOrder = 1 },
                    new() { Order = inDbOrder, MenuPosition = inDbMenuPosition2, PositionsInOrder = 1 } };
            
            context.Orders.Add(inDbOrder);
            context.SaveChanges();
            inDbRestaurant.Orders.Add(inDbOrder);
            inDbCustomer.Orders.Add(inDbOrder);
            context.SaveChanges();
            //inDbReview = new()
            //{
            //    Client = inDbCustomer,
            //    Restaurant = inDbRestaurant,
            //    Content = "ReviewServiceTests Opinion content",
            //    Rate = 3,
            //};
            //context.Reviews.Add(inDbReview);
            //context.SaveChanges();

            inDbAdmin = new Admin()
            {
                Email = "InDbAdminMail@xd",
                Name = "InDbAdminName",
                Surname = "InDbAdminSurname"
            };
            context.Admins.Add(inDbAdmin);
            context.SaveChanges();
        }

        public void Dispose()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            recreateContext.Database.EnsureDeleted();
        }
        #endregion

        #region Add order tests
        //[Fact(Skip = "Database update needed")]
        [Fact]
        public async void AddNewOrder_OrderAdded()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var orderService = new OrderService(addContext, Mapper);

            NewOrderDTO newOrder = new NewOrderDTO()
            {
                paymentMethod = PaymentMethodDTO.card,
                date = DateTime.Now.ToString(),
                address = new AddressDTO() { city = "Warsaw", postcode = "00-000", street = "Noakowskiego" },
                discountcodeId = null,
                customerId = inDbCustomer.Id,
                restaurantId = inDbRestaurant.Id,
                positionsIds = new int[] { inDbMenuPosition1.Id, inDbMenuPosition1.Id, inDbMenuPosition2.Id }
            };

            var serviceResult = await orderService.AddOrder(newOrder);
            AssertSuccessServiceResult(serviceResult);
            int addedOrderId = (int)serviceResult.Data;

            // retrieving
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var foundOrder = await getContext.Orders
                .Include(o => o.Address)
                .Include(o => o.Client)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdersMenuPositions).ThenInclude(omp => omp.MenuPosition)
                .FirstOrDefaultAsync(o => o.Id == addedOrderId);
            Assert.NotNull(foundOrder);
            Assert.Equal(PaymentMethod.Card, foundOrder.PaymentMethod);
            Assert.Equal(newOrder.date, foundOrder.Date.ToString());
            Assert.Equal(newOrder.address.city, foundOrder.Address.City);
            Assert.Equal(newOrder.address.postcode, foundOrder.Address.PostalCode);
            Assert.Equal(newOrder.address.street, foundOrder.Address.Street);
            Assert.Null(foundOrder.DiscountCode);
            Assert.Equal(newOrder.customerId, foundOrder.Client.Id);
            Assert.Equal(newOrder.restaurantId, foundOrder.Restaurant.Id);

            // flatten MenuPositions
            var flattenedPositions = TestHelper.FlattenMenuPositions(foundOrder.OrdersMenuPositions);
            Assert.Equal(newOrder.positionsIds[0], flattenedPositions[0].Id);
            Assert.Equal(newOrder.positionsIds[1], flattenedPositions[1].Id);
            Assert.Equal(newOrder.positionsIds[2], flattenedPositions[2].Id);
        }
        #endregion

        #region get order tests
        [Fact]
        public async void GetAllRestaurantOrders_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var orderService = new OrderService(getContext, Mapper);

            var serviceResult = await orderService.GetAllOrders(inDbEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedOrders = serviceResult.Data;
            Assert.Single(receivedOrders);
            Assert.Equal(inDbOrder.Id, receivedOrders[0].id);
            Assert.Equal(inDbOrder.OriginalPrice, receivedOrders[0].originalPrice);
            Assert.Equal(inDbOrder.FinalPrice, receivedOrders[0].finalPrice);
            Assert.Equal(inDbOrder.Date, receivedOrders[0].date);
            Assert.Equal(inDbOrder.Address.City, receivedOrders[0].address.city);
            Assert.Equal(inDbEmployee.Name, receivedOrders[0].employee.name);

            // flatten MenuPositions
            var flattenedPositions = TestHelper.FlattenMenuPositions(inDbOrder.OrdersMenuPositions);
            Assert.Equal(flattenedPositions.Count, receivedOrders[0].positions.Length);
            Assert.Equal(flattenedPositions[0].Id, receivedOrders[0].positions[1].id);
            Assert.Equal(flattenedPositions[0].Name, receivedOrders[0].positions[1].name);
        }

        [Fact]
        public async void GetAllRestaurantOrders_RestaurantNotInDb()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var orderService = new OrderService(getContext, Mapper);

            var serviceResult = await orderService.GetAllOrders(notInDbRestaurantId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion
    }
}
