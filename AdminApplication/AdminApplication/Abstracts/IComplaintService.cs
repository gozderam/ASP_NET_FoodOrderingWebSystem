using AdminApplication.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminApplication.Abstracts
{
    public interface IComplaintService
    {
        Task<List<ComplaintModel>> GetComplaintsForUser(int userId, ISession callingUserSession);
        Task<List<ComplaintModel>> GetComplaintsForRestaurant(int restaurantId, ISession callingUserSession);

    }
}
