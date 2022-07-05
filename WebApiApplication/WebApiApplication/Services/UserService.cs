using AutoMapper;
using Common.DTO;
using Common.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services.Results;
using static Common.Definitions;

namespace WebApiApplication.Services
{
    public class UserService : IUserService
    {
        private FoodDeliveryDbContext dbContext;
        private IMapper mapper;
        public UserService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IOperationResult<string>> LoginCustomer(string email)
        {
            var client = await dbContext.Clients.FirstOrDefaultAsync(c => c.Email == email);
            if(client==null)
                 return new ServiceOperationResult<string>(null, ResultCode.NotFound, "Customer not found in the database");
            return new ServiceOperationResult<string>(
                    ApiKeyConverter.GetApiKey(client.Id, Role.Customer), 
                    ResultCode.Success);
        }

        public async Task<IOperationResult<string>> LoginRestaurantEmployee(string email)
        {
            var empoyee = await dbContext.RestaurantEmployees.FirstOrDefaultAsync(c => c.Email == email);
            if (empoyee == null)
                return new ServiceOperationResult<string>(null, ResultCode.NotFound, "Restaurant employee not found in the database");
            return new ServiceOperationResult<string>(
                    ApiKeyConverter.GetApiKey(empoyee.Id, empoyee.IsRestaurateur ? Role.Restaurateur : Role.Employee),
                    ResultCode.Success);
        }

        public async Task<IOperationResult<string>> LoginAdmin(string email)
        {
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin == null)
                return new ServiceOperationResult<string>(null, ResultCode.NotFound, "Admin not found in the database");
            return new ServiceOperationResult<string>(
                    ApiKeyConverter.GetApiKey(admin.Id, Role.Admin),
                    ResultCode.Success);
        }

        public async Task<IOperationResult<CustomerADTO[]>> GetAllClients()
        {
            var Clients = dbContext.Clients.Include(c => c.Address);

            CustomerADTO[] ret = new CustomerADTO[await Clients.CountAsync()];
            
            int i = 0;
            foreach (var c in Clients)
            {
                ret[i] = mapper.Map<CustomerADTO>(c);
                i++;
            }
            return new ServiceOperationResult<CustomerADTO[]>(ret, ResultCode.Success);
        }

        public async Task<IOperationResult<EmployeeDTO[]>> GetAllEmployees()
        {
            var Employees = dbContext.RestaurantEmployees
                .Include(e => e.Restaurant)
                .ThenInclude(r => r.Address);

            EmployeeDTO[] ret = new EmployeeDTO[await Employees.CountAsync()];

            int i = 0;
            foreach (var e in Employees)
            {
                ret[i] = mapper.Map<EmployeeDTO>(e);
                i++;
            }
            return new ServiceOperationResult<EmployeeDTO[]>(ret, ResultCode.Success);
        }
        public async Task<IOperationResult<AdministratorDTO[]>> GetAllAdmins()
        {
            var Admins = dbContext.Admins;

            AdministratorDTO[] ret = new AdministratorDTO[await Admins.CountAsync()];

            int i = 0;
            foreach (var a in Admins)
            {
                ret[i] = mapper.Map<AdministratorDTO>(a);
                i++;
            }
            return new ServiceOperationResult<AdministratorDTO[]>(ret, ResultCode.Success);
        }

        public async Task<IOperationResult<bool>> DeleteUser(int id, UserTypes type)
        {
            switch (type)
            {
                case UserTypes.Client:
                    var ClientToDelete = await dbContext.Clients.FindAsync(id);
                    if (ClientToDelete == null)
                        return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Client not found");
                    dbContext.Clients.Remove(ClientToDelete);
                    dbContext.SaveChanges();
                    break;
                case UserTypes.Employee:
                    var EmployeeToDelete = await dbContext.RestaurantEmployees.FindAsync(id);
                    if (EmployeeToDelete == null)
                        return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Restaurant employee not found");
                    dbContext.RestaurantEmployees.Remove(EmployeeToDelete);
                    dbContext.SaveChanges();
                    break;
                case UserTypes.Admin:
                    var AdminToDelete = await dbContext.Admins.FindAsync(id);
                    if (AdminToDelete == null)
                        return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Admin not found");
                    dbContext.Admins.Remove(AdminToDelete);
                    dbContext.SaveChanges();
                    break;
            }

            return new ServiceOperationResult<bool>(true, ResultCode.Success); 
        }

        public async Task<IOperationResult<int?>> AddNewCustomer(NewCustomerDTO newCustomer)
        {
            var toAdd = mapper.Map<Client>(newCustomer);
            if (toAdd.Address != null)
                await dbContext.ClientAddresses.AddAsync(toAdd.Address);
 
            await dbContext.Clients.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success); 
        }

        public async Task<IOperationResult<int?>> AddNewEmployee(NewEmployeeDTO newEmployee)
        {
            RestaurantEmployee toAdd = mapper.Map<RestaurantEmployee>(newEmployee);
            if(newEmployee.restaurantId != null)
            {
                toAdd.Restaurant = dbContext.Restaurants.Find(newEmployee.restaurantId);
            }
        
            await dbContext.RestaurantEmployees.AddAsync(toAdd);
            await dbContext.SaveChangesAsync(); 

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success);
        }

        public async Task<IOperationResult<int?>> AddNewAdmin(NewAdministratorDTO newAdmin)
        {
            Admin toAdd = mapper.Map<Admin>(newAdmin);
            await dbContext.Admins.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success);
        }

        public async Task<IOperationResult<CustomerADTO>> GetCustomerA(int id)
        {
            var client = await dbContext.Clients.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
                return new ServiceOperationResult<CustomerADTO>(null, ResultCode.NotFound, "Client not found");

            return new ServiceOperationResult<CustomerADTO>(mapper.Map<CustomerADTO>(client), ResultCode.Success); 
        }

        public async Task<IOperationResult<CustomerCDTO>> GetCustomerC(int id)
        {
            var client = await dbContext.Clients.Include(c => c.Address).Include(c => c.FavouriteRestaurants).ThenInclude(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
                return new ServiceOperationResult<CustomerCDTO>(null, ResultCode.NotFound, "Client not found");

            return new ServiceOperationResult<CustomerCDTO>(mapper.Map<CustomerCDTO>(client), ResultCode.Success);
        }

        public async Task<IOperationResult<EmployeeDTO>> GetEmployee(int id)
        {
            var employee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).ThenInclude(e => e.Address).FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
                return new ServiceOperationResult<EmployeeDTO>(null, ResultCode.NotFound, "Restaurant employee not found");

            return new ServiceOperationResult<EmployeeDTO>(mapper.Map<EmployeeDTO>(employee), ResultCode.Success);  
        }

        public async Task<IOperationResult<AdministratorDTO>> GetAdministrator(int id, int requestUserId)
        {
            if (id == -1)
                id = requestUserId;
            var admin = await dbContext.Admins.FindAsync(id);
            if (admin == null)
                return new ServiceOperationResult<AdministratorDTO>(null, ResultCode.NotFound, "Admin not found");

            return new ServiceOperationResult<AdministratorDTO>(mapper.Map<AdministratorDTO>(admin), ResultCode.Success);
        }

        public async Task<IOperationResult<OrderCDTO[]>> GetCustomerOrdersByCustomer(int id)
        {
            var client = await dbContext.Clients.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);

            if(client == null)
                return new ServiceOperationResult<OrderCDTO[]>(null, ResultCode.NotFound,
                "Klient o podanym identyfikatorze nie został znaleziony");

            var orders = dbContext.Orders
                .Include(c => c.Address)
                .Include(c => c.Restaurant.Address)
                .Include(c => c.OrdersMenuPositions).ThenInclude(omp => omp.MenuPosition)
                .Where(c => c.Client.Id == id);
            OrderCDTO[] result = new OrderCDTO[orders.Count()];
            int i = 0;
            foreach(var order in orders)
            {
                result[i] = mapper.Map<OrderCDTO>(order);
                i++;
            }
            return new ServiceOperationResult<OrderCDTO[]>(result, ResultCode.Success, 
                "Zwrócono listę wszystkich zamówień złożonych przez klienta");
        }
    }
}
