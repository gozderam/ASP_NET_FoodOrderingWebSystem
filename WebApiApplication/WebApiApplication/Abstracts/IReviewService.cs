using Common.DTO;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Abstracts
{
    public interface IReviewService
    {
        Task<IOperationResult<int?>> AddReview(NewReviewDTO newReview, int authorId);
        Task<IOperationResult<ReviewDTO>> GetReview(int reviewId);
        Task<IOperationResult<ReviewRDTO>> GetReviewR(int reviewId);
        Task<IOperationResult<ReviewDTO[]>> GetAllReviews(int revstaurantId);
        Task<IOperationResult<ReviewRDTO[]>> GetAllReviewsR(int revstaurantId);
        Task<IOperationResult<bool>> DeleteReview(int reviewId);
        Task<IOperationResult<ReviewRDTO[]>> GetAllReviewsForEmployee(int employeeId);
    }
}
