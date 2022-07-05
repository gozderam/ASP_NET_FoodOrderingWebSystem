using Microsoft.EntityFrameworkCore;
using System;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
using Common.DTO;
using System.Collections.Generic;
using Xunit;

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
    public class RestaurantServiceTests : BaseTests, IDisposable
    {
        #region seeding and disposeing
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptions { get; }
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptionsEmpty { get; }
        private Restaurant inDbRestaurant;
        private readonly int notInDbRestaurantId = -2;

        private Client inDbCustomer;
        private readonly int notInDbCustomerId = -2;

        private RestaurantEmployee inDbEmployee;
        private RestaurantEmployee inDbEmployeeNoRestaurant;
        private readonly int notInDbEmployeeId = -2;

        public RestaurantServiceTests()
        {
            ContextOptions = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            ContextOptionsEmpty = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            Seed();
        }

        private void Seed()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            using var recreateContextEmpty = new FoodDeliveryDbContext(ContextOptionsEmpty);

            // recreating in-memory database
            recreateContext.Database.EnsureCreated();
            recreateContextEmpty.Database.EnsureCreated();

            using var context = new FoodDeliveryDbContext(ContextOptions);
            // seeding data
            inDbRestaurant = new Restaurant()
            {
                Name = "ReviewServiceTests_Restaurant",
                Contact = "111-222-333",
                State = RestaurantState.Active,
                Rate = 5.0,
                ToPay = 25,
                TotalPayment = 100,
                DateOfJoining = DateTime.Parse("03/03/2003"),
                Address = new RestaurantAddress() { City = "Warsaw", PostalCode = "01-000", Street = "Nowowiejska 26" }
            };
            context.Restaurants.Add(inDbRestaurant);
            context.SaveChanges();

            inDbCustomer = new Client()
            {
                Name = "ReviewServiceTests_CustomerName",
                Surname = "ReviewServiceTests_CustomerSurname",
                Address = new ClientAddress(),
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
            inDbEmployeeNoRestaurant = new RestaurantEmployee()
            {
                Name = "Adam",
                IsRestaurateur = true,
            };
            context.RestaurantEmployees.Add(inDbEmployee);
            context.RestaurantEmployees.Add(inDbEmployeeNoRestaurant);
            inDbRestaurant.RestaurantEmployees = new List<RestaurantEmployee> { inDbEmployee };
            context.SaveChanges();
        }

        public void Dispose()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            recreateContext.Database.EnsureDeleted();
        }
        #endregion

        #region private methods
        private void CompareRestaruantAndRestaurantDTO(Restaurant restaurant, RestaurantDTO restaurantDTO)
        {
            Assert.Equal(restaurant.Id, restaurantDTO.id);
            Assert.Equal(restaurant.Contact, restaurantDTO.contactInformation);
            Assert.Equal(restaurant.Rate, restaurantDTO.rating);
            Assert.Equal(restaurant.State.ToString().ToLower(), restaurantDTO.state.ToString().ToLower());
            Assert.Equal(restaurant.Name, restaurantDTO.name);
            Assert.Equal(restaurant.ToPay, restaurantDTO.owing);
            Assert.Equal(restaurant.TotalPayment, restaurantDTO.aggregatePayment);
            Assert.Equal(restaurant.DateOfJoining.ToString(), restaurantDTO.dateOfJoining);

            //Address
            Assert.Equal(restaurant.Address.Street, restaurantDTO.address.street);
            Assert.Equal(restaurant.Address.City, restaurantDTO.address.city);
            Assert.Equal(restaurant.Address.PostalCode, restaurantDTO.address.postcode);

        }

        private void CompareRestaruantAndRestaurantCDTO(Restaurant restaurant, RestaurantCDTO restaurantDTO)
        {
            Assert.Equal(restaurant.Id, restaurantDTO.id);
            Assert.Equal(restaurant.Contact, restaurantDTO.contactInformation);
            Assert.Equal(restaurant.Rate, restaurantDTO.rating);
            Assert.Equal(restaurant.State.ToString().ToLower(), restaurantDTO.state.ToString().ToLower());
            Assert.Equal(restaurant.Name, restaurantDTO.name);

            //Address
            Assert.Equal(restaurant.Address.Street, restaurantDTO.address.street);
            Assert.Equal(restaurant.Address.City, restaurantDTO.address.city);
            Assert.Equal(restaurant.Address.PostalCode, restaurantDTO.address.postcode);

        }

        private void CompareRestaruantAndNewRestaurantDTO(Restaurant restaurant, NewRestaurantDTO restaurantDTO)
        {
            Assert.Equal(restaurant.Contact, restaurantDTO.contactInformation);
            Assert.Equal(restaurant.Name, restaurantDTO.name);

            //Address
            Assert.Equal(restaurant.Address.Street, restaurantDTO.address.street);
            Assert.Equal(restaurant.Address.City, restaurantDTO.address.city);
            Assert.Equal(restaurant.Address.PostalCode, restaurantDTO.address.postcode);

        }
        #endregion

        #region Get restaurant tests
        [Fact]
        public async void GetAllRestaurants_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new RestaurantService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllRestaurants();
            AssertSuccessServiceResult(serviceResult);
            var receivedRestaurant = serviceResult.Data;

            CompareRestaruantAndRestaurantDTO(inDbRestaurant, receivedRestaurant[0]);
        }

        [Fact]
        public async void GetAllRestaurants_RestaurantsNotInDb()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptionsEmpty);
            var reviewService = new RestaurantService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllRestaurants();

            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void GetAllRestaurantsC_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new RestaurantService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllRestaurantsC();
            AssertSuccessServiceResult(serviceResult);
            var receivedRestaurant = serviceResult.Data;

            CompareRestaruantAndRestaurantCDTO(inDbRestaurant, receivedRestaurant[0]);
        }

        [Fact]
        public async void GetAllRestaurantsC_RestaurantsNotInDb()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptionsEmpty);
            var reviewService = new RestaurantService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllRestaurantsC();

            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void GetRestaurant_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new RestaurantService(getContext, Mapper);

            var serviceResult = await reviewService.GetRestaurant(inDbRestaurant.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedRestaurant = serviceResult.Data;

            CompareRestaruantAndRestaurantDTO(inDbRestaurant, receivedRestaurant);

            var serviceResult2 = await reviewService.GetRestaurantC(inDbRestaurant.Id);
            AssertSuccessServiceResult(serviceResult2);
            var receivedRestaurant2 = serviceResult2.Data;

            CompareRestaruantAndRestaurantCDTO(inDbRestaurant, receivedRestaurant2);

            var serviceResult3 = await reviewService.GetRestaurantByEmployee(inDbEmployee.Id);
            AssertSuccessServiceResult(serviceResult3);
            var receivedRestaurant3 = serviceResult3.Data;

            CompareRestaruantAndRestaurantDTO(inDbRestaurant, receivedRestaurant3);
        }

        [Fact]
        public async void GetRestaurant_RestaurantsNotInDb()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptionsEmpty);
            var reviewService = new RestaurantService(getContext, Mapper);

            var serviceResult = await reviewService.GetRestaurant(notInDbRestaurantId);

            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);

            var serviceResult2 = await reviewService.GetRestaurantC(notInDbRestaurantId);

            Assert.NotNull(serviceResult2);
            Assert.Equal(ResultCode.NotFound, serviceResult2.Code);
            Assert.Null(serviceResult2.Data);

            var serviceResult3 = await reviewService.GetRestaurantByEmployee(notInDbEmployeeId);

            Assert.NotNull(serviceResult3);
            Assert.Equal(ResultCode.NotFound, serviceResult3.Code);
            Assert.Null(serviceResult3.Data);
        }

        #endregion

        #region add restaurant tests
        [Fact]
        public async void AddNewRestaurant_Success()
        {
            var addContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new RestaurantService(addContext, Mapper);
            NewRestaurantDTO newRes = Mapper.Map<NewRestaurantDTO>(inDbRestaurant);
            var serviceResult = await reviewService.AddNewRestaurant(newRes,inDbEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            var newResReal = await addContext.Restaurants.FindAsync(serviceResult.Data);
            CompareRestaruantAndNewRestaurantDTO(newResReal, newRes);
            Assert.Equal(inDbRestaurant.Address.PostalCode, newResReal.Address.PostalCode);
        }

        [Fact]
        public async void AddNewRestaurantToFavourites_Success()
        {
            var addContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new RestaurantService(addContext, Mapper);
            
            var serviceResult = await reviewService.AddRestaurantToFavourites(inDbCustomer.Id,inDbRestaurant.Id);
            AssertSuccessServiceResult(serviceResult);
            var customer = await addContext.Clients.Include(c=>c.FavouriteRestaurants).FirstOrDefaultAsync(c=>c.Id==inDbCustomer.Id);
            Assert.NotNull(customer.FavouriteRestaurants);
            Assert.Single(customer.FavouriteRestaurants);
            Assert.Equal(inDbRestaurant.Name, customer.FavouriteRestaurants[0].Name);
            Assert.Equal(inDbRestaurant.Id, customer.FavouriteRestaurants[0].Id);
        }

        [Fact]
        public async void AddNewRestaurantToFavourites_Failed()
        {
            var addContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new RestaurantService(addContext, Mapper);

            var serviceResult = await reviewService.AddRestaurantToFavourites(notInDbCustomerId, inDbRestaurant.Id);

            Assert.NotNull(serviceResult);
            Assert.Null(serviceResult.Data);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);

            var serviceResult2 = await reviewService.AddRestaurantToFavourites(inDbCustomer.Id, notInDbRestaurantId);

            Assert.NotNull(serviceResult2);
            Assert.Null(serviceResult2.Data);
            Assert.Equal(ResultCode.NotFound, serviceResult2.Code);

            var serviceResult3 = await reviewService.AddRestaurantToFavourites(notInDbCustomerId, notInDbRestaurantId);

            Assert.NotNull(serviceResult3);
            Assert.Null(serviceResult3.Data);
            Assert.Equal(ResultCode.NotFound, serviceResult3.Code);
        }

        #endregion

        #region delete restaurant tests
        [Fact]
        public async void DeleteRestaurant_Success()
        {
            var addContext = new FoodDeliveryDbContext(ContextOptions);
            var restaurantService = new RestaurantService(addContext, Mapper);

            var serviceResult = await restaurantService.DeleteRestaurantRestaurateur(inDbRestaurant.Id, inDbEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            Assert.True(serviceResult.Data);

            // checking if deleted successfully
            var checkContext = new FoodDeliveryDbContext(ContextOptions);
            var result = await checkContext.Restaurants.FindAsync(inDbRestaurant.Id);
            Assert.Null(result);

        }

        [Fact]
        public async void DeleteRestaurant_RestaurantNotInDb()
        {
            var addContext = new FoodDeliveryDbContext(ContextOptions);
            var restaurantService = new RestaurantService(addContext, Mapper);
            
            var serviceResult = await restaurantService.DeleteRestaurantRestaurateur(-1,inDbEmployeeNoRestaurant.Id);

            Assert.NotNull(serviceResult);
            Assert.False(serviceResult.Data);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);

        }
        #endregion

        #region changestate restaurant tests
        [Theory]
        [InlineData(RestaurantState.Active)]
        [InlineData(RestaurantState.Blocked)]
        [InlineData(RestaurantState.Deactivated)]
        public async void ChangeRestaurantStateTests(RestaurantState toChange)
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var stateService = new RestaurantService(getContext, Mapper);

            var serviceResult = await stateService.ChangeRestaurantState(inDbRestaurant.Id, toChange);
            AssertSuccessServiceResult(serviceResult);

            var restaurant = await getContext.Restaurants.FindAsync(inDbRestaurant.Id);
            Assert.Equal(toChange, restaurant.State);

        }

        [Fact]
        public async void ChangeNonExistingRestaurantStateTests()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var stateService = new RestaurantService(getContext, Mapper);
            var serviceResult = await stateService.ChangeRestaurantState(notInDbRestaurantId, RestaurantState.Active);

            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
        }
        #endregion
    }



}



