using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
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
    public class ReviewServiceTests : BaseTests, IDisposable
    {
        #region seeding and disposeing
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptions { get; }
        private Restaurant inDbRestaurant;     
        private readonly int notInDbRestaurantId = -2;

        private RestaurantEmployee inDbEmployee;

        private Client inDbCustomer;
        private readonly int notInDbCustomerId = -2;

        private Review inDbReview;
        private readonly int notInDbReviewId = -2;

        public ReviewServiceTests()
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
            context.RestaurantEmployees.Add(inDbEmployee);
            context.SaveChanges();

            inDbReview = new()
            {
                Client = inDbCustomer,
                Restaurant = inDbRestaurant,
                Content = "ReviewServiceTests Opinion content",
                Rate = 3,
            };
            Review inDbReview2 = new()
            {
                Client = inDbCustomer,
                Restaurant = inDbRestaurant,
                Content = "ReviewServiceTests Opinion content",
                Rate = 2,
            };
            inDbRestaurant.Rate = (inDbReview.Rate + inDbReview2.Rate)/2.0;
            context.Reviews.Add(inDbReview);
            context.Reviews.Add(inDbReview2);
            context.SaveChanges();
            inDbRestaurant.Reviews = new System.Collections.Generic.List<Review> { inDbReview, inDbReview2 };
            context.SaveChanges();
        }

        public void Dispose()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            recreateContext.Database.EnsureDeleted();
        }
        #endregion

        #region Add review tests
        [Fact]
        public async void AddRestaurantReview_ReviewAdded()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(addContext, Mapper);

            NewReviewDTO review = new NewReviewDTO()
            {
                content = "Opinion content",
                rating = 3,
                restaurantId = inDbRestaurant.Id,
            };

            var serviceResult = await reviewService.AddReview(review, inDbCustomer.Id);
            AssertSuccessServiceResult(serviceResult);
            int addedReviewId = (int)serviceResult.Data;


            // retrieving
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var foundReview = await getContext.Reviews.FindAsync(addedReviewId);
            Assert.NotNull(foundReview);
            Assert.Equal(review.content, foundReview.Content);
            Assert.Equal(review.rating, foundReview.Rate);

            inDbRestaurant = await addContext.Restaurants.Include(r => r.Reviews).FirstOrDefaultAsync(r => r.Id == inDbRestaurant.Id);
            var restauratRating = inDbRestaurant.Reviews.Select(r => r.Rate).Aggregate((a, b) => a + b) / (double)(inDbRestaurant.Reviews.Count);
            Assert.NotNull(inDbRestaurant);
            Assert.Equal(restauratRating, inDbRestaurant.Rate);
        }

        [Fact]
        public async void AddRestaurantReview_RestarantNotInDb()
        {
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(addContext, Mapper);

            // add review
            NewReviewDTO review = new NewReviewDTO()
            {
                content = "Opinion content",
                rating = 3,
                restaurantId = notInDbRestaurantId,
            };

            var serviceResult = await reviewService.AddReview(review, inDbCustomer.Id);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void AddRestaurantReview_AuthorNotInDb()
        {
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(addContext, Mapper);

            // add review
            NewReviewDTO review = new NewReviewDTO()
            {
                content = "Opinion content",
                rating = 3,
                restaurantId = inDbRestaurant.Id,
            };

            var serviceResult = await reviewService.AddReview(review, notInDbCustomerId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region Get review tests
        [Fact]
        public async void GetRestaurantReview_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetReview(inDbReview.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedReview = serviceResult.Data;

            Assert.Equal(inDbReview.Id, receivedReview.id);
            Assert.Equal(inDbReview.Content, receivedReview.content);
            Assert.Equal(inDbReview.Rate, receivedReview.rating);
            Assert.Equal(inDbReview.Client.Id, receivedReview.customerId);
            Assert.Equal(inDbReview.Restaurant.Id, receivedReview.restaurantId);
        }

        [Fact]
        public async void GetRestaurantReview_ReviewNotInDb()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetReview(notInDbReviewId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void GetRestaurantReviewR_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetReviewR(inDbReview.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedReviewR = serviceResult.Data;

            Assert.Equal(inDbReview.Id, receivedReviewR.id);
            Assert.Equal(inDbReview.Content, receivedReviewR.content);
            Assert.Equal(inDbReview.Rate, receivedReviewR.rating);
        }

        [Fact]
        public async void GetRestaurantReviewR_ReviewNotInDb()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetReviewR(notInDbReviewId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void GetAllRestaurantReview_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllReviews(inDbRestaurant.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedReviews = serviceResult.Data;

            Assert.Equal(2, receivedReviews.Length);
            Assert.Equal(inDbReview.Id, receivedReviews[1].id);
            Assert.Equal(inDbReview.Content, receivedReviews[1].content);
            Assert.Equal(inDbReview.Rate, receivedReviews[1].rating);
            Assert.Equal(inDbReview.Restaurant.Id, receivedReviews[1].restaurantId);
            Assert.Equal(inDbReview.Client.Id, receivedReviews[1].customerId);
        }
        [Fact]
        public async void GetAllRestaurantReviewR_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllReviewsR(inDbRestaurant.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedReviews = serviceResult.Data;

            Assert.Equal(2, receivedReviews.Length);
            Assert.Equal(inDbReview.Id, receivedReviews[1].id);
            Assert.Equal(inDbReview.Content, receivedReviews[1].content);
            Assert.Equal(inDbReview.Rate, receivedReviews[1].rating);
        }

        [Fact]
        public async void GetAllRestaurantReviewRForEmployee_Success()
        {
            var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllReviewsForEmployee(inDbEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedReviews = serviceResult.Data;

            Assert.Equal(2, receivedReviews.Length);
            Assert.Equal(inDbReview.Id, receivedReviews[1].id);
            Assert.Equal(inDbReview.Content, receivedReviews[1].content);
            Assert.Equal(inDbReview.Rate, receivedReviews[1].rating);
        }

        [Fact]
        public async void GetAllRestaurantReview_RestaurantNotInDb()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(getContext, Mapper);

            var serviceResult = await reviewService.GetAllReviews(notInDbReviewId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        #endregion

        #region Delete review tests
        [Fact]
        public async void DeleteRestaurantReview_Success()
        {
            // deleting
            var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var reviewService = new ReviewService(deleteContext, Mapper);

            var serviceResult = await reviewService.DeleteReview(inDbReview.Id);
            AssertSuccessServiceResult(serviceResult);
            Assert.True(serviceResult.Data);

            // checking if deleted successfully
            var checkContext = new FoodDeliveryDbContext(ContextOptions);
            var result = await checkContext.Reviews.FindAsync(inDbReview.Id);
            Assert.Null(result);
        }
        #endregion
    }

}
