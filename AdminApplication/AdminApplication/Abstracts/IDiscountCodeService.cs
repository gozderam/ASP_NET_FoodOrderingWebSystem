using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminApplication.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace AdminApplication.Abstracts
{
    public interface IDiscountCodeService
    {
        Task<DiscountCodeModel[]> GetAllDiscountCodes(ISession callingUserSession);
        Task<bool> DeleteDiscountCode(int id, ISession callingUserSession);

        Task<bool> AddNewDiscountCode(NewDiscountCodeModel newDiscountCodeModel, ISession callingUserSession);
    }
}