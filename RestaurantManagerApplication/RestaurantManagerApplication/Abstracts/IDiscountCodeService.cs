using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using Microsoft.AspNetCore.Http;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IDiscountCodeService
    {
        Task<bool> AddDiscountCode(DiscountCodeModel dCode, ISession session);
        Task<DiscountCodeModel> GetDiscountCode(int id, ISession session);
        Task<bool> DeleteDiscountCode(int id, ISession session);
        Task<DiscountCodeModel[]> GetAllDiscountCodes(ISession session);
    }
}
