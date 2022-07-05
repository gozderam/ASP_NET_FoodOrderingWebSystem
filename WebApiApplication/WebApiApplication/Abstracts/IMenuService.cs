using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database.POCO;
using Common.DTO;

namespace WebApiApplication.Abstracts
{
    public interface IMenuService
    {
        Task<IOperationResult<SectionDTO[]>> GetMenu(int id);
        Task<IOperationResult<SectionDTO[]>> GetMenuByEmployee(int employeeID);
        Task<IOperationResult<int?>> AddNewMenuPosition(int sectionID, NewPositionFromMenuDTO newPosition);
        Task<IOperationResult<int?>> AddNewMenuSection(int id, string name);
        Task<IOperationResult<int?>> EditMenuPosition(int menuPositionID, NewPositionFromMenuDTO newPosition);
        Task<IOperationResult<bool>> DeleteMenuPosition(int menuPositionID);
        Task<IOperationResult<int?>> ChangeMenuSectionName(int id, string name);
        Task<IOperationResult<bool>> DeleteMenuSection(int id);
    }
}
