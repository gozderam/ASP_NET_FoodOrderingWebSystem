using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace RestaurantManagerApplication.Abstracts
{
    public interface IMenuService
    {
        Task<bool> CreateMenuPosition(NewMenuPositionModel newPosition, ISession session);
        Task<MenuSectionModel[]> GetMenu(ISession session);
        Task<bool> AddMenuSection(MenuSectionModel newSection, ISession session);
        Task<bool> DeleteMenuPosition(int id, ISession session);
        Task<bool> EditMenuPosition(MenuPositionModel positionToEdit, ISession session);
        Task<bool> EditMenuSection(int sectionId, string newSectionName, ISession session);
        Task<bool> DeleteMenuSection(int id, ISession session);
    }
}
