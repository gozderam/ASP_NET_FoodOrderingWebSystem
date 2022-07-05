using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ClientApplication.Abstracts
{
    public interface IRestaurantService
    {
        Task<RestaurantViewModel[]> GetAllRestaurants(ISession session);
        Task<RestaurantMenuViewModel> GetRestaurantMenu(int id, ISession session);
        Task<bool> PostNewOrder(int clientID, NewOrderViewModel newOrder, ISession session);
        Task<bool> CheckDiscountCode(string discountCode, int restaurantId, ISession session);
        Task<RestaurantViewModel[]> GetFavouriteRestaurants(ISession session);
        Task<bool> AddToFavourites(int restaurantId, ISession session);
        Task<AllReviewsViewModel> GetAllReviews(int restaurantId, ISession session);
     }
}
