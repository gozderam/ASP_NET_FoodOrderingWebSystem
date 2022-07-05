using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ClientApplication.Abstracts
{
    public interface IReviewService
    {
        public Task<bool> AddNewReview(NewReviewViewModel newReview, ISession session);
    }
}
