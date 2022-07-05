using ClientApplication.Models;
using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientApplication.Abstracts
{
    public interface IComplaintService
    {
        Task<List<ComplaintModel>> GetComplaintsForUser(ISession callingUserSession);
        Task<bool> AddNewComplaint(NewComplaintViewModel newComplaint, ISession callingUserSession);
    }
}
