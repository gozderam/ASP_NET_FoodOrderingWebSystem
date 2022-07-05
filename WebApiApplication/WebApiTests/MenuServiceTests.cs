using Microsoft.EntityFrameworkCore;
using System;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using Common.DTO;
using WebApiApplication.Services;
using WebApiTests.Base;
using Xunit;
using System.Collections.Generic;

namespace WebApiTests
{
    public class MenuServiceTests : BaseTests, IDisposable
    {
        #region seeding and disposeing
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptions { get; }
        private Restaurant inDbRestaurant;
        private readonly int notInDbRestaurantId = -2;

        private RestaurantEmployee noRestaurantEmployee;

        private RestaurantEmployee inDbEmployee;
        private readonly int notInDbEmployeeId = -2;

        private MenuPosition inDbMenuPosition;
        private readonly int notinDbMenuPositionId = -2;

        private MenuSection inDbMenuSection;
        private readonly int notinDbMenuSectionId = -2;


        public MenuServiceTests()
        {
            ContextOptions = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            Seed();
        }

        private void Seed()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);

            // recreating in-memory database
            recreateContext.Database.EnsureCreated();
            using var context = new FoodDeliveryDbContext(ContextOptions);

            inDbMenuPosition = new MenuPosition()
            {
                Name = "Kurczak",
                Description = "Danie z kurczaka",
                Price = 9.99,
            };

            

            inDbRestaurant = new Restaurant()
            {
                Name = "ReviewServiceTests_Restaurant",
                Address = new RestaurantAddress(),

            };

            inDbEmployee = new RestaurantEmployee()
            {
                Name = "Adam",
                IsRestaurateur = true,
                Restaurant = inDbRestaurant,
            };

            inDbMenuSection = new MenuSection()
            {
                Name = "Dania z kurczaka",
                MenuPositions = new List<MenuPosition> { inDbMenuPosition },
                Restaurant = inDbRestaurant,
            };
            noRestaurantEmployee = new RestaurantEmployee()
            {
                Name = "alan",
            };

            inDbMenuPosition.MenuSection = inDbMenuSection;

            inDbRestaurant.MenuSections = new List<MenuSection> { inDbMenuSection };
            context.MenuSections.Add(inDbMenuSection);
            context.RestaurantEmployees.Add(noRestaurantEmployee);
            context.MenuPositions.Add(inDbMenuPosition);
            
            context.Restaurants.Add(inDbRestaurant);
            context.RestaurantEmployees.Add(inDbEmployee);
            context.SaveChanges();
        }

        public void Dispose()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            recreateContext.Database.EnsureDeleted();
        }
        #endregion

        #region Add menu test
        [Fact]
        public async void AddMenuSectionAndPosition_Success()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            SectionDTO menusec = new SectionDTO()
            {
                name = "section1",
            };
            NewPositionFromMenuDTO menupos = new NewPositionFromMenuDTO()
            {
                name = "naame",
                description = "description",
            };

            var serviceResult = await menuService.AddNewMenuSection(inDbEmployee.Id, menusec.name);
            AssertSuccessServiceResult(serviceResult);
            int addedMenuSectionId = (int)serviceResult.Data;

            serviceResult = await menuService.AddNewMenuPosition(addedMenuSectionId, menupos);
            AssertSuccessServiceResult(serviceResult);
            int addedMenuPositionId = (int)serviceResult.Data;
            // retrieving
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var foundMenuSection = await getContext.MenuSections.FindAsync(addedMenuSectionId);
            Assert.NotNull(foundMenuSection);
            Assert.Equal(menusec.name, foundMenuSection.Name);   
            
            var foundMenuPosition = await getContext.MenuPositions.FindAsync(addedMenuPositionId);
            Assert.NotNull(foundMenuPosition);
            Assert.Equal(menupos.name, foundMenuPosition.Name);
            Assert.Equal(menupos.description, foundMenuPosition.Description);
        }
        [Fact]
        public async void AddMenuSection_RestaurantNotInDb()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            SectionDTO menusec = new SectionDTO()
            {
                name = "section1",
            };

            RestaurantEmployee noRestaurantEmployee = new RestaurantEmployee()
            {
                Name = "alan",
            };
            var serviceResult = await menuService.AddNewMenuSection(notInDbEmployeeId, menusec.name);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        [Fact]
        public async void AddMenuPosition_EmployeeNotInDb()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            NewPositionFromMenuDTO menusec = new NewPositionFromMenuDTO()
            {
                name = "pos1",
            };

            var serviceResult = await menuService.AddNewMenuPosition(notinDbMenuSectionId, menusec);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        #endregion

        #region get menu tests
        [Fact]
        public async void GetMenu_Success()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.GetMenu(inDbRestaurant.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedMenu = serviceResult.Data;
            Assert.Equal(inDbMenuSection.Name, receivedMenu[0].name);
            Assert.Equal(inDbMenuSection.MenuPositions.Count, receivedMenu[0].positions.Length);
            Assert.Equal(inDbMenuSection.MenuPositions[0].Name, receivedMenu[0].positions[0].name);
        }
        [Fact]
        public async void GetMenuByEmployeeTest()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.GetMenuByEmployee(inDbEmployee.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedMenu = serviceResult.Data;
            Assert.Equal(inDbMenuSection.Name, receivedMenu[0].name);
            Assert.Equal(inDbMenuSection.MenuPositions.Count, receivedMenu[0].positions.Length);
            Assert.Equal(inDbMenuSection.MenuPositions[0].Name, receivedMenu[0].positions[0].name);

            serviceResult = await menuService.GetMenuByEmployee(notInDbEmployeeId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.BadRequest, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        [Fact]
        public async void GetMenu_NoRestaurnat()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.GetMenu(notInDbRestaurantId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void GetMenuPosition_Success()
        {
            
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.GetMenuPosition(inDbMenuPosition.Id);
            AssertSuccessServiceResult(serviceResult);
            var receivedMenuPos = serviceResult.Data;
            Assert.Equal(inDbMenuPosition.Name, receivedMenuPos.name);
            Assert.Equal(inDbMenuPosition.Description, receivedMenuPos.description);
        }

        [Fact]
        public async void GetMenuPosition_NotFound()
        {
   
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.GetMenuPosition(notinDbMenuPositionId);
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region Edit menu tests
        [Fact]
        public async void EditMenu_Success()
        {
            // adding 
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            NewPositionFromMenuDTO modelEdit = new NewPositionFromMenuDTO() 
            { 
                name ="Warzywo",
                description="bbb",    
                price = 33.3,
            };
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.EditMenuPosition(inDbMenuPosition.Id, modelEdit);
            AssertSuccessServiceResult(serviceResult);
            var afterEdit = await addContext.MenuPositions.FindAsync(inDbMenuPosition.Id);
            Assert.Equal(afterEdit.Name, modelEdit.name);
            Assert.Equal(afterEdit.Description, modelEdit.description);
            Assert.Equal(afterEdit.Price, modelEdit.price);
        }

        [Fact]
        public async void EditMenu_NotFound()
        {
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.EditMenuPosition(notinDbMenuPositionId,new NewPositionFromMenuDTO());
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }

        [Fact]
        public async void EditSection_Success()
        {            
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var a = await addContext.MenuSections.FindAsync(inDbMenuSection.Id);
            a.Restaurant = new Restaurant();
            addContext.SaveChanges();
            string newName = "Asas";
            var menuService = new MenuService(addContext, Mapper);
            var serviceResult = await menuService.ChangeMenuSectionName(inDbMenuSection.Id, newName);
            AssertSuccessServiceResult(serviceResult);
            var afterEdit = await addContext.MenuSections.FindAsync(inDbMenuSection.Id);
            Assert.Equal(afterEdit.Name, newName);
        }

        [Fact]
        public async void EditSection_NotFound()
        {
            using var addContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(addContext, Mapper);
            string newName = "Asas";
            var serviceResult = await menuService.ChangeMenuSectionName(-7, newName); 
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.NotFound, serviceResult.Code);
            Assert.Null(serviceResult.Data);
        }
        #endregion

        #region delete menu tests
        [Fact]
        public async void DeletePosition_Success()
        {
            
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(deleteContext, Mapper);
            var serviceResult = await menuService.DeleteMenuPosition(inDbMenuPosition.Id);
            AssertSuccessServiceResult(serviceResult);
            Assert.True(serviceResult.Data);
            using var checkContext = new FoodDeliveryDbContext(ContextOptions);
            var result = await checkContext.MenuPositions.FindAsync(inDbMenuPosition.Id);
            Assert.Null(result);

            
        }
        [Fact]
        public async void DeleteSection_Success()
        {
            using var deleteContext = new FoodDeliveryDbContext(ContextOptions);
            var menuService = new MenuService(deleteContext, Mapper);

            var serviceResult2 = await menuService.DeleteMenuSection(inDbMenuSection.Id);
            AssertSuccessServiceResult(serviceResult2);
            Assert.True(serviceResult2.Data);

            var result2 = await deleteContext.MenuSections.FindAsync(inDbMenuSection.Id);
            Assert.Null(result2);
        }

        #endregion
    }
}
