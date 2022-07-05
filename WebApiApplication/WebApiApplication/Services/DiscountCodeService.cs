using AutoMapper;
using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services.Results;

namespace WebApiApplication.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly FoodDeliveryDbContext dbContext;
        private readonly IMapper mapper;

        public DiscountCodeService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IOperationResult<int?>> AddDiscountCodeAdmin(NewDiscountCodeDTO newDiscountCode, int actorId)
        {
            if (String.IsNullOrEmpty(newDiscountCode.code) || newDiscountCode.percent < 1 || newDiscountCode.percent > 99 
                || newDiscountCode.dateFrom == null || newDiscountCode.dateTo == null) 
            {
                
                return new ServiceOperationResult<int?>(null, ResultCode.BadRequest, "Bad request");
            }

            var admin = await dbContext.Admins.FindAsync(actorId);
            if (admin == null)
                return new ServiceOperationResult<int?>(null, ResultCode.BadRequest, "Admin not found");

            var toAdd = mapper.Map<DiscountCode>(newDiscountCode);

            if (!toAdd.AppliesToAllRestaurants)
            {
                var restaurant = await dbContext.Restaurants.FindAsync(newDiscountCode.restaurantId);
                if (restaurant == null)
                    return new ServiceOperationResult<int?>(null, ResultCode.BadRequest, "Restuarant not found");
                toAdd.AppliedToRestaurant = restaurant;
            }

            await dbContext.DiscountCodes.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success);
        }

        public async Task<IOperationResult<int?>> AddDiscountCodeEmployee(NewDiscountCodeDTO newDiscountCode, int actorId)
        {
            if (String.IsNullOrEmpty(newDiscountCode.code) || newDiscountCode.percent < 1 || newDiscountCode.percent > 99 
                || newDiscountCode.dateFrom == null || newDiscountCode.dateTo == null || newDiscountCode.restaurantId == null)
            {
                return new ServiceOperationResult<int?>(null, ResultCode.BadRequest, "Bad request 123");
            }

            var employee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).FirstOrDefaultAsync(e => e.Id == actorId);
            if (employee == null)
                return new ServiceOperationResult<int?>(null, ResultCode.BadRequest, "Employee not found");

            newDiscountCode.restaurantId = employee.Restaurant.Id;

            var toAdd = mapper.Map<DiscountCode>(newDiscountCode);
            toAdd.AppliedToRestaurant = employee.Restaurant;

            await dbContext.DiscountCodes.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success);
        }

        public async Task<IOperationResult<DiscountCodeDTO>> GetDiscountCode(string code)
        {
            var discountCode = await dbContext.DiscountCodes.Include(d => d.AppliedToRestaurant).FirstOrDefaultAsync(d => d.Code == code);

            if (discountCode == null)
                return new ServiceOperationResult<DiscountCodeDTO>(null, ResultCode.NotFound, "Discount code not found");

            return new ServiceOperationResult<DiscountCodeDTO>(mapper.Map<DiscountCodeDTO>(discountCode), ResultCode.Success);
        }

        public async Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodes(int actorId)
        {
            var admin = await dbContext.Admins.FindAsync(actorId);
            if (admin == null)
                return new ServiceOperationResult<DiscountCodeDTO[]>(null, ResultCode.BadRequest, "Admin not found");

            var discountCodes = dbContext.DiscountCodes.Include(d => d.AppliedToRestaurant);
            DiscountCodeDTO[] ret = new DiscountCodeDTO[await discountCodes.CountAsync()];

            int i = 0;
            foreach(var d in discountCodes)
            {
                ret[i] = mapper.Map<DiscountCodeDTO>(d);
                i++;
            }
            return new ServiceOperationResult<DiscountCodeDTO[]>(ret, ResultCode.Success);
        }

        public async Task<IOperationResult<DiscountCodeDTO[]>> GetAllDiscountCodesEmployee(int actorId)
        {
            var employee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).Where(e => e.Id == actorId).FirstOrDefaultAsync();
            if (employee == null)
                return new ServiceOperationResult<DiscountCodeDTO[]>(null, ResultCode.BadRequest, "Employee not found");

            var discountCodes = await dbContext.DiscountCodes.Include(d => d.AppliedToRestaurant).Where(d => d.AppliesToAllRestaurants == false).
                Where(d => d.AppliedToRestaurant.Id == employee.Restaurant.Id).ToListAsync(); 
            DiscountCodeDTO[] ret = new DiscountCodeDTO[discountCodes.Count];

            int i = 0;
            foreach (var d in discountCodes)
            {
                ret[i] = mapper.Map<DiscountCodeDTO>(d);
                i++;
            }
            return new ServiceOperationResult<DiscountCodeDTO[]>(ret, ResultCode.Success);
        }

        public async Task<IOperationResult<bool>> DeleteDiscountCodeAdmin(int discountCodeId, int actorId)
        {
            var admin = await dbContext.Admins.FindAsync(actorId);
            if (admin == null)
                return new ServiceOperationResult<bool>(false, ResultCode.BadRequest, "Admin not found");

            var discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Id == discountCodeId);

            if (discountCode == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Discount code not found");

            dbContext.DiscountCodes.Remove(discountCode);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<bool>(true, ResultCode.Success);
        }

        public async Task<IOperationResult<bool>> DeleteDiscountCodeEmployee(int discountCodeId, int actorId)
        {
            var employee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).FirstOrDefaultAsync(e => e.Id == actorId);
            if (employee == null)
                return new ServiceOperationResult<bool>(false, ResultCode.BadRequest, "Employee not found");

            var discountCode = await dbContext.DiscountCodes.Include(d => d.AppliedToRestaurant).FirstOrDefaultAsync(d => d.Id == discountCodeId);
            if (discountCode == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Discount code not found");

            if (discountCode.AppliesToAllRestaurants)
                return new ServiceOperationResult<bool>(false, ResultCode.Unauthorized, "Access denied");

            if (discountCode.AppliedToRestaurant.Id != employee.Restaurant.Id)
                return new ServiceOperationResult<bool>(false, ResultCode.Unauthorized, "Access denied");

            dbContext.DiscountCodes.Remove(discountCode);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<bool>(true, ResultCode.Success);
        }
    }
}
