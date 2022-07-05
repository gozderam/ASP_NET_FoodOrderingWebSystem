using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{
    public interface IUserRoleExecutor
    {
        Task<IOperationResult<object>> GetAllComplaints(IComplaintService service, int userId);
        Task<IOperationResult<object>> GetCustomer(IUserService service, int userId);
        Task<IOperationResult<object>> GetEmployee(IUserService service, int employeeId);
    }


    public class AdminUserExecutor : IUserRoleExecutor
    {
        private int requestUserId;

        public AdminUserExecutor(int requestUserId)
        {
            this.requestUserId = requestUserId;
        }

        public async Task<IOperationResult<object>> GetAllComplaints(IComplaintService service, int userId)
        {
            if (userId == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.BadRequest, "No user id value provided."); 
            return (await service.GetAllComplaintsForClient(userId)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetCustomer(IUserService service, int userId)
        {
            if (userId == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.BadRequest, "No user id value provided.");
            return (await service.GetCustomerA(userId)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetEmployee(IUserService service, int employeeId)
        {
            if (employeeId == -1)
                return new RoleExecutorOperationResult<object>(null, ResultCode.BadRequest, "No user id value provided.");
            return (await service.GetEmployee(employeeId)).GetInstanceWithObjectData();
        }
    }

    public class CustomerUserExecutor : IUserRoleExecutor
    {
        private int requestUserId;

        public CustomerUserExecutor(int requestUserId)
        {
            this.requestUserId = requestUserId;
        }

        public async Task<IOperationResult<object>> GetAllComplaints(IComplaintService service, int userId)
        {
            if (!(userId == -1 || userId == requestUserId)) // !(id not provided or equal to the id of the user that makes the request)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized, "Trying to retrieve data without permissions.");

            return (await service.GetAllComplaintsForClient(requestUserId)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetCustomer(IUserService service, int userId)
        {
            if (!(userId == -1 || userId == requestUserId)) // !(id not provided or equal to the id of the user that makes the request)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized, "Trying to retrieve data without permissions.");
            return (await service.GetCustomerC(requestUserId)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<object>> GetEmployee(IUserService service, int employeeId)
        {
            return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized);
        }
    }

    public class EmployeeUserExecutor : IUserRoleExecutor
    {
        private int requestUserId;

        public EmployeeUserExecutor(int requestUserId)
        {
            this.requestUserId = requestUserId;
        }

        public async Task<IOperationResult<object>> GetAllComplaints(IComplaintService service, int userId)
        {
            return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetCustomer(IUserService service, int userId)
        {
            return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetEmployee(IUserService service, int employeeId)
        {
            if (!(employeeId == -1 || employeeId == requestUserId)) // !(id not provided or equal to the id of the user that makes the request)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized, "Trying to retrieve data without permissions.");
            return (await service.GetEmployee(requestUserId)).GetInstanceWithObjectData();
        }
    }

    public class RestaurateurUserExecutor : IUserRoleExecutor
    {
        private int requestUserId;

        public RestaurateurUserExecutor(int requestUserId)
        {
            this.requestUserId = requestUserId;
        }

        public async Task<IOperationResult<object>> GetAllComplaints(IComplaintService service, int userId)
        {
            return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetCustomer(IUserService service, int userId)
        {
            return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetEmployee(IUserService service, int employeeId)
        {
            if (!(employeeId == -1 || employeeId == requestUserId)) // !(id not provided or equal to the id of the user that makes the request)
                return new RoleExecutorOperationResult<object>(null, ResultCode.Unauthorized, "Trying to retrieve data without permissions.");
            return (await service.GetEmployee(requestUserId)).GetInstanceWithObjectData();
        }
    }
}
