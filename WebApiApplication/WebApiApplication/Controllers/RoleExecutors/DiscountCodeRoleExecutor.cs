using Common.DTO;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Controllers.RoleExecutors
{
    public interface IDiscountCodeRoleExecutor
    {
        Task<IOperationResult<int?>> AddDiscountCode(IDiscountCodeService service, NewDiscountCodeDTO newDiscountCode, int actorId);
        Task<IOperationResult<bool>> DeleteDiscountCode(IDiscountCodeService service, int discountCodeId, int actorId);
        Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodes(IDiscountCodeService service, int actorId);
    }
    public class AdminDiscountCodeExecutor : IDiscountCodeRoleExecutor
    {
        public async Task<IOperationResult<int?>> AddDiscountCode(IDiscountCodeService service, NewDiscountCodeDTO newDiscountCode, int actorId)
            => (await service.AddDiscountCodeAdmin(newDiscountCode, actorId));

        public async Task<IOperationResult<bool>> DeleteDiscountCode(IDiscountCodeService service, int discountCodeId, int actorId)
            => (await service.DeleteDiscountCodeAdmin(discountCodeId, actorId));
        public async Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodes(IDiscountCodeService service, int actorId)
            => (await service.GetAllDiscountCodes(actorId));
    }

    public class EmployeeDiscountCodeExecutor : IDiscountCodeRoleExecutor
    {
        public async Task<IOperationResult<int?>> AddDiscountCode(IDiscountCodeService service, NewDiscountCodeDTO newDiscountCode, int actorId)
            => (await service.AddDiscountCodeEmployee(newDiscountCode, actorId));

        public async Task<IOperationResult<bool>> DeleteDiscountCode(IDiscountCodeService service, int discountCodeId, int actorId)
            => (await service.DeleteDiscountCodeEmployee(discountCodeId, actorId));
        public async Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodes(IDiscountCodeService service, int actorId)
            => (await service.GetAllDiscountCodesEmployee(actorId));
    }

}
