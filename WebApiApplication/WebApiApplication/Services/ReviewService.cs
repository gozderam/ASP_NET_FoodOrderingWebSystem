using AutoMapper;
using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services.Results;
using System.Collections.Generic;
using System.Linq;

namespace WebApiApplication.Services
{
    public class ReviewService : IReviewService
    {
        private readonly FoodDeliveryDbContext dbContext;
        private readonly IMapper mapper;

        public ReviewService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IOperationResult<int?>> AddReview(NewReviewDTO newReview, int authorId)
        {
            var restaurant = await dbContext.Restaurants.FindAsync(newReview.restaurantId);
            if (restaurant == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound, "Restuarant not found");

            var author = await dbContext.Clients.FindAsync(authorId);
            if (author == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound, "Review author not found");

            var toAdd = mapper.Map<Review>(newReview);
            toAdd.Restaurant = restaurant;
            toAdd.Client = author;

            var reviewsCount = dbContext.Reviews.Where(r => r.Restaurant.Id == restaurant.Id).Count();
            restaurant.Rate = (double)(restaurant.Rate * reviewsCount + newReview.rating) / (double)(reviewsCount + 1);

            await dbContext.Reviews.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success);
        }

        public async Task<IOperationResult<ReviewDTO>> GetReview(int reviewId)
        {
            var review = await dbContext.Reviews
                .Include(r => r.Restaurant).Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return new ServiceOperationResult<ReviewDTO>(null, ResultCode.NotFound, "Review not found");

            return new ServiceOperationResult<ReviewDTO>(mapper.Map<ReviewDTO>(review), ResultCode.Success);  
        }

        public async Task<IOperationResult<ReviewRDTO>> GetReviewR(int reviewId)
        {
            var review = await dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return new ServiceOperationResult<ReviewRDTO>(null, ResultCode.NotFound, "Review not found");

            return new ServiceOperationResult<ReviewRDTO>(mapper.Map<ReviewRDTO>(review), ResultCode.Success);  
        }

        public async Task<IOperationResult<ReviewDTO[]>> GetAllReviews(int restaurantID)
        {
            var res = await dbContext.Restaurants.FirstOrDefaultAsync(r=>r.Id == restaurantID);
            if (res == null)
                return new ServiceOperationResult<ReviewDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");

            var reviews = await dbContext.Reviews.Include(r => r.Restaurant).Include(r => r.Client).Where(r => r.Restaurant.Id == restaurantID).ToListAsync();
            return new ServiceOperationResult<ReviewDTO[]>(reviews.ConvertAll(c => mapper.Map<ReviewDTO>(c)).ToArray(), ResultCode.Success);
        }
        public async Task<IOperationResult<ReviewRDTO[]>> GetAllReviewsForEmployee(int employeeId)
        {
            var employee = await dbContext.RestaurantEmployees.Include(r => r.Restaurant).Where(e => e.Id == employeeId).FirstOrDefaultAsync();
            if (employee == null)
                return new ServiceOperationResult<ReviewRDTO[]>(null, ResultCode.NotFound, "Pracownik o podanym identyfikatorze nie został znaleziony");
            
            if (employee.Restaurant == null)
                return new ServiceOperationResult<ReviewRDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");
            int restaurantId = employee.Restaurant.Id;
            var reviews = await dbContext.Reviews.Include(r => r.Restaurant).Where(r => r.Restaurant.Id == restaurantId).ToListAsync();
            return new ServiceOperationResult<ReviewRDTO[]>(reviews.ConvertAll(c => mapper.Map<ReviewRDTO>(c)).ToArray(), ResultCode.Success);
        }
        public async Task<IOperationResult<ReviewRDTO[]>> GetAllReviewsR(int restaurantID)
        {
            var res = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantID);
            if (res == null)
                return new ServiceOperationResult<ReviewRDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");

            var reviews = await dbContext.Reviews.Include(r => r.Restaurant).Where(r => r.Restaurant.Id == restaurantID).ToListAsync();
            return new ServiceOperationResult<ReviewRDTO[]>(reviews.ConvertAll(c => mapper.Map<ReviewRDTO>(c)).ToArray(), ResultCode.Success);
        }

        public async Task<IOperationResult<bool>> DeleteReview(int reviewId)
        {
            var review = await dbContext.Reviews
               .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Review not found");

            dbContext.Reviews.Remove(review);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<bool>(true, ResultCode.Success);
        }
    }
}
