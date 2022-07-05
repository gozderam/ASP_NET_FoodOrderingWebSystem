using Common.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Abstracts
{
    public interface IComplaintService
    {
        Task<IOperationResult<List<ComplaintDTO>>> GetAllComplaintsForClient(int userId);
        Task<IOperationResult<ComplaintDTO>> GetComplaintCustomer(int complaintId, int customerId);
        Task<IOperationResult<ComplaintRDTO>> GetComplaintEmployee(int complaintId, int employeeId);
        Task<IOperationResult<ComplaintDTO>> GetComplaintAdmin(int complaintId);
        Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint, int customerId);
        Task<IOperationResult<bool>> DeleteComplaint(int complaintId);
        Task<IOperationResult<bool>> RespondToComplaint(int complaintId, int employeeId, string response);
        Task<IOperationResult<ComplaintRDTO[]>> GetAllComplaintsForRestaurant(int restaurantId);
        Task<IOperationResult<ComplaintDTO[]>> GetAllComplaintsForAdmin(int restaurantId);
        Task<IOperationResult<ComplaintRDTO[]>> GetAllComplaintsForRestaurantEmployee(int employeeId);
    }
}
