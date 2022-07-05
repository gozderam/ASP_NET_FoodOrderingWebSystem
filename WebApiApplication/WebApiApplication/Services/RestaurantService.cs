using AutoMapper;
using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services.Results;
using System.Collections.Generic;

namespace WebApiApplication.Services
{
    public class RestaurantService : IRestaurantService
    {
        private FoodDeliveryDbContext dbContext;
        private IMapper mapper;
        public RestaurantService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IOperationResult<RestaurantDTO>> GetRestaurant(int id)
        {
            var restaurant = await dbContext.Restaurants.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (restaurant == null)
            {
                return new ServiceOperationResult<RestaurantDTO>(null, ResultCode.NotFound, 
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            return new ServiceOperationResult<RestaurantDTO>(mapper.Map<RestaurantDTO>(restaurant), ResultCode.Success, 
                "Zwrócono szczegóły restauracji");
        }

        public async Task<IOperationResult<RestaurantCDTO>> GetRestaurantC(int id)
        {
            var restaurant = await dbContext.Restaurants.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (restaurant == null)
            {
                return new ServiceOperationResult<RestaurantCDTO>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            RestaurantCDTO res = mapper.Map<RestaurantCDTO>(restaurant);
            return new ServiceOperationResult<RestaurantCDTO>(mapper.Map<RestaurantCDTO>(restaurant), ResultCode.Success,
                "Zwrócono szczegóły restauracji");
        }

        public async Task<IOperationResult<RestaurantDTO>> GetRestaurantByEmployee(int employeeID)
        {
            var employee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).FirstOrDefaultAsync(c => c.Id == employeeID);
            if (employee == null)
            {
                return new ServiceOperationResult<RestaurantDTO>(null, ResultCode.NotFound,
                   "Pracownik o podanym identyfikatorze nie został znaleziony");
            }
            var restaurantID = employee.Restaurant.Id;
            var restaurant = await dbContext.Restaurants.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == restaurantID);
            if (restaurant == null)
            {
                return new ServiceOperationResult<RestaurantDTO>(null, ResultCode.NotFound,
                   "Restauracja o podanym identyfikatorze nie została znaleziona");
            }

            RestaurantDTO res = mapper.Map<RestaurantDTO>(restaurant);
            return new ServiceOperationResult<RestaurantDTO>(mapper.Map<RestaurantDTO>(restaurant), ResultCode.Success,
                "Zwrócono szczegóły restauracji");
        }

        public async Task<IOperationResult<RestaurantDTO[]>> GetAllRestaurants()
        {
            var restaurants = dbContext.Restaurants.Include(c => c.Address);

            if(restaurants.Count() == 0)
                return new ServiceOperationResult<RestaurantDTO[]>(null, ResultCode.NotFound,
                    "Pobranie listy wszystkich dostępnychrestauracji nie powiodło się");

            RestaurantDTO[] ret = new RestaurantDTO[await restaurants.CountAsync()];

            int i = 0;
            foreach (var r in restaurants)
            {
                ret[i] = mapper.Map<RestaurantDTO>(r);
                i++;
            }
            return new ServiceOperationResult<RestaurantDTO[]>(mapper.Map<RestaurantDTO[]>(restaurants), ResultCode.Success,
                "Zwrócono listę wszystkich dostępnych restauracji");
        }

        public async Task<IOperationResult<RestaurantCDTO[]>> GetAllRestaurantsC()
        {
            var restaurants = dbContext.Restaurants.Where(c => c.State == RestaurantState.Active).Include(c => c.Address);

            if (restaurants.Count() == 0)
                return new ServiceOperationResult<RestaurantCDTO[]>(null, ResultCode.NotFound,
                     "Pobranie listy wszystkich dostępnychrestauracji nie powiodło się");

            RestaurantCDTO[] ret = new RestaurantCDTO[await restaurants.CountAsync()];

            int i = 0;
            foreach (var r in restaurants)
            {
                ret[i] = mapper.Map<RestaurantCDTO>(r);
                i++;
            }
            return new ServiceOperationResult<RestaurantCDTO[]>(mapper.Map<RestaurantCDTO[]>(restaurants), ResultCode.Success,
                "Zwrócono listę wszystkich dostępnych restauracji");
        }

        public async Task<IOperationResult<int?>> AddNewRestaurant(NewRestaurantDTO newRestaurant, int restaurateurID)
        {
            var toAdd = mapper.Map<Restaurant>(newRestaurant);
            toAdd.DateOfJoining = System.DateTime.Now;

            var restaurateur = await dbContext.RestaurantEmployees.FindAsync(restaurateurID);
            restaurateur.Restaurant = toAdd;

            await dbContext.RestaurantAddresses.AddAsync(toAdd.Address);
            await dbContext.Restaurants.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success,
                 "Restauracja została pomyślnie dodana do listy nieaktywnych restauracji");
        }

        public async Task<IOperationResult<int?>> AddRestaurantToFavourites(int customerId, int restaurantID)
        {
            var toAdd = await dbContext.Restaurants.FindAsync(restaurantID);
            if (toAdd == null)
            {
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            var cus = await dbContext.Clients.Include(c => c.FavouriteRestaurants).FirstOrDefaultAsync(c => c.Id == customerId);
            if (cus == null)
            {
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
                     "Klient o podanym identyfikatorze nie został znaleziony");
            }
            if (cus.FavouriteRestaurants == null)
                cus.FavouriteRestaurants = new List<Restaurant>();
            cus.FavouriteRestaurants.Add(toAdd);
            if (toAdd.FavouriteForClients == null)
                toAdd.FavouriteForClients = new List<Client>();
            toAdd.FavouriteForClients.Add(cus);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success,
                 "Restauracja została dodana do ulubionych");
        }

        public async Task<IOperationResult<bool>> DeleteRestaurantAdmin(int id)
        {
            var res = await dbContext.Restaurants
               .FirstOrDefaultAsync(r => r.Id == id);

            if (res == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");

            foreach (var x in dbContext.Orders.Where(o => o.Restaurant.Id == res.Id)){
                dbContext.Orders.Remove(x);
            }
            foreach (var x in dbContext.DiscountCodes.Where(d => d.AppliedToRestaurant.Id == res.Id))
            {
                dbContext.DiscountCodes.Remove(x);
            }
            foreach (var x in dbContext.RestaurantEmployees.Where(e => e.Restaurant.Id == res.Id))
            {
                dbContext.RestaurantEmployees.Remove(x);
            }

            dbContext.Restaurants.Remove(res);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<bool>(true, ResultCode.Success, "Restauracja została usunięta");
        }

        public async Task<IOperationResult<bool>> DeleteRestaurantRestaurateur(int restaurantID, int restaurateurID)
        {
            var restaurateur = await dbContext.RestaurantEmployees.Include(r => r.Restaurant).FirstOrDefaultAsync(r => r.Id == restaurateurID);
            if (restaurateur == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Restaurator o podanym identyfikatorze nie został znaleziony");
            var res = restaurateur.Restaurant;

            if (res == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");

            foreach (var x in dbContext.Orders.Where(o => o.Restaurant.Id == res.Id))
            {
                dbContext.Orders.Remove(x);
            }
            foreach (var x in dbContext.DiscountCodes.Where(d => d.AppliedToRestaurant.Id == res.Id))
            {
                dbContext.DiscountCodes.Remove(x);
            }
            foreach (var x in dbContext.RestaurantEmployees.Where(e => e.Restaurant.Id == res.Id))
            {
                dbContext.RestaurantEmployees.Remove(x);
            }

            dbContext.Restaurants.Remove(res);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<bool>(true, ResultCode.Success, "Restauracja została usunięta");
        }

        public async Task<IOperationResult<RestaurantState?>> ChangeRestaurantState(int id, RestaurantState state)
        {
            var restaurantToChange = await dbContext.Restaurants.FindAsync(id);
            if (restaurantToChange == null)
            {
                return new ServiceOperationResult<RestaurantState?>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            restaurantToChange.State = state;
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<RestaurantState?>(state, ResultCode.Success,
                "Zmieniono stan restauracji");

        }

        public async Task<IOperationResult<RestaurantState?>> RestaurantDeactivate(int id)
        {
            var restaurateur = await dbContext.RestaurantEmployees.Include(r => r.Restaurant).FirstOrDefaultAsync(r => r.Id == id);
            var restaurant = restaurateur.Restaurant;
            if (restaurant == null)
            {
                return new ServiceOperationResult<RestaurantState?>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            restaurant.State = RestaurantState.Deactivated;
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<RestaurantState?>(RestaurantState.Deactivated, ResultCode.Success,
                "Zmieniono stan restauracji");

        }

        public async Task<IOperationResult<RestaurantState?>> RestaurantReactivate(int id)
        {
            var restaurateur = await dbContext.RestaurantEmployees.Include(r => r.Restaurant).FirstOrDefaultAsync(r => r.Id == id);
            var restaurant = restaurateur.Restaurant;
            if (restaurant == null)
            {
                return new ServiceOperationResult<RestaurantState?>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            restaurant.State = RestaurantState.Active;
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<RestaurantState?>(RestaurantState.Active, ResultCode.Success,
                "Zmieniono stan restauracji");

        }

        public async Task<IOperationResult<OrderRDTO[]>> GetAllOrders(int id)
        {
            var restaurateur = await dbContext.RestaurantEmployees
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == id);
            var restaurant = restaurateur.Restaurant;
            if (restaurant == null)
            {
                return new ServiceOperationResult<OrderRDTO[]>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            var orders = await dbContext.Orders
                .Where(o => o.Restaurant == restaurant)
                .Include(o => o.Address)
                .Include(o => o.DiscountCode)
                .Include(o => o.OrdersMenuPositions)
                .ThenInclude(mp => mp.MenuPosition)
                .Include(o => o.ResponsibleEmployee)
                .ToArrayAsync();
            var ret = new List<OrderRDTO>();
            foreach(var o in orders)
            {
                ret.Add(mapper.Map<OrderRDTO>(o));
            }
            return new ServiceOperationResult<OrderRDTO[]>(ret.ToArray(), ResultCode.Success,
                "Zwrócono zamówienia restauracji");
        }
    }
}
