using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{ 
    public interface IRestaurantRoleExecutor
    {
        Task<IOperationResult<object>> GetRestaurant(int id, IRestaurantService restaurantService);
        Task<IOperationResult<object>> GetRestaurants(IRestaurantService restaurantService);
        Task<IOperationResult<object>> GetAllComplaints(int restaurantId, IComplaintService complaintService);
        Task<IOperationResult<object>> GetMenu(int id, IMenuService menuService);
        Task<IOperationResult<bool>> DeleteRestaurant(int id, IRestaurantService restaurantService);
        Task<IOperationResult<object>> GetAllReviews(IReviewService service, int restaurantId);
    }

    public class AdminRestaurantExecutor : IRestaurantRoleExecutor
    {
        readonly int userID;

        public AdminRestaurantExecutor(int adminIid)
        {
            userID = adminIid;
        }


        public async Task<IOperationResult<object>> GetRestaurant(int id, IRestaurantService restaurantService)
        {
            if (id == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji");
            return (await restaurantService.GetRestaurant(id)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetRestaurants(IRestaurantService restaurantService)
        {
            return (await restaurantService.GetAllRestaurants()).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetMenu(int id, IMenuService menuService)
        {
            if (id == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji");
            return (await menuService.GetMenu(id)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<bool>> DeleteRestaurant(int id, IRestaurantService restaurantService)
        {
            return await restaurantService.DeleteRestaurantAdmin(id);
        }

        public async Task<IOperationResult<object>> GetAllComplaints(int restaurantId, IComplaintService complaintService)
        {
            if (restaurantId == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji");
            return (await complaintService.GetAllComplaintsForAdmin(restaurantId)).GetInstanceWithObjectData();
        }
        public async Task<IOperationResult<object>> GetAllReviews(IReviewService service, int restaurantId)
            => ( restaurantId == -1 ? new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji"): (await service.GetAllReviews(restaurantId)).GetInstanceWithObjectData());
    }

    public class CustomerRestaurantExecutor : IRestaurantRoleExecutor
    {
        readonly int userID;

        public CustomerRestaurantExecutor(int customerId)
        {
            userID = customerId;
        }

        public async Task<IOperationResult<object>> GetRestaurant(int id, IRestaurantService restaurantService)
        {
            if (id == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji");
            return (await restaurantService.GetRestaurantC(id)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetRestaurants(IRestaurantService restaurantService)
        {
            return (await restaurantService.GetAllRestaurantsC()).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetMenu(int id, IMenuService menuService)
        {
            if (id == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji");
            return (await menuService.GetMenu(id)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<bool>> DeleteRestaurant(int id, IRestaurantService restaurantService)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetAllComplaints(int restaurantId, IComplaintService complaintService)
        {
            return new RoleExecutorOperationResult<object>(null,ResultCode.Unauthorized);
        }
        public async Task<IOperationResult<object>> GetAllReviews(IReviewService service, int restaurantId)
            => (restaurantId == -1 ? new RoleExecutorOperationResult<object>(null, ResultCode.Forbidded,
                    "Nie podano id restauracji") : (await service.GetAllReviews(restaurantId)).GetInstanceWithObjectData());
    }

    public class EmployeeRestaurantExecutor : IRestaurantRoleExecutor
    {
        readonly int userID;

        public EmployeeRestaurantExecutor(int employeeId)
        {
            userID = employeeId;
        }
        public async Task<IOperationResult<object>> GetRestaurant(int id, IRestaurantService restaurantService)
        {
            return (await restaurantService.GetRestaurantByEmployee(userID)).GetInstanceWithObjectData();
        }
        public async Task<IOperationResult<object>> GetRestaurants(IRestaurantService restaurantService)
        {
            return (await restaurantService.GetAllRestaurants()).GetInstanceWithObjectData();
        }
        public async Task<IOperationResult<object>> GetMenu(int id, IMenuService menuService)
        {
            return (await menuService.GetMenuByEmployee(userID)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<bool>> DeleteRestaurant(int id, IRestaurantService restaurantService)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetAllComplaints(int restaurantId, IComplaintService complaintService)
        =>  (await complaintService.GetAllComplaintsForRestaurantEmployee(userID)).GetInstanceWithObjectData();
        
        public async Task<IOperationResult<object>> GetAllReviews(IReviewService reviewService, int restaurantId)
            => (await reviewService.GetAllReviewsForEmployee(userID)).GetInstanceWithObjectData();
    }

    public class RestaurateurRestaurantExecutor : IRestaurantRoleExecutor
    {
        int userID;

        public RestaurateurRestaurantExecutor(int restaurateurId)
        {
            userID = restaurateurId;
        }
        public async Task<IOperationResult<object>> GetRestaurant(int id, IRestaurantService restaurantService)
        {
            return (await restaurantService.GetRestaurantByEmployee(userID)).GetInstanceWithObjectData();
        }
        public async Task<IOperationResult<object>> GetRestaurants(IRestaurantService restaurantService)
        {
            return (await restaurantService.GetAllRestaurants()).GetInstanceWithObjectData();
        }
        public async Task<IOperationResult<object>> GetMenu(int id, IMenuService menuService)
        {
            return (await menuService.GetMenuByEmployee(userID)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<bool>> DeleteRestaurant(int id, IRestaurantService restaurantService)
        {
            return await restaurantService.DeleteRestaurantRestaurateur(id,userID);
        }

        public async Task<IOperationResult<object>> GetAllComplaints(int restaurantId, IComplaintService complaintService)
        => (await complaintService.GetAllComplaintsForRestaurantEmployee(userID)).GetInstanceWithObjectData();
           
        public async Task<IOperationResult<object>> GetAllReviews(IReviewService reviewService, int restaurantId)
            => (await reviewService.GetAllReviewsForEmployee(userID)).GetInstanceWithObjectData();
    }


    public class EmptyRestaurantExecutor : IRestaurantRoleExecutor
    {

        public EmptyRestaurantExecutor(int _)
        { }

        public async Task<IOperationResult<object>> GetRestaurant(int id, IRestaurantService restaurantService)
            => new RoleExecutorOperationResult<object>(false, ResultCode.Unauthorized);
        public async Task<IOperationResult<object>> GetRestaurants(IRestaurantService restaurantService)
            => (await restaurantService.GetAllRestaurantsC()).GetInstanceWithObjectData();
        
        public async Task<IOperationResult<object>> GetMenu(int id, IMenuService menuService)
            => new RoleExecutorOperationResult<object>(false, ResultCode.Unauthorized);

        public async Task<IOperationResult<bool>> DeleteRestaurant(int id, IRestaurantService restaurantService)
            => new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);

        public async Task<IOperationResult<object>> GetAllComplaints(int restaurantId, IComplaintService complaintService)
            => new RoleExecutorOperationResult<object>(false, ResultCode.Unauthorized);

        public async Task<IOperationResult<object>> GetAllReviews(IReviewService reviewService, int restaurantId)
            => new RoleExecutorOperationResult<object>(false, ResultCode.Unauthorized);
    }
}
