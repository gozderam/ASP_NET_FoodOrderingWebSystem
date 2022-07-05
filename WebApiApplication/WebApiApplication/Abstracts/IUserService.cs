using Common.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database.POCO;

namespace WebApiApplication.Abstracts
{
    public interface IUserService
    {
        Task<IOperationResult<string>> LoginCustomer(string email);
        Task<IOperationResult<string>> LoginRestaurantEmployee(string email);
        Task<IOperationResult<string>> LoginAdmin(string email);
        Task<IOperationResult<bool>> DeleteUser(int id, UserTypes type);
        Task<IOperationResult<CustomerADTO[]>> GetAllClients();
        Task<IOperationResult<EmployeeDTO[]>> GetAllEmployees();
        Task<IOperationResult<AdministratorDTO[]>> GetAllAdmins();

        Task<IOperationResult<int?>> AddNewCustomer(NewCustomerDTO newCustomer);
        Task<IOperationResult<int?>> AddNewEmployee(NewEmployeeDTO newEmployee);
        Task<IOperationResult<int?>> AddNewAdmin(NewAdministratorDTO newAdmin);

        Task<IOperationResult<CustomerADTO>> GetCustomerA(int id);
        Task<IOperationResult<CustomerCDTO>> GetCustomerC(int id);
        Task<IOperationResult<EmployeeDTO>> GetEmployee(int id);
        Task<IOperationResult<AdministratorDTO>> GetAdministrator(int id, int requestUserId);
        Task<IOperationResult<OrderCDTO[]>> GetCustomerOrdersByCustomer(int id);
    }
}
