using RestaurantManagerApplication.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IComplaintService
    {
        Task<List<ComplaintModel>> GetComplaints(ISession callingUserSession);
        Task<bool> RespondToComplaint(int complaintId, string responseToComplaint, ISession callingUserSession);
    }
}
