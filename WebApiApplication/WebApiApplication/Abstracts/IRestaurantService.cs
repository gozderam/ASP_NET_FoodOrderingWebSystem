using Common.DTO;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services.Results;

namespace WebApiApplication.Abstracts
{
    public interface IRestaurantService
    {
        Task<IOperationResult<RestaurantCDTO>> GetRestaurantC(int id);
        Task<IOperationResult<RestaurantDTO>> GetRestaurant(int id);
        Task<IOperationResult<RestaurantDTO>> GetRestaurantByEmployee(int employeeID);
        Task<IOperationResult<RestaurantDTO[]>> GetAllRestaurants();
        Task<IOperationResult<RestaurantCDTO[]>> GetAllRestaurantsC();
        Task<IOperationResult<int?>> AddNewRestaurant(NewRestaurantDTO newRestaurant, int restaurateurID);
        Task<IOperationResult<bool>> DeleteRestaurantAdmin(int id);
        Task<IOperationResult<bool>> DeleteRestaurantRestaurateur(int restaurantID, int restaurateurID);
        Task<IOperationResult<RestaurantState?>> ChangeRestaurantState(int id, RestaurantState state);
        Task<IOperationResult<RestaurantState?>> RestaurantDeactivate(int id);
        Task<IOperationResult<RestaurantState?>> RestaurantReactivate(int id);
        Task<IOperationResult<int?>> AddRestaurantToFavourites(int customerId, int restaurantID);
        Task<IOperationResult<OrderRDTO[]>> GetAllOrders(int id);
    }
}
