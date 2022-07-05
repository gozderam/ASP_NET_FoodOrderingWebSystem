using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
using Xunit;

/*
 * Xunit 
 * Xunit creates a new test class instance for each test method run (DiscountCodeServiceTests per test method).
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
    public class DiscountCodeServiceTests : BaseTests, IDisposable
    {
        #region seeding and disposeing
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptions { get; }

        private Restaurant inDbRestaurant;
        private Restaurant inDbEmployeeRestaurant;
        private readonly int notInDbRestaurantId = -1;
        
        private Admin inDbAdmin;
        private readonly int notInDbAdminId = -1;

        private RestaurantEmployee inDbRestaurantEmployee;
        private readonly int notInDbRestaurantEmployeeId = -1;

        private DiscountCode inDbDiscountCodeGlobal;
        private DiscountCode inDbDiscountCodeForRestaurant;
        private DiscountCode inDbDiscountCodeForEmployeeRestaurant;
        private readonly string notInDbDiscountCodeCode = "incorrectCode";
        private readonly int notInDbDiscountCodeId = -1;

        public DiscountCodeServiceTests()
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
            inDbAdmin = new Admin()
            {
                Email = "InDbAdminMail@xd",
                Name = "InDbAdminName",
                Surname = "InDbAdminSurname"
            };
            context.Admins.Add(inDbAdmin);
            context.SaveChanges();

            inDbRestaurant = new Restaurant()
            {
                Name = "InDbRestaurantName",
                Address = new RestaurantAddress(),
            };
            context.Restaurants.Add(inDbRestaurant);
            context.SaveChanges();

            inDbEmployeeRestaurant = new Restaurant()
            {
                Name = "InDbEmployeeRestaurantName",
                Address = new RestaurantAddress()
            };
            context.Restaurants.Add(inDbEmployeeRestaurant);
            context.SaveChanges();

            inDbRestaurantEmployee = new RestaurantEmployee()
            {
                Name = "inDbRestaurantEmployeeName",
                Surname = "inDbRestaurantEmployeeSurname",
                Email = "inDbRestaurantEmployeeEmail",
                IsRestaurateur = false,
                Restaurant = inDbEmployeeRestaurant
            };
            context.RestaurantEmployees.Add(inDbRestaurantEmployee);
            context.SaveChanges();

            inDbDiscountCodeGlobal = new DiscountCode()
            {
                Code = "goodCodeGlobal",
                Percent = 0.05,
                AppliedToRestaurant = null,
                AppliesToAllRestaurants = true,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now
            };
            context.DiscountCodes.Add(inDbDiscountCodeGlobal);
            context.SaveChanges();

            inDbDiscountCodeForRestaurant = new DiscountCode()
            {
                Code = "goodCodeForRestaurant",
                Percent = 0.1,
                AppliedToRestaurant = inDbRestaurant,
                AppliesToAllRestaurants = false,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now
            };
            context.DiscountCodes.Add(inDbDiscountCodeForRestaurant);
            context.SaveChanges();

            inDbDiscountCodeForEmployeeRestaurant = new DiscountCode()
            {
                Code = "goodCodeForEmployeeRestaurant",
                Percent = 0.3,
                AppliedToRestaurant = inDbEmployeeRestaurant,
                AppliesToAllRestaurants = false,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now
            };
            context.DiscountCodes.Add(inDbDiscountCodeForEmployeeRestaurant);
            context.SaveChanges();
        }

        public void Dispose()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            recreateContext.Database.EnsureDeleted();
        }
        #endregion

        #region Admin add discount code tests
        [Fact]
        public async void AddDiscountCodeAdminGlobal_DiscountCodeAdded()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = null
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, inDbAdmin.Id);
            AssertSuccessServiceResult(serviceResult);
            int addedDiscountCodeId = (int)serviceResult.Data;

            // retrieving
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var foundDiscountCode = await getContext.DiscountCodes.FindAsync(addedDiscountCodeId);
            Assert.NotNull(foundDiscountCode);
            Assert.Equal(discountCode.percent, (int)(foundDiscountCode.Percent * 100));
            Assert.Equal(discountCode.code, foundDiscountCode.Code);
            Assert.Equal(discountCode.dateFrom, foundDiscountCode.DateFrom.ToString());
            Assert.Equal(discountCode.dateTo, foundDiscountCode.DateTo.ToString());
            Assert.True(foundDiscountCode.AppliesToAllRestaurants);
            Assert.Null(foundDiscountCode.AppliedToRestaurant);
        }

        [Fact]
        public async void AddDiscountCodeAdminForRestaurant_DiscountCodeAdded()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = inDbRestaurant.Id
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, inDbAdmin.Id);
            AssertSuccessServiceResult(serviceResult);
            int addedDiscountCodeId = (int)serviceResult.Data;

            // retrieving
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var foundDiscountCode = await getContext.DiscountCodes.Include(d => d.AppliedToRestaurant).FirstOrDefaultAsync(d => d.Id == addedDiscountCodeId);
            Assert.NotNull(foundDiscountCode);
            Assert.Equal(discountCode.percent, (int)(foundDiscountCode.Percent * 100));
            Assert.Equal(discountCode.code, foundDiscountCode.Code);
            Assert.Equal(discountCode.dateFrom, foundDiscountCode.DateFrom.ToString());
            Assert.Equal(discountCode.dateTo, foundDiscountCode.DateTo.ToString());
            Assert.False(foundDiscountCode.AppliesToAllRestaurants);
            Assert.NotNull(foundDiscountCode.AppliedToRestaurant);
            Assert.Equal(discountCode.restaurantId, foundDiscountCode.AppliedToRestaurant.Id);
        }

        [Fact]
        public async void AddDiscountCodeAdminForRestaurant_RestaurantNotFound()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = notInDbRestaurantId
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, inDbAdmin.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddDiscountCodeAdminGlobal_AdminNotFound()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = null
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, notInDbAdminId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddDiscountCodeAdminForRestaurant_AdminNotFound()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = inDbRestaurant.Id
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, notInDbAdminId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddDiscountCodeAdminGlobal_BadRequest_IncorrectPercent()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 100,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = null
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, inDbAdmin.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddDiscountCodeAdminGlobal_BadRequest_NullField()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 5,
                code = null,
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = null
            };

            var serviceResult = await discountCodeService.AddDiscountCodeAdmin(discountCode, inDbAdmin.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region Employee add discount code tests
        [Fact]
        public async void AddDiscountCodeEmployee_DiscountCodeAdded()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = inDbEmployeeRestaurant.Id
            };

            var serviceResult = await discountCodeService.AddDiscountCodeEmployee(discountCode, inDbRestaurantEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            int addedDiscountCodeId = (int)serviceResult.Data;

            // retrieving
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var foundDiscountCode = await getContext.DiscountCodes.Include(d => d.AppliedToRestaurant).FirstOrDefaultAsync(d => d.Id == addedDiscountCodeId);
            Assert.NotNull(foundDiscountCode);
            Assert.Equal(discountCode.percent, (int)(foundDiscountCode.Percent * 100));
            Assert.Equal(discountCode.code, foundDiscountCode.Code);
            Assert.Equal(discountCode.dateFrom, foundDiscountCode.DateFrom.ToString());
            Assert.Equal(discountCode.dateTo, foundDiscountCode.DateTo.ToString());
            Assert.False(foundDiscountCode.AppliesToAllRestaurants);
            Assert.NotNull(foundDiscountCode.AppliedToRestaurant);
            Assert.Equal(discountCode.restaurantId, foundDiscountCode.AppliedToRestaurant.Id);
        }

        //[Fact]
        //public async void AddDiscountCodeEmployee_NotFoundRestaurantId()
        //{
        //    // adding 
        //    using var addContext = new FoodDeliveryDbContext(ContextOptions);
        //    var discountCodeService = new DiscountCodeService(addContext, Mapper);

        //    NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
        //    {percent = 5,
        //        code = "summer",
        //        dateFrom = DateTime.Now.ToString(),
        //        dateTo = DateTime.Now.ToString(),
        //        restaurantId = notInDbRestaurantId
        //    };

        //    var serviceResult = await discountCodeService.AddDiscountCodeEmployee(discountCode, inDbRestaurantEmployee.Id);
        //    Assert.NotNull(serviceResult);
        //    Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
        //    Assert.Null(serviceResult.Data);
        //}

        //[Fact]
        //public async void AddDiscountCodeEmployee_AccessDenied()
        //{
        //    // adding 
        //    using var addContext = new FoodDeliveryDbContext(ContextOptions);
        //    var discountCodeService = new DiscountCodeService(addContext, Mapper);

        //    NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
        //    {percent = 5,
        //        code = "summer",
        //        dateFrom = DateTime.Now.ToString(),
        //        dateTo = DateTime.Now.ToString(),
        //        restaurantId = inDbRestaurant.Id
        //    };

        //    var serviceResult = await discountCodeService.AddDiscountCodeEmployee(discountCode, inDbRestaurantEmployee.Id);
        //    Assert.NotNull(serviceResult);
        //    Assert.Equal(ResultCode.Unauthorized, serviceResult.Code);
        //    Assert.Null(serviceResult.Data);
        //}

        [Fact]
        public async void AddDiscountCodeEmployee_EmployeeNotFound()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {percent = 5,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = inDbRestaurant.Id
            };

            var serviceResult = await discountCodeService.AddDiscountCodeEmployee(discountCode, notInDbRestaurantEmployeeId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddDiscountCodeEmployee_BadRequest_NullField()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 5,
                code = null,
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = inDbEmployeeRestaurant.Id
            };

            var serviceResult = await discountCodeService.AddDiscountCodeEmployee(discountCode, inDbRestaurantEmployee.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddDiscountCodeEmployee_BadRequest_IncorrectPercent()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(addContext, Mapper);

            NewDiscountCodeDTO discountCode = new NewDiscountCodeDTO()
            {
                percent = 0,
                code = "summer",
                dateFrom = DateTime.Now.ToString(),
                dateTo = DateTime.Now.ToString(),
                restaurantId = inDbEmployeeRestaurant.Id
            };

            var serviceResult = await discountCodeService.AddDiscountCodeEmployee(discountCode, inDbRestaurantEmployee.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region Get discount code tests
        [Fact]
        public async void GetGlobalDiscountCode_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(getContext, Mapper);

            var serviceResult = await discountCodeService.GetDiscountCode(inDbDiscountCodeGlobal.Code);
            AssertSuccessServiceResult(serviceResult);
            var receivedDiscountCode = serviceResult.Data;

            Assert.Equal(inDbDiscountCodeGlobal.Id, receivedDiscountCode.id);
            Assert.Equal(inDbDiscountCodeGlobal.Code, receivedDiscountCode.code);
            Assert.Equal(inDbDiscountCodeGlobal.Percent, receivedDiscountCode.percent / 100);
            Assert.Equal(inDbDiscountCodeGlobal.DateFrom.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture), receivedDiscountCode.dateFrom);
            Assert.Equal(inDbDiscountCodeGlobal.DateTo.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture), receivedDiscountCode.dateTo);
            Assert.Null(receivedDiscountCode.restaurantId);
        }

        [Fact]
        public async void GetForRestaurantDiscountCode_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(getContext, Mapper);

            var serviceResult = await discountCodeService.GetDiscountCode(inDbDiscountCodeForRestaurant.Code);
            AssertSuccessServiceResult(serviceResult);
            var receivedDiscountCode = serviceResult.Data;

            Assert.Equal(inDbDiscountCodeForRestaurant.Id, receivedDiscountCode.id);
            Assert.Equal(inDbDiscountCodeForRestaurant.Code, receivedDiscountCode.code);
            Assert.Equal(inDbDiscountCodeForRestaurant.Percent, receivedDiscountCode.percent / 100);
            Assert.Equal(inDbDiscountCodeForRestaurant.DateFrom.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture), receivedDiscountCode.dateFrom);
            Assert.Equal(inDbDiscountCodeForRestaurant.DateTo.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture), receivedDiscountCode.dateTo);
            Assert.Equal(inDbDiscountCodeForRestaurant.AppliedToRestaurant.Id, receivedDiscountCode.restaurantId);
        }

        [Fact]
        public async void GetDiscountCode_NotFound()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(getContext, Mapper);

            var serviceResult = await discountCodeService.GetDiscountCode(notInDbDiscountCodeCode);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region Get all dicount codes tests
        [Fact]
        public async void GetAllDiscountCodes_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(getContext, Mapper);

            var serviceResult = await discountCodeService.GetAllDiscountCodes(inDbAdmin.Id);
            AssertSuccessServiceResult(serviceResult);
            Assert.IsType<DiscountCodeDTO[]>(serviceResult.Data);
            Assert.Equal(await getContext.DiscountCodes.CountAsync(), serviceResult.Data.Length);
        }

        [Fact]
        public async void GetAllDiscountCodes_AdminNotFound()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(getContext, Mapper);

            var serviceResult = await discountCodeService.GetAllDiscountCodes(notInDbAdminId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region Admin delete discount code tests
        [Fact]
        public async void DeleteDiscountCodeAdmin_Success()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeAdmin(inDbDiscountCodeGlobal.Id, inDbAdmin.Id);
            AssertSuccessServiceResult(serviceResult);
            Assert.True(serviceResult.Data);

            // checking if deleted successfully
            var checkContext = new FoodDeliveryDbContext(ContextOptions);
            var result = await checkContext.DiscountCodes.FindAsync(inDbDiscountCodeGlobal.Id);
            Assert.Null(result);
        }

        [Fact]
        public async void DeleteDiscountCodeAdmin_DiscountCodeNotFound()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeAdmin(notInDbDiscountCodeId, inDbAdmin.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
        }

        [Fact]
        public async void DeleteDiscountCodeAdmin_AdminNotFound()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeAdmin(inDbDiscountCodeGlobal.Id, notInDbAdminId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
        }
        #endregion

        #region Employee delete discount code tests
        [Fact]
        public async void DeleteDiscountCodeEmployee_Success()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeEmployee(inDbDiscountCodeForEmployeeRestaurant.Id, inDbRestaurantEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            Assert.True(serviceResult.Data);

            // checking if deleted successfully
            var checkContext = new FoodDeliveryDbContext(ContextOptions);
            var result = await checkContext.DiscountCodes.FindAsync(inDbDiscountCodeForEmployeeRestaurant.Id);
            Assert.Null(result);
        }

        [Fact]
        public async void DeleteDiscountCodeEmployee_EmployeeNotFound()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeEmployee(inDbDiscountCodeForEmployeeRestaurant.Id, notInDbRestaurantEmployeeId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
        }

        [Fact]
        public async void DeleteDiscountCodeEmployee_DiscountCodeNotFound()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeEmployee(notInDbDiscountCodeId, inDbRestaurantEmployee.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
        }

        [Fact]
        public async void DeleteDiscountCodeEmployee_GlobalDiscountCodeCantBeDeleted()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeEmployee(inDbDiscountCodeGlobal.Id, inDbRestaurantEmployee.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.Unauthorized, serviceResult.Code);
        }

        [Fact]
        public async void DeleteDiscountCodeEmployee_DiscountCodeFromForeignRestaurantCantBeDeleted()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var discountCodeService = new DiscountCodeService(deleteContext, Mapper);

            var serviceResult = await discountCodeService.DeleteDiscountCodeEmployee(inDbDiscountCodeForRestaurant.Id, inDbRestaurantEmployee.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.Unauthorized, serviceResult.Code);
        }
        #endregion
    }
}
