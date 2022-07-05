using Common.DTO;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;

namespace WebApiApplication.Abstracts
{
    public interface IDiscountCodeService
    {
        Task<IOperationResult<int?>> AddDiscountCodeAdmin(NewDiscountCodeDTO newDiscountCode, int actorId);
        Task<IOperationResult<int?>> AddDiscountCodeEmployee(NewDiscountCodeDTO newDiscountCode, int actorId);
        Task<IOperationResult<DiscountCodeDTO>> GetDiscountCode(string code);
        Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodes(int actorId);
        Task<IOperationResult<bool>> DeleteDiscountCodeAdmin(int discountCodeId, int actorId);
        Task<IOperationResult<bool>> DeleteDiscountCodeEmployee(int discountCodeId, int actorId);
        Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodesEmployee(int actorId);
    }
}
