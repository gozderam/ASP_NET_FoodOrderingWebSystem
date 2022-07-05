using Microsoft.Data.SqlClient;
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
    public class SeedDatabaseService : ISeedDatabaseService
    {
        private FoodDeliveryDbContext dbContext;
        private readonly string[] tableNames = new string[] {
            "Order_MenuPosition",
            "Clients_FavouriteRestaurants",
            "Complaints",
            "Orders",
            "MenuPositions",
            "MenuSections",
            "DiscountCodes",
            "Reviews",
            "RestaurantEmployees",
            "Restaurants",
            "Clients",
            "RestaurantAddresses",
            "ClientAddresses",
            "Admins"
        };
        public SeedDatabaseService(FoodDeliveryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IOperationResult<bool>> Seed()
        {
            ClearDatabase();
            AddData();
            return new ServiceOperationResult<bool>(true, ResultCode.Success);
        }

        private void ClearDatabase()
        {

            foreach (string name in tableNames)
            {
                string query = string.Format("DELETE FROM {0}", name);
                dbContext.Database.ExecuteSqlRaw(query);
            }

            dbContext.SaveChanges();
        }

        private void AddData()
        {
            #region ADMINS
            List<Admin> admins = new List<Admin>();
            admins.Add(new Admin()
            {
                Id = 1,
                Name = "Admin",
                Surname = "Test",
                Email = "admin@test.com"
            });
            admins.Add(new Admin()
            {
                Id = 2,
                Name = "Jan",
                Surname = "Kowalski",
                Email = "jan@kowalski.com"
            });
            admins.Add(new Admin()
            {
                Id = 3,
                Name = "Marek",
                Surname = "Nowak",
                Email = "marek@nowak.com"
            });
            dbContext.Admins.AddRange(admins.ToArray());
            IdentityInsertSaveChanges("Admins");
            #endregion

            #region CLIENT_ADRESSES
            List<ClientAddress> clientAddresses = new List<ClientAddress>();
            clientAddresses.Add(new ClientAddress()
            {
                Id = 1,
                City = "Test",
                Street = "Test",
                PostalCode = "00-000"
            });
            clientAddresses.Add(new ClientAddress()
            {
                Id = 2,
                City = "Warszawa",
                Street = "Polna",
                PostalCode = "00-571"
            });
            clientAddresses.Add(new ClientAddress()
            {
                Id = 3,
                City = "Kraków",
                Street = "Wawelska",
                PostalCode = "12-345"
            });
            clientAddresses.Add(new ClientAddress()
            {
                Id = 4,
                City = "Poznań",
                Street = "Sienkiewicz",
                PostalCode = "43-867"
            });
            clientAddresses.Add(new ClientAddress()
            {
                Id = 5,
                City = "Płock",
                Street = "Kwiatka",
                PostalCode = "09-400"
            });
            dbContext.ClientAddresses.AddRange(clientAddresses.ToArray());
            IdentityInsertSaveChanges("ClientAddresses");
            #endregion

            #region RESTAURANT_ADDRESSES
            List<RestaurantAddress> restaurantAddresses = new List<RestaurantAddress>();
            restaurantAddresses.Add(new RestaurantAddress()
            {
                Id = 1,
                City = "Test",
                Street = "Test",
                PostalCode = "00-000",
            });
            restaurantAddresses.Add(new RestaurantAddress()
            {
                Id = 2,
                City = "Warszawa",
                Street = "Polna",
                PostalCode = "00-571"
            });
            restaurantAddresses.Add(new RestaurantAddress()
            {
                Id = 3,
                City = "Kraków",
                Street = "Wawelska",
                PostalCode = "12-345"
            });
            restaurantAddresses.Add(new RestaurantAddress()
            {
                Id = 4,
                City = "Poznań",
                Street = "Sienkiewicz",
                PostalCode = "43-867"
            });
            restaurantAddresses.Add(new RestaurantAddress()
            {
                Id = 5,
                City = "Płock",
                Street = "Kwiatka",
                PostalCode = "09-400"
            });
            dbContext.RestaurantAddresses.AddRange(restaurantAddresses.ToArray());
            IdentityInsertSaveChanges("RestaurantAddresses");
            #endregion

            #region RESTAURANTS
            List<Restaurant> restaurants = new List<Restaurant>();
            restaurants.Add(new Restaurant()
            {
                Id = 1,
                Name = "Test",
                Contact = "restaurant@test.com",
                State = RestaurantState.Active,
                Rate = 5.0,
                ToPay = 1000.0,
                TotalPayment = 200.0,
                DateOfJoining = DateTime.Now,
                Address = restaurantAddresses[0],
            });
            restaurants.Add(new Restaurant()
            {
                Id = 2,
                Name = "Tasty Burgers",
                Contact = "tasty@burgers.com",
                State = RestaurantState.Disabled,
                Rate = 4.5,
                ToPay = 1234.0,
                TotalPayment = 123.0,
                DateOfJoining = DateTime.Now,
                Address = restaurantAddresses[1],
            });
            restaurants.Add(new Restaurant()
            {
                Id = 3,
                Name = "Pizza & Pasta",
                Contact = "pizza@pasta.com",
                State = RestaurantState.Deactivated,
                Rate = 4.3,
                ToPay = 2500.0,
                TotalPayment = 500.0,
                DateOfJoining = DateTime.Now,
                Address = restaurantAddresses[2],
            });
            restaurants.Add(new Restaurant()
            {
                Id = 4,
                Name = "Los Pollos Hermanos",
                Contact = "los@pollos.com",
                State = RestaurantState.Blocked,
                Rate = 3.1,
                ToPay = 10000.0,
                TotalPayment = 100.0,
                DateOfJoining = DateTime.Now,
                Address = restaurantAddresses[3],
            });
            restaurants.Add(new Restaurant()
            {
                Id = 5,
                Name = "Gofery z dżemorem",
                Contact = "gofry@dzem.com",
                State = RestaurantState.Active,
                Rate = 4.9,
                ToPay = 100.0,
                TotalPayment = 30.0,
                DateOfJoining = DateTime.Now,
                Address = restaurantAddresses[4],
            });

            dbContext.Restaurants.AddRange(restaurants.ToArray());
            IdentityInsertSaveChanges("Restaurants");
            #endregion

            #region CLIENTS
            List<Client> clients = new List<Client>();
            var favRestaurants1 = new List<Restaurant>();
            favRestaurants1.Add(restaurants[0]);
            clients.Add(new Client()
            {
                Id = 1,
                Name = "Client",
                Surname = "Test",
                Email = "client@test.com",
                Address = clientAddresses[0],
                FavouriteRestaurants = favRestaurants1
            });
            clients.Add(new Client()
            {
                Id = 2,
                Name = "Freddie",
                Surname = "Mercury",
                Email = "freddie@mercury.com",
                Address = clientAddresses[1]
            });
            clients.Add(new Client()
            {
                Id = 3,
                Name = "Elvis",
                Surname = "Presley",
                Email = "elvis@presley.com",
                Address = clientAddresses[2]
            });
            clients.Add(new Client()
            {
                Id = 4,
                Name = "Bon",
                Surname = "Jovi",
                Email = "bon@jovi.com",
                Address = clientAddresses[3]
            });
            clients.Add(new Client()
            {
                Id = 5,
                Name = "Bob",
                Surname = "Dylan",
                Email = "bob@dylan.com",
                Address = clientAddresses[4]
            });

            dbContext.Clients.AddRange(clients.ToArray());
            IdentityInsertSaveChanges("Clients");
            #endregion

            #region RESTAURANT_EMPLOYEES
            List<RestaurantEmployee> restaurantEmployees = new List<RestaurantEmployee>();
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 1,
                Name = "Restaurateur",
                Surname = "Test",
                Email = "restaurateur@test.com",
                IsRestaurateur = true,
                Restaurant = restaurants[0]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 2,
                Name = "Employee",
                Surname = "Test",
                Email = "employee@test.com",
                IsRestaurateur = false,
                Restaurant = restaurants[0]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 3,
                Name = "Marco",
                Surname = "Polo",
                Email = "marco@polo.com",
                IsRestaurateur = true,
                Restaurant = restaurants[1]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 4,
                Name = "Bob",
                Surname = "Budowniczy",
                Email = "bob@budowniczy.com",
                IsRestaurateur = false,
                Restaurant = restaurants[1]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 5,
                Name = "Don",
                Surname = "Matteo",
                Email = "don@matteo.com",
                IsRestaurateur = true,
                Restaurant = restaurants[2]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 6,
                Name = "Al",
                Surname = "Capone",
                Email = "al@capone.com",
                IsRestaurateur = false,
                Restaurant = restaurants[2]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 7,
                Name = "Walter",
                Surname = "white",
                Email = "walter@white.com",
                IsRestaurateur = true,
                Restaurant = restaurants[3]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 8,
                Name = "Jesse",
                Surname = "Pinkman",
                Email = "jesse@pinkman.com",
                IsRestaurateur = false,
                Restaurant = restaurants[3]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 9,
                Name = "Neil",
                Surname = "Armstrong",
                Email = "neil@armstrong.com",
                IsRestaurateur = true,
                Restaurant = restaurants[4]
            });
            restaurantEmployees.Add(new RestaurantEmployee()
            {
                Id = 10,
                Name = "Buzz",
                Surname = "Aldrin",
                Email = "buzz@aldrin.com",
                IsRestaurateur = false,
                Restaurant = restaurants[4]
            });

            dbContext.RestaurantEmployees.AddRange(restaurantEmployees.ToArray());
            IdentityInsertSaveChanges("RestaurantEmployees");
            #endregion

            #region REVIEWS
            List<Review> reviews = new List<Review>();
            reviews.Add(new Review()
            {
                Id = 1,
                Content = "Test",
                Rate = 5.0,
                Client = clients[0],
                Restaurant = restaurants[0]
            });
            reviews.Add(new Review()
            {
                Id = 2,
                Content = "Good burger",
                Rate = 4,
                Client = clients[1],
                Restaurant = restaurants[1]
            });
            reviews.Add(new Review()
            {
                Id = 3,
                Content = "Very Good Burger",
                Rate = 5.0,
                Client = clients[2],
                Restaurant = restaurants[1]
            });
            reviews.Add(new Review()
            {
                Id = 4,
                Content = "Not so good burger",
                Rate = 2.0,
                Client = clients[2],
                Restaurant = restaurants[1]
            });
            reviews.Add(new Review()
            {
                Id = 5,
                Content = "Spicy chicken",
                Rate = 5,
                Client = clients[4],
                Restaurant = restaurants[3]
            });
            reviews.Add(new Review()
            {
                Id = 6,
                Content = "Weird Owner",
                Rate = 2.0,
                Client = clients[2],
                Restaurant = restaurants[3]
            });
            reviews.Add(new Review()
            {
                Id = 7,
                Content = "Kosmicznie dobre goferki",
                Rate = 5.0,
                Client = clients[3],
                Restaurant = restaurants[4]
            });
            dbContext.Reviews.AddRange(reviews.ToArray());
            IdentityInsertSaveChanges("Reviews");
            #endregion

            #region DISCOUNT_CODES
            List<DiscountCode> discountCodes = new List<DiscountCode>();
            discountCodes.Add(new DiscountCode()
            {
                Id = 1,
                Percent = 0.5,
                Code = "SingleRestaurantTest",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(10),
                AppliesToAllRestaurants = false,
                AppliedToRestaurant = restaurants[0]
            });

            discountCodes.Add(new DiscountCode()
            {
                Id = 2,
                Percent = 0.3,
                Code = "SingleRestaurantTest",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(10),
                AppliesToAllRestaurants = true,
                AppliedToRestaurant = null
            });

            dbContext.DiscountCodes.AddRange(discountCodes.ToArray());
            IdentityInsertSaveChanges("DiscountCodes");
            #endregion


            #region MENU_SECTIONS
            List<MenuSection> menuSections = new List<MenuSection>();

            menuSections.Add(new MenuSection()
            {
                Id = 1,
                Name = "Test",
                Restaurant = restaurants[0]
            });

            menuSections.Add(new MenuSection()
            {
                Id = 2,
                Name = "Burgers",
                Restaurant = restaurants[1]
            });


            menuSections.Add(new MenuSection()
            {
                Id = 3,
                Name = "Pizza",
                Restaurant = restaurants[2]
            });

            menuSections.Add(new MenuSection()
            {
                Id = 4,
                Name = "Pasta",
                Restaurant = restaurants[2]
            });

            menuSections.Add(new MenuSection()
            {
                Id = 5,
                Name = "Chicken",
                Restaurant = restaurants[3]
            });

            menuSections.Add(new MenuSection()
            {
                Id = 6,
                Name = "Sides",
                Restaurant = restaurants[3]
            });

            menuSections.Add(new MenuSection()
            {
                Id = 7,
                Name = "Gofery",
                Restaurant = restaurants[4]
            });



            dbContext.MenuSections.AddRange(menuSections.ToArray());
            IdentityInsertSaveChanges("MenuSections");
            #endregion

            #region MENU_POSITIONS
            List<MenuPosition> menuPositions = new List<MenuPosition>();
            menuPositions.Add(new MenuPosition()
            {
                Id = 1,
                Name = "Test1",
                Description = "Test1",
                Price = 10.0,
                MenuSection = menuSections[0]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 2,
                Name = "Test2",
                Description = "Test2",
                Price = 20.0,
                MenuSection = menuSections[0]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 3,
                Name = "Test3",
                Description = "Test3",
                Price = 30.0,
                MenuSection = menuSections[0]
            });

            menuPositions.Add(new MenuPosition()
            {
                Id = 4,
                Name = "Burger",
                Description = "Tasty Burger",
                Price = 20.0,
                MenuSection = menuSections[1]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 5,
                Name = "Pizza Cappriciosa",
                Description = "Ser, szynka, pieczarki",
                Price = 20.0,
                MenuSection = menuSections[2]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 6,
                Name = "Pene al'arrabiata",
                Description = "Pikantne",
                Price = 15.0,
                MenuSection = menuSections[3]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 7,
                Name = "Fried Chicken",
                Description = "Crispy",
                Price = 15.0,
                MenuSection = menuSections[4]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 8,
                Name = "Boiled Chicken",
                Description = "Soft but healthy",
                Price = 11.50,
                MenuSection = menuSections[4]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 9,
                Name = "Fried Chicken",
                Description = "Crispy",
                Price = 15.0,
                MenuSection = menuSections[4]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 10,
                Name = "French Fries",
                Description = "All time classic",
                Price = 5.0,
                MenuSection = menuSections[5]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 11,
                Name = "Onion rings",
                Description = "Free dip included",
                Price = 6.0,
                MenuSection = menuSections[5]
            });
            menuPositions.Add(new MenuPosition()
            {
                Id = 12,
                Name = "Gofer",
                Description = "Do wyboru dżem truskawkowy i malinowy",
                Price = 7.0,
                MenuSection = menuSections[6]
            });

            dbContext.MenuPositions.AddRange(menuPositions.ToArray());
            IdentityInsertSaveChanges("MenuPositions");
            #endregion


            #region ORDERS
            List<Order> orders = new List<Order>();

            orders.Add(new Order()
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Card,
                OrderState = OrderState.Pending,
                Date = DateTime.Now,
                OriginalPrice = 60.0,
                FinalPrice = 30.0,
                Address = clientAddresses[0],
                DiscountCode = discountCodes[0],
                Client = clients[0],
                Restaurant = restaurants[0],
                ResponsibleEmployee = restaurantEmployees[0],
            });
            List<Order_MenuPosition> positions1 = new List<Order_MenuPosition>();
            positions1.Add(new Order_MenuPosition()
            {
                Order = orders[0],
                MenuPosition = menuPositions[0],
                PositionsInOrder = 1
            });
            positions1.Add(new Order_MenuPosition()
            {
                Order = orders[0],
                MenuPosition = menuPositions[1],
                PositionsInOrder = 1
            });
            positions1.Add(new Order_MenuPosition()
            {
                Order = orders[0],
                MenuPosition = menuPositions[2],
                PositionsInOrder = 1
            });
            orders[0].OrdersMenuPositions = positions1;

            orders.Add(new Order()
            {
                Id = 2,
                PaymentMethod = PaymentMethod.Transfer,
                OrderState = OrderState.Pending,
                Date = DateTime.Now,
                OriginalPrice = 20.0,
                FinalPrice = 20.0,
                Address = clientAddresses[1],
                Client = clients[1],
                Restaurant = restaurants[1],
                ResponsibleEmployee = restaurantEmployees[2],
            });
            List<Order_MenuPosition> positions2 = new List<Order_MenuPosition>();
            positions2.Add(new Order_MenuPosition()
            {
                Order = orders[1],
                MenuPosition = menuPositions[3],
                PositionsInOrder = 1
            });
            orders[1].OrdersMenuPositions = positions2;

            orders.Add(new Order()
            {
                Id = 3,
                PaymentMethod = PaymentMethod.Transfer,
                OrderState = OrderState.Pending,
                Date = DateTime.Now,
                OriginalPrice = 15.0,
                FinalPrice = 15.0,
                Address = clientAddresses[2],
                Client = clients[2],
                Restaurant = restaurants[2],
                ResponsibleEmployee = restaurantEmployees[4],
            });
            List<Order_MenuPosition> positions3 = new List<Order_MenuPosition>();
            positions3.Add(new Order_MenuPosition()
            {
                Order = orders[2],
                MenuPosition = menuPositions[5],
                PositionsInOrder = 1
            });
            orders[2].OrdersMenuPositions = positions3;

            orders.Add(new Order()
            {
                Id = 4,
                PaymentMethod = PaymentMethod.Transfer,
                OrderState = OrderState.Pending,
                Date = DateTime.Now,
                OriginalPrice = 20.0,
                FinalPrice = 20.0,
                Address = clientAddresses[2],
                Client = clients[2],
                Restaurant = restaurants[2],
                ResponsibleEmployee = restaurantEmployees[4],
            });
            List<Order_MenuPosition> positions4 = new List<Order_MenuPosition>();
            positions4.Add(new Order_MenuPosition()
            {
                Order = orders[3],
                MenuPosition = menuPositions[4],
                PositionsInOrder = 1
            });
            orders[3].OrdersMenuPositions = positions4;


            orders.Add(new Order()
            {
                Id = 5,
                PaymentMethod = PaymentMethod.Transfer,
                OrderState = OrderState.Pending,
                Date = DateTime.Now,
                OriginalPrice = 20.0,
                FinalPrice = 20.0,
                Address = clientAddresses[1],
                Client = clients[1],
                Restaurant = restaurants[1],
                ResponsibleEmployee = restaurantEmployees[2],
            });
            List<Order_MenuPosition> positions5 = new List<Order_MenuPosition>();
            positions5.Add(new Order_MenuPosition()
            {
                Order = orders[4],
                MenuPosition = menuPositions[3],
                PositionsInOrder = 1
            });
            orders[4].OrdersMenuPositions = positions5;

            dbContext.Orders.AddRange(orders.ToArray());
            IdentityInsertSaveChanges("Orders");
            #endregion


            #region COMPLAINTS
            List<Complaint> complaints = new List<Complaint>();
            complaints.Add(new Complaint()
            {
                Id = 1,
                Content = "Test",
                IsOpened = true,
                Answer = "Test",
                Client = clients[0],
                Order = orders[0],
                AttendingEmployee = restaurantEmployees[0]
            });
            complaints.Add(new Complaint()
            {
                Id = 2,
                Content = "Not tasty Burger",
                IsOpened = true,
                Answer = "It was",
                Client = clients[1],
                Order = orders[1],
                AttendingEmployee = restaurantEmployees[2]
            });
            complaints.Add(new Complaint()
            {
                Id = 3,
                Content = "Too small portion",
                IsOpened = true,
                Answer = "It'll get biger",
                Client = clients[2],
                Order = orders[2],
                AttendingEmployee = restaurantEmployees[4]
            });
            complaints.Add(new Complaint()
            {
                Id = 4,
                Content = "Square pizza",
                IsOpened = true,
                Answer = "what?",
                Client = clients[2],
                Order = orders[3],
                AttendingEmployee = restaurantEmployees[4]
            });
            complaints.Add(new Complaint()
            {
                Id = 5,
                Content = "Burned meat",
                IsOpened = true,
                Answer = "Sorry, we'll keep the flame low next time",
                Client = clients[1],
                Order = orders[4],
                AttendingEmployee = restaurantEmployees[2]
            });

            dbContext.Complaints.AddRange(complaints.ToArray());
            IdentityInsertSaveChanges("Complaints");
            #endregion


        }

        //Without this, you can't add entities with explicit ID.
        private void IdentityInsertSaveChanges(string tableName)
        {
            string queryOn = string.Format("SET IDENTITY_INSERT dbo.{0} ON", tableName);
            string queryOff = string.Format("SET IDENTITY_INSERT dbo.{0} OFF", tableName);

            using var transaction = dbContext.Database.BeginTransaction();
            dbContext.Database.ExecuteSqlRaw(queryOn);
            dbContext.SaveChanges();
            dbContext.Database.ExecuteSqlRaw(queryOff);
            transaction.Commit();
        }

    }
}
