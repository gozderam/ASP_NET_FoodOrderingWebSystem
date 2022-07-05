using Microsoft.AspNetCore.Http;
using RestaurantManagerApplication.Models;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IReviewService
    {
        Task<ReviewViewModel[]> GetReviews(ISession session);
    }
}
