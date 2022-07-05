using AutoMapper;
using Common.DTO;
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
    public class ComplaintService : IComplaintService
    {
        private readonly FoodDeliveryDbContext dbContext;
        private readonly IMapper mapper;

        public ComplaintService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IOperationResult<List<ComplaintDTO>>> GetAllComplaintsForClient(int clientId)
        {
            if (await dbContext.Clients.FindAsync(clientId) == null)
                return new ServiceOperationResult<List<ComplaintDTO>>(null, ResultCode.NotFound, "Client with the given id not found.");
            var complaints = await dbContext.Complaints.Include(c => c.Client).Where(c => c.Client.Id == clientId).ToListAsync();
            return new ServiceOperationResult<List<ComplaintDTO>>(complaints.ConvertAll(c => mapper.Map<ComplaintDTO>(c)), ResultCode.Success);
        }

        public async Task<IOperationResult<ComplaintDTO>> GetComplaintCustomer(int complaintId, int customerId)
        {
            var ret = await dbContext.Complaints.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == complaintId);
            if (ret == null)
            {
                return new ServiceOperationResult<ComplaintDTO>(null, ResultCode.NotFound,
                    "Brak reklamacji o podanym numerze");
            }
            var currCustomer = await dbContext.Clients.FindAsync(customerId);
            if (currCustomer != null && ret.Client.Id == currCustomer.Id)
            {
                return new ServiceOperationResult<ComplaintDTO>(mapper.Map<ComplaintDTO>(ret), ResultCode.Success,
                    "Zwrócono reklamację");
            }
            else
            {
                return new ServiceOperationResult<ComplaintDTO>(null, ResultCode.Unauthorized,
                    "Brak uprawnień");
            }
        }

        public async Task<IOperationResult<ComplaintRDTO>> GetComplaintEmployee(int complaintId, int employeeId)
        {
            var ret = await dbContext.Complaints.Include(c => c.Order.Restaurant).Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == complaintId);
            if (ret == null)
            {
                return new ServiceOperationResult<ComplaintRDTO>(null, ResultCode.NotFound,
                    "Brak reklamacji o podanym numerze");
            }
            var currEmployee = await dbContext.RestaurantEmployees.Include(e => e.Restaurant).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (currEmployee!=null && ret.Order.Restaurant.Id == currEmployee.Restaurant.Id)
            {
                return new ServiceOperationResult<ComplaintRDTO>(mapper.Map<ComplaintRDTO>(ret), ResultCode.Success,
                    "Zwrócono reklamację");
            }
            else
            {
                return new ServiceOperationResult<ComplaintRDTO>(null, ResultCode.Unauthorized,
                    "Brak uprawnień");
            }
        }

        public async Task<IOperationResult<ComplaintDTO>> GetComplaintAdmin(int complaintId)
        {
            var ret = await dbContext.Complaints.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == complaintId);
            if(ret == null)
            {
                return new ServiceOperationResult<ComplaintDTO>(null, ResultCode.NotFound,
                    "Brak reklamacji o podanym numerze");
            }
            return new ServiceOperationResult<ComplaintDTO>(mapper.Map<ComplaintDTO>(ret), ResultCode.Success,
                    "Zwrócono reklamację");
        }

        public async Task<IOperationResult<int>> PostComplaint(NewComplaintDTO newComplaint, int customerId)
        {
            var toAdd = mapper.Map<Complaint>(newComplaint);

            var client = await dbContext.Clients.FindAsync(customerId);
            if (client == null)
            {
                return new ServiceOperationResult<int>(-1, ResultCode.NotFound,
                    "Brak klienta o danym identyfikatorze");
            }
            toAdd.Client = client;
            toAdd.IsOpened = true;

            var order = await dbContext.Orders.Include(o => o.Client).FirstOrDefaultAsync(o => o.Id == newComplaint.orderId);
            if (order == null)
            {
                return new ServiceOperationResult<int>(-1, ResultCode.NotFound,
                    "Brak zamówienia o danym identyfikatorze");
            }
            toAdd.Order = order;
            toAdd.OrderForeignKey = order.Id;
            if(order.Client.Id != customerId)
            {
                return new ServiceOperationResult<int>(-1, ResultCode.Unauthorized,
                    "Zamówienie nie zostało złożone przez podanego klienta");
            }
            await dbContext.Complaints.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int>(toAdd.Id, ResultCode.Success,
                "Dodano reklamację");
        }

        public async Task<IOperationResult<bool>> DeleteComplaint(int complaintId)
        {
            var toDelete = await dbContext.Complaints.FindAsync(complaintId);
            if(toDelete == null)
            {
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound,
                    "Brak reklamacji o podanym numerze");
            }
            dbContext.Complaints.Remove(toDelete);
            await dbContext.SaveChangesAsync();
            return new ServiceOperationResult<bool>(true, ResultCode.Success,
                "Podana reklamacja została usunięta");
        }

        public async Task<IOperationResult<bool>> RespondToComplaint(int complaintId, int employeeId, string response)
        {
            var ret = await dbContext.Complaints.Include(c => c.Order.Restaurant).FirstOrDefaultAsync(c => c.Id == complaintId);
            if (ret == null)
            {
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound,
                    "Brak reklamacji o podanym numerze");
            }
            var currEmployee = await dbContext.RestaurantEmployees.FindAsync(employeeId);
            if (currEmployee != null && ret.Order.Restaurant.Id == currEmployee.Restaurant.Id)
            {
                ret.Answer = response;
                ret.AttendingEmployee = currEmployee;
                await dbContext.SaveChangesAsync();
                return new ServiceOperationResult<bool>(true, ResultCode.Success,
                    "Dodano odpowiedź");
            }
            else
            {
                return new ServiceOperationResult<bool>(false, ResultCode.Unauthorized,
                    "Brak uprawnień");
            }
        }

        public async Task<IOperationResult<ComplaintRDTO[]>> GetAllComplaintsForRestaurant(int restaurantId)
        {
            if (await dbContext.Restaurants.FindAsync(restaurantId) == null)
                return new ServiceOperationResult<ComplaintRDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");
            var complaints = await dbContext.Complaints.Include(e => e.AttendingEmployee).Include(c => c.Order).Where(c => c.Order.Restaurant.Id == restaurantId).ToListAsync();
            return new ServiceOperationResult<ComplaintRDTO[]>(complaints.ConvertAll(c => mapper.Map<ComplaintRDTO>(c)).ToArray(), ResultCode.Success);
        }

        public async Task<IOperationResult<ComplaintRDTO[]>> GetAllComplaintsForRestaurantEmployee(int employeeId)
        {
            var employee = await dbContext.RestaurantEmployees.Include(r => r.Restaurant).Where(e => e.Id == employeeId).FirstOrDefaultAsync();
            if (employee == null)
                return new ServiceOperationResult<ComplaintRDTO[]>(null, ResultCode.NotFound, "Pracownik o podanym identyfikatorze nie został znaleziony");
            if (employee.Restaurant == null)
                return new ServiceOperationResult<ComplaintRDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");
            int restaurantId = employee.Restaurant.Id; var complaints = await dbContext.Complaints.Include(e => e.AttendingEmployee).Include(c => c.Order).Where(c => c.Order.Restaurant.Id == restaurantId).ToListAsync();
            return new ServiceOperationResult<ComplaintRDTO[]>(complaints.ConvertAll(c => mapper.Map<ComplaintRDTO>(c)).ToArray(), ResultCode.Success);
        }

        public async Task<IOperationResult<ComplaintDTO[]>> GetAllComplaintsForAdmin(int restaurantId)

        {
            if (await dbContext.Clients.FindAsync(restaurantId) == null)
                return new ServiceOperationResult<ComplaintDTO[]>(null, ResultCode.NotFound, "Restauracja o podanym identyfikatorze nie została znaleziona");
            var complaints = await dbContext.Complaints.Include(c => c.Client).Include(c => c.Order).Where(c => c.Order.Restaurant.Id == restaurantId).ToListAsync();
            return new ServiceOperationResult<ComplaintDTO[]>(complaints.ConvertAll(c => mapper.Map<ComplaintDTO>(c)).ToArray(), ResultCode.Success);
        }
    }
}
