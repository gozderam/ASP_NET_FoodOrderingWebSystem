using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{
    public interface IReviewRoleExecutor
    {
        Task<IOperationResult<object>> GetReview(IReviewService service, int reviewId);
    }

    public class AdminReviewExecutor : IReviewRoleExecutor
    {
        public async Task<IOperationResult<object>> GetReview(IReviewService service, int reviewId)
            => (await service.GetReview(reviewId)).GetInstanceWithObjectData();

    }

    public class RestaurateurReviewExecutor : IReviewRoleExecutor
    {
        public async Task<IOperationResult<object>> GetReview(IReviewService service, int reviewId)
            => (await service.GetReviewR(reviewId)).GetInstanceWithObjectData();
    }

    public class EmployeeReviewExecutor : IReviewRoleExecutor
    {
        public async Task<IOperationResult<object>> GetReview(IReviewService service, int reviewId)
            => (await service.GetReviewR(reviewId)).GetInstanceWithObjectData();
    }


    public class CustomerReviewExecutor : IReviewRoleExecutor
    {
        public async Task<IOperationResult<object>> GetReview(IReviewService service, int reviewId)
            => (await service.GetReviewR(reviewId)).GetInstanceWithObjectData();
    }
}
