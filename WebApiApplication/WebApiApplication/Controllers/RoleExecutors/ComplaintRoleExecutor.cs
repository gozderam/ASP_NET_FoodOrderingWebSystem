using Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{
    public interface IComplaintRoleExecutor
    {
        Task<IOperationResult<object>> GetComplaint(int complaintId, IComplaintService service);
        Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint,IComplaintService service);
        Task<IOperationResult<bool>> DeleteComplaint(int complaintId, IComplaintService service);
        Task<IOperationResult<bool>> RespondToComplaint(int complaintId, string response, IComplaintService service);

    }

    public class AdminComplaintExecutor : IComplaintRoleExecutor
    {
        int userID;

        public AdminComplaintExecutor(int id)
        {
            userID = id;
        }

        public async Task<IOperationResult<bool>> DeleteComplaint(int complaintId, IComplaintService service)
        {
            return await service.DeleteComplaint(complaintId);
        }

        public async Task<IOperationResult<object>> GetComplaint(int complaintId, IComplaintService service)
        {
            return (await service.GetComplaintAdmin(complaintId)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint, IComplaintService service)
        {
            return new RoleExecutorOperationResult<int>(-1, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<bool>> RespondToComplaint(int complaintId, string response, IComplaintService service)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }
    }

    public class CustomerComplaintExecutor : IComplaintRoleExecutor
    {
        int userID;

        public CustomerComplaintExecutor(int id)
        {
            userID = id;
        }

        public async Task<IOperationResult<bool>> DeleteComplaint(int complaintId, IComplaintService service)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetComplaint(int complaintId, IComplaintService service)
        {
            return (await service.GetComplaintCustomer(complaintId, userID)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint, IComplaintService service)
        {
            return await service.PostComplaint(newComplaint, userID);
        }

        public async Task<IOperationResult<bool>> RespondToComplaint(int complaintId, string response, IComplaintService service)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }
    }

    public class EmployeeComplaintExecutor : IComplaintRoleExecutor
    {
        int userID;

        public EmployeeComplaintExecutor(int id)
        {
            userID = id;
        }

        public async Task<IOperationResult<bool>> DeleteComplaint(int complaintId, IComplaintService service)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetComplaint(int complaintId, IComplaintService service)
        {
            return (await service.GetComplaintEmployee(complaintId, userID)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint, IComplaintService service)
        {
            return new RoleExecutorOperationResult<int>(-1, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<bool>> RespondToComplaint(int complaintId, string response, IComplaintService service)
        {
            return await service.RespondToComplaint(complaintId, userID, response);
        }
    }

    public class RestaurateurComplaintExecutor : IComplaintRoleExecutor
    {
        int userID;

        public RestaurateurComplaintExecutor(int id)
        {
            userID = id;
        }

        public async Task<IOperationResult<bool>> DeleteComplaint(int complaintId, IComplaintService service)
        {
            return new RoleExecutorOperationResult<bool>(false, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<object>> GetComplaint(int complaintId, IComplaintService service)
        {
            return (await service.GetComplaintEmployee(complaintId, userID)).GetInstanceWithObjectData();
        }

        public async Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint, IComplaintService service)
        {
            return new RoleExecutorOperationResult<int>(-1, ResultCode.Unauthorized);
        }

        public async Task<IOperationResult<bool>> RespondToComplaint(int complaintId, string response, IComplaintService service)
        {
            return await service.RespondToComplaint(complaintId, userID, response);
        }
    }
}
