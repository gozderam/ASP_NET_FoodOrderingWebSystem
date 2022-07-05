using AutoMapper;
using Common.DTO;
using Common.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services.Results;

namespace WebApiApplication.Services
{
    public class OrderService : IOrderService
    {
        private readonly FoodDeliveryDbContext dbContext;
        private readonly IMapper mapper;

        public OrderService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IOperationResult<int?>> AddOrder(NewOrderDTO newOrder)
        {

            var restaurant = await dbContext.Restaurants.FindAsync(newOrder.restaurantId);
            if (restaurant == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound, "Nie znaleziono restauracji");

            var author = await dbContext.Clients.Include(a => a.Address).FirstOrDefaultAsync(a => a.Id == newOrder.customerId);
            if (author == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound, "Nie znaleziono klienta");

            var toAdd = mapper.Map<Order>(newOrder);

            if (newOrder.address == null)
                toAdd.Address = author.Address;
            else
                toAdd.Address = mapper.Map<ClientAddress>(newOrder.address);

            double percent = 0.0;

            if (newOrder.discountcodeId.HasValue)
            {
                var discountCode = await dbContext.DiscountCodes.FindAsync(newOrder.discountcodeId);
                if (discountCode == null)
                    return new ServiceOperationResult<int?>(null, ResultCode.NotFound, "Nie znaleziono kodu promocyjnego");

                toAdd.DiscountCode = discountCode;

                if (DateTime.Compare(discountCode.DateTo, DateTime.Now) > 0)
                    percent = discountCode.Percent;
            }

            var menuPositions = dbContext.MenuPositions.Where(p => newOrder.positionsIds.Contains(p.Id)).ToList();

            Dictionary<int, double> prices = new Dictionary<int, double>();
            Dictionary<int, MenuPosition> idsToMenuPositions = new Dictionary<int, MenuPosition>();

            foreach (var menuPosition in menuPositions)
            {
                prices.Add(menuPosition.Id, menuPosition.Price);
                idsToMenuPositions.Add(menuPosition.Id, menuPosition);
            }

            toAdd.Restaurant = restaurant;
            toAdd.Client = author;

            List<MenuPosition> menuPositionsToAdd = new List<MenuPosition>();

            double originalPrice = 0;
            foreach (var menuPosition in newOrder.positionsIds)
            {
                menuPositionsToAdd.Add(idsToMenuPositions[menuPosition]);
                originalPrice += prices[menuPosition];
            }

            toAdd.OriginalPrice = originalPrice;
            toAdd.FinalPrice = (1-percent) * originalPrice;

            toAdd.OrdersMenuPositions = menuPositionsToAdd
                .GroupBy(mp => mp.Id)
                .Select(x => new Order_MenuPosition() { Order = toAdd, MenuPosition = x.ElementAt(0), PositionsInOrder = x.Count() })
                .ToList();
                                        
            toAdd.OrderState = OrderState.Unrealized;

            await dbContext.Orders.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success,
                "Zamówienie zostało wysłane");
        }

        public async Task<IOperationResult<OrderRDTO[]>> GetAllOrders(int employeeID)
        {

            var employee = await dbContext.RestaurantEmployees.Include(e=>e.Restaurant).FirstOrDefaultAsync(e => e.Id == employeeID);
            if (employee == null)
                return new ServiceOperationResult<OrderRDTO[]>(null, ResultCode.NotFound, "Pracownik o podanym identyfikatorze nie został znaleziony");

            if (employee.Restaurant == null)
                return new ServiceOperationResult<OrderRDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");

            var orders = await dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.DiscountCode)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdersMenuPositions).ThenInclude(omp => omp.MenuPosition)
                .Include(o => o.ResponsibleEmployee)
                .Where(o => o.Restaurant.Id == employee.Restaurant.Id).ToListAsync();
            return new ServiceOperationResult<OrderRDTO[]>(orders.ConvertAll(c=>mapper.Map<OrderRDTO>(c)).ToArray(), ResultCode.Success);
        }

        public async Task<IOperationResult<OrderADTO>> GetOrderA(int orderID)
        {
            var order = await dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.Client)
                .Include(o => o.Restaurant)
                .FirstAsync(o => o.Id == orderID);
            if (order == null)
                return new ServiceOperationResult<OrderADTO>(null, ResultCode.NotFound,
                    "Zamówienie o podanym identyfikatorze nie zostało znalezione");

            return new ServiceOperationResult<OrderADTO>(mapper.Map<OrderADTO>(order), ResultCode.Success);
        }

        public async Task<IOperationResult<OrderCDTO>> GetOrderC(int orderID)
        {
            var order = await dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdersMenuPositions).ThenInclude(omp => omp.MenuPosition)
                .FirstAsync(o => o.Id == orderID);
            if (order == null)
                return new ServiceOperationResult<OrderCDTO>(null, ResultCode.NotFound,
                    "Zamówienie o podanym identyfikatorze nie zostało znalezione");

            return new ServiceOperationResult<OrderCDTO>(mapper.Map<OrderCDTO>(order), ResultCode.Success);
        }

        public async Task<IOperationResult<OrderRDTO>> GetOrderR(int orderID)
        {
            var order = await dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.DiscountCode)
                .Include(o => o.Restaurant)
                .Include(o => o.OrdersMenuPositions).ThenInclude(omp => omp.MenuPosition)
                .Include(o => o.ResponsibleEmployee)
                .FirstAsync(o => o.Id == orderID);
            if (order == null)
                return new ServiceOperationResult<OrderRDTO>(null, ResultCode.NotFound,
                    "Zamówienie o podanym identyfikatorze nie zostało znalezione");

            return new ServiceOperationResult<OrderRDTO>(mapper.Map<OrderRDTO>(order), ResultCode.Success);
        }
        public async Task<IOperationResult<int>> RefuseOrder(int orderID)
        {
            var order = await dbContext.Orders.FirstAsync(o => o.Id == orderID);
            if (order == null)
                return new ServiceOperationResult<int>(-1, ResultCode.NotFound,
                    "Zamówienie o podanym identyfikatorze nie zostało znalezione");

            order.OrderState = OrderState.Cancelled;
            dbContext.SaveChanges();

            return new ServiceOperationResult<int>(order.Id, ResultCode.Success,
                    "Zamówienie zostało odrzucone");
        }

        public async Task<IOperationResult<int>> AcceptOrder(int orderID, int employeeID)
        {
            var order = await dbContext.Orders.FirstAsync(o => o.Id == orderID);
            if (order == null)
                return new ServiceOperationResult<int>(-1, ResultCode.NotFound,
                    "Zamówienie o podanym identyfikatorze nie zostało znalezione");

            order.OrderState = OrderState.Pending;
            
            var employee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).FirstAsync(e => e.Id == employeeID);
            if (order == null)
                return new ServiceOperationResult<int>(-1, ResultCode.NotFound,
                    "Nie znaleziono pracownika o tym id");

            order.ResponsibleEmployee = employee;

            dbContext.SaveChanges();

            return new ServiceOperationResult<int>(order.Id, ResultCode.Success,
                    "Zamówienie zostało odebrane");
        }

        public async Task<IOperationResult<int>> RealizeOrder(int orderID)
        {
            var order = await dbContext.Orders.FirstAsync(o => o.Id == orderID);
            if (order == null)
                return new ServiceOperationResult<int>(-1, ResultCode.NotFound,
                    "Zamówienie o podanym identyfikatorze nie zostało znalezione");

            order.OrderState = OrderState.Completed;
            dbContext.SaveChanges();

            return new ServiceOperationResult<int>(order.Id, ResultCode.Success,
                    "Zamówienie zostało oznaczone jako zrealizowane");
        }

        public async Task<IOperationResult<OrderADTO[]>> GetAllOrdersAdmin(int adminId)
        {
            var admin = await dbContext.Admins.FindAsync(adminId);
            if (admin == null)
                return new ServiceOperationResult<OrderADTO[]>(null, ResultCode.BadRequest, "Admin not found");

            var orders = dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.Client)
                .Include(o => o.Restaurant);

            OrderADTO[] ret = new OrderADTO[await orders.CountAsync()];
            int i = 0;
            foreach (var o in orders)
            {
                ret[i] = mapper.Map<OrderADTO>(o);
                i++;
            }

            return new ServiceOperationResult<OrderADTO[]>(ret, ResultCode.Success);
        }
    }
}
