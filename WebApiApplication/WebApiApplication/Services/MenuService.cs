using AutoMapper;
using Common;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplication.Abstracts;
using WebApiApplication.Authorization;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using Common.DTO;
using WebApiApplication.Exceptions;
using static Common.Definitions;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Services.Results;

namespace WebApiApplication.Services
{
    public class MenuService : IMenuService
    {
        private FoodDeliveryDbContext dbContext;
        private IMapper mapper;
        public MenuService(FoodDeliveryDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        public async Task<IOperationResult<SectionDTO[]>> GetMenu(int id)
        {
            var restaurant = await dbContext.Restaurants.Include(c => c.MenuSections).FirstOrDefaultAsync(c => c.Id == id);

            if (restaurant == null)
            {
                return new ServiceOperationResult<SectionDTO[]>(null, ResultCode.NotFound,
                    "Restauracja o podanym identyfikatorze nie została znaleziona");
            }
            var sections = restaurant.MenuSections;

            SectionDTO[] ret = new SectionDTO[sections.Count()];
            int i = 0;
            foreach (var s in sections)
            {
                ret[i] = mapper.Map<SectionDTO>(s);
                var pos = await dbContext.MenuPositions.Where(m => m.MenuSection.Id == sections[i].Id).ToListAsync();
                ret[i].positions = mapper.Map<PositionFromMenuDTO[]>(pos);
                i++;
            }
            return new ServiceOperationResult<SectionDTO[]>(ret, ResultCode.Success,
                "Zwrócono menu wybranej restauracji");
        }

        public async Task<IOperationResult<SectionDTO[]>> GetMenuByEmployee(int employeeID)
        {
            var employee = await dbContext.RestaurantEmployees.Include(c => c.Restaurant.MenuSections).FirstOrDefaultAsync(c => c.Id == employeeID);
            if (employee == null)
            {
                return new ServiceOperationResult<SectionDTO[]>(null, ResultCode.BadRequest,
                    "Pobranie menu restauracji nie powiodło się");
            }
            SectionDTO[] ret = new SectionDTO[employee.Restaurant.MenuSections.Count()];

            int i = 0;
            foreach (var s in employee.Restaurant.MenuSections)
            {
                ret[i] = mapper.Map<SectionDTO>(s);
                var pos = await dbContext.MenuPositions.Where(m => m.MenuSection.Id == employee.Restaurant.MenuSections[i].Id).ToListAsync();
                ret[i].positions = mapper.Map<PositionFromMenuDTO[]>(pos);
                i++;
            }
            return new ServiceOperationResult<SectionDTO[]>(ret, ResultCode.Success,
                "Zwrócono menu wybranej restauracji");
        }
        public async Task<IOperationResult<PositionFromMenuDTO>> GetMenuPosition(int id)
        {
            var menuPosition = await dbContext.MenuPositions.FirstOrDefaultAsync(c => c.Id == id);
            if (menuPosition == null)
            {
                return new ServiceOperationResult<PositionFromMenuDTO>(null, ResultCode.NotFound,
                    "Pozycja z menu o podanym identyfikatorze nie została znaleziona");
            }
            return new ServiceOperationResult<PositionFromMenuDTO>(mapper.Map<PositionFromMenuDTO>(menuPosition), ResultCode.Success,
                "Zwrócono pozycje z menu");
        }
        public async Task<IOperationResult<int?>> EditMenuPosition(int menuPositionID, NewPositionFromMenuDTO editedPosition)
        {
            var menuPositionUpdated = mapper.Map<MenuPosition>(editedPosition);
            var menuPositonToEdit = await dbContext.MenuPositions.FindAsync(menuPositionID);
            if (menuPositonToEdit == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound, "Pozycja o podanym identyfikatorze nie została znaleziona");

            menuPositonToEdit.Name = editedPosition.name;
            menuPositonToEdit.Price = editedPosition.price;
            menuPositonToEdit.Description = editedPosition.description;
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(menuPositonToEdit.Id, ResultCode.Success,
                 "Pozycja została zaktualizowana");
        }
        public async Task<IOperationResult<bool>> DeleteMenuPosition(int menuPositionID)
        {
            var menuPositonToDelete = await dbContext.MenuPositions.FindAsync(menuPositionID);
            if (menuPositonToDelete == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound, "Pozycja o podanym identyfikatorze nie została znaleziona");

            dbContext.MenuPositions.Remove(menuPositonToDelete);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<bool>(true, ResultCode.Success,
                 "Pozycja została usunięta");
        }
        public async Task<IOperationResult<int?>> AddNewMenuPosition(int sectionID, NewPositionFromMenuDTO newPosition)
        {
            var toAdd = mapper.Map<MenuPosition>(newPosition);

            var section = await dbContext.MenuSections.FirstOrDefaultAsync(c => c.Id == sectionID);
            if (section == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
              "Sekcja o podanym identyfikatorze nie została znaleziona");

            toAdd.MenuSection = section;

            await dbContext.MenuPositions.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success,
                 "Pozycja została pomyślnie dodana do menu restauracji");
        }

        public async Task<IOperationResult<int?>> AddNewMenuSection(int id, string name)
        {
            var restaurateur = await dbContext.RestaurantEmployees.Include(c => c.Restaurant).FirstOrDefaultAsync(c => c.Id == id);

            if (restaurateur == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
              "Dodanie sekcji nie powiodło się");

            if (restaurateur.Restaurant == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
              "Dodanie sekcji nie powiodło się");

            MenuSection toAdd = new MenuSection() { Name = name , Restaurant = restaurateur.Restaurant ,};

            toAdd.Restaurant = restaurateur.Restaurant;

            await dbContext.MenuSections.AddAsync(toAdd);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(toAdd.Id, ResultCode.Success,
                 "Sekcja została pomyślnie dodana do menu restauracji");
        }

        public async Task<IOperationResult<int?>> ChangeMenuSectionName(int id, string name)
        {
            var section = await dbContext.MenuSections.Include(s=>s.Restaurant).Where(s=>s.Id == id).FirstOrDefaultAsync();

            if (section == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
              "Sekcja o podanym identyfikatorze nie została znaleziona");

            if (section.Restaurant == null)
                return new ServiceOperationResult<int?>(null, ResultCode.NotFound,
              "Sekcja o podanym identyfikatorze nie została znaleziona");

            section.Name = name;
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<int?>(id, ResultCode.Success,
                 "Nazwa sekcji została zmieniona");
        }

        public async Task<IOperationResult<bool>> DeleteMenuSection(int id)
        {
            var section = await dbContext.MenuSections.FindAsync(id);

            if (section == null)
                return new ServiceOperationResult<bool>(false, ResultCode.NotFound,
              "Sekcja o podanym identyfikatorze nie została znaleziona");

            dbContext.Remove(section);
            await dbContext.SaveChangesAsync();

            return new ServiceOperationResult<bool>(true, ResultCode.Success,
                 "Sekcja została usunięta");
        }
    }
}
