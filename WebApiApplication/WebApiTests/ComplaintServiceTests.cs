using Common.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.Database;
using WebApiApplication.Database.POCO;
using WebApiApplication.Services;
using WebApiTests.Base;
using Xunit;

namespace WebApiTests
{
    public class ComplaintServiceTests : BaseTests, IDisposable
    {
        #region seeding and disposeing
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptions { get; }
        protected DbContextOptions<FoodDeliveryDbContext> ContextOptionsEmpty { get; }

        private Restaurant inDbRestaurant;
        private readonly int notInDbRestaurantId = -2;

        private Order inDbOrder;

        private Client inDbCustomer;
        private readonly int notInDbCustomerId = -2;

        private RestaurantEmployee inDbEmployee;
        private readonly int notInDbEmployeeId = -2;


        private Complaint inDbComplaint;
        private Complaint toDeleteComplaint;
        private Complaint withoutAnswerComplaint;

        public ComplaintServiceTests()
        {
            ContextOptions = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            ContextOptionsEmpty = new DbContextOptionsBuilder<FoodDeliveryDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

            Seed();
        }

        private void Seed()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            using var recreateContextEmpty = new FoodDeliveryDbContext(ContextOptionsEmpty);

            // recreating in-memory database
            recreateContext.Database.EnsureCreated();
            recreateContextEmpty.Database.EnsureCreated();

            using var context = new FoodDeliveryDbContext(ContextOptions);

            // seeding data
            inDbRestaurant = new Restaurant()
            {
                Name = "ComplaintServiceTests_Restaurant",
                Address = new RestaurantAddress(),
            };
            context.Restaurants.Add(inDbRestaurant);
            context.SaveChanges();

            inDbEmployee = new RestaurantEmployee()
            {
                Name = "Adam",
                IsRestaurateur = true,
                Restaurant = inDbRestaurant,
            };
            context.RestaurantEmployees.Add(inDbEmployee);
            context.SaveChanges();
            inDbCustomer = new Client()
            {
                Name = "ComplaintServiceTests_CustomerName",
                Surname = "ComplaintServiceTests_CustomerSurname",
                Address = new ClientAddress(),
                Email = "ComplaintServiceTests_customer@gmail.com",
            };
            context.Clients.Add(inDbCustomer);
            context.SaveChanges();

            toDeleteComplaint = new()
            {
                Client = new Client(),
                Order = new Order(),
                Content = "ComplantServiceTests Complaint content",
                Answer = "ComplaintServiceTests Answer content",
                IsOpened = true,
            };
            context.Complaints.Add(toDeleteComplaint);
            context.SaveChanges();

            inDbEmployee = new()
            {
                Name = "test",
                Surname = "test",
                Restaurant = inDbRestaurant
            };
            context.RestaurantEmployees.Add(inDbEmployee);
            context.SaveChanges();

            inDbOrder = new Order()
            {
                Client = inDbCustomer,
                Date = DateTime.Now,
                Address = new ClientAddress(),
                FinalPrice = 30,
                OriginalPrice = 30,
                OrdersMenuPositions = new List<Order_MenuPosition>(),
                Restaurant = inDbRestaurant,
                OrderState = OrderState.Completed,
                PaymentMethod = PaymentMethod.Card,
            };
            context.Orders.Add(inDbOrder);
            context.SaveChanges();

            inDbComplaint = new()
            {
                Client = inDbCustomer,
                Order = inDbOrder,
                Content = "ComplantServiceTests Complaint content",
                Answer = "ComplaintServiceTests Answer content",
                IsOpened = true,
                AttendingEmployee = inDbEmployee
            };
            context.Complaints.Add(inDbComplaint);
            context.SaveChanges();
        }

        public void Dispose()
        {
            using var recreateContext = new FoodDeliveryDbContext(ContextOptions);
            recreateContext.Database.EnsureDeleted();
        }
        #endregion

        #region Get Complaints tests
        [Fact]
        public async void GetComplaint_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var foundComplaints = await complaintService.GetAllComplaintsForClient(inDbCustomer.Id);
            AssertSuccessServiceResult(foundComplaints);
            Assert.Single(foundComplaints.Data);
            var foundComplaint = foundComplaints.Data[0];
            Assert.NotNull(foundComplaint);
            Assert.Equal(foundComplaint.content, inDbComplaint.Content);
            Assert.Equal(foundComplaint.open, inDbComplaint.IsOpened);
            Assert.Equal(foundComplaint.response, inDbComplaint.Answer);
            Assert.Equal(foundComplaint.customerId, inDbComplaint.Client.Id);
        }

        [Fact]
        public async void GetComplaint_UserNotInDb()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var foundComplaints = await complaintService.GetAllComplaintsForClient(notInDbCustomerId);
            Assert.NotNull(foundComplaints);
            Assert.Equal(ResultCode.NotFound, foundComplaints.Code);
            Assert.Null(foundComplaints.Data);
        }

        [Fact]
        public async void GetComplaintForRestaurant_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var foundComplaints = await complaintService.GetAllComplaintsForRestaurant(inDbRestaurant.Id);
            AssertSuccessServiceResult(foundComplaints);
            Assert.Single(foundComplaints.Data);
            var foundComplaint = foundComplaints.Data[0];
            Assert.NotNull(foundComplaint);
            Assert.Equal(foundComplaint.content, inDbComplaint.Content);
            Assert.Equal(foundComplaint.open, inDbComplaint.IsOpened);
            Assert.Equal(foundComplaint.response, inDbComplaint.Answer);
            //Assert.Equal(foundComplaint., inDbComplaint.Client.Id);
            Assert.Equal(foundComplaint.orderId, inDbOrder.Id);
        }
        [Fact]
        public async void GetComplaintForRestaurantEmployee_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var foundComplaints = await complaintService.GetAllComplaintsForRestaurantEmployee(inDbEmployee.Id);
            AssertSuccessServiceResult(foundComplaints);
            Assert.Single(foundComplaints.Data);
            var foundComplaint = foundComplaints.Data[0];
            Assert.NotNull(foundComplaint);
            Assert.Equal(foundComplaint.content, inDbComplaint.Content);
            Assert.Equal(foundComplaint.open, inDbComplaint.IsOpened);
            Assert.Equal(foundComplaint.response, inDbComplaint.Answer);
            //Assert.Equal(foundComplaint., inDbComplaint.Client.Id);
            Assert.Equal(foundComplaint.orderId, inDbOrder.Id);
        }

        [Fact]
        public async void GetComplaintForRestaurant_RestaurantrNotInDb()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var foundComplaints = await complaintService.GetAllComplaintsForRestaurant(notInDbRestaurantId);
            Assert.NotNull(foundComplaints);
            Assert.Equal(ResultCode.NotFound, foundComplaints.Code);
            Assert.Null(foundComplaints.Data);
        }

        #endregion

        #region Get single complaint tests
        [Fact]
        public async void GetComplaintAdmin_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var ret = await complaintService.GetComplaintAdmin(inDbComplaint.Id);
            AssertSuccessServiceResult(ret);
            var data = ret.Data;
            Assert.Equal(data.content, inDbComplaint.Content);
            Assert.Equal(data.open, inDbComplaint.IsOpened);
            Assert.Equal(data.response, inDbComplaint.Answer);
            Assert.Equal(data.customerId, inDbComplaint.Client.Id);
        }

        [Fact]
        public async void GetComplaintCustomer_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var ret = await complaintService.GetComplaintCustomer(inDbComplaint.Id, inDbCustomer.Id);
            AssertSuccessServiceResult(ret);
            var data = ret.Data;
            Assert.Equal(data.content, inDbComplaint.Content);
            Assert.Equal(data.open, inDbComplaint.IsOpened);
            Assert.Equal(data.response, inDbComplaint.Answer);
            Assert.Equal(data.customerId, inDbComplaint.Client.Id);
        }

        [Fact]
        public async void GetComplaintCustomer_NotHisComplaint()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            Client test = new Client();
            var ret = await complaintService.GetComplaintCustomer(inDbComplaint.Id, test.Id);
            Assert.False(ret.IsSuccessful);
            Assert.Equal(ResultCode.Unauthorized, ret.Code);
        }

        [Fact]
        public async void GetComplaintEmployee_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            var ret = await complaintService.GetComplaintEmployee(inDbComplaint.Id, inDbEmployee.Id);
            AssertSuccessServiceResult(ret);
            var data = ret.Data;
            Assert.Equal(data.content, inDbComplaint.Content);
            Assert.Equal(data.open, inDbComplaint.IsOpened);
            Assert.Equal(data.response, inDbComplaint.Answer);
            Assert.Equal(data.employee.id, inDbComplaint.AttendingEmployee.Id);
        }

        [Fact]
        public async void GetComplaintEmployee_NotHisRestaurant()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            RestaurantEmployee test = new RestaurantEmployee()
            {
                Restaurant = new Restaurant()
            };
            var ret = await complaintService.GetComplaintEmployee(inDbComplaint.Id, test.Id);
            Assert.False(ret.IsSuccessful);
            Assert.Equal(ResultCode.Unauthorized, ret.Code);
        }
        #endregion

        #region Add new complaint tests
        [Fact]
        public async void AddComplaint_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            string contentTest = "VERY BAD";
            int orderIdTest = inDbOrder.Id;
            // retrieving
            NewComplaintDTO toAdd = new NewComplaintDTO()
            {
                content = contentTest,
                orderId = orderIdTest
            };
            var ret = await complaintService.PostComplaint(toAdd, inDbCustomer.Id);
            AssertSuccessServiceResult(ret);
            var added = await getContext.Complaints.FindAsync(ret.Data);
            Assert.NotNull(added);
            Assert.Equal(contentTest, added.Content);
            Assert.Equal(orderIdTest, added.OrderForeignKey);
        }

        [Fact]
        public async void AddComplaint_NotHisOrder()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            string contentTest = "VERY BAD";
            int orderIdTest = inDbOrder.Id;
            // retrieving
            NewComplaintDTO toAdd = new NewComplaintDTO()
            {
                content = contentTest,
                orderId = orderIdTest
            };

            Client test = new Client();
            getContext.Clients.Add(test);
            getContext.SaveChanges();

            var ret = await complaintService.PostComplaint(toAdd, test.Id);
            Assert.False(ret.IsSuccessful);
            Assert.Equal(ResultCode.Unauthorized, ret.Code);
        }
        #endregion

        #region Delete complaint tests
        [Fact]
        public async void DeleteComplaint_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            int id = toDeleteComplaint.Id;
            Assert.NotNull(await getContext.Complaints.FindAsync(id));
            var ret = await complaintService.DeleteComplaint(toDeleteComplaint.Id);
            AssertSuccessServiceResult(ret);
            Assert.Null(await getContext.Complaints.FindAsync(id));
        }
        #endregion

        #region respond to complaint tests
        [Fact]
        public async void RespondToComplaint_Success()
        {
            using var getContext = new FoodDeliveryDbContext(ContextOptions);
            var complaintService = new ComplaintService(getContext, Mapper);
            // retrieving
            Client testClient = new Client();
            getContext.Clients.Add(testClient);
            getContext.SaveChanges();

            Restaurant testRestaurant = new Restaurant();
            getContext.Restaurants.Add(testRestaurant);

            getContext.SaveChanges();
            Order testOrder = new Order()
            {
                Client = testClient,
                Restaurant = testRestaurant
            };
            getContext.Orders.Add(testOrder);
            getContext.SaveChanges();

            RestaurantEmployee testEmployee = new RestaurantEmployee()
            {
                Restaurant = testRestaurant
            };
            getContext.RestaurantEmployees.Add(testEmployee);
            getContext.SaveChanges();

            Complaint testComplaint = new Complaint()
            {
                Client = testClient,
                Order = testOrder,
                Answer = ""
            };
            getContext.Complaints.Add(testComplaint);
            getContext.SaveChanges();

            string response = "SORRY";
            var ret = await complaintService.RespondToComplaint(testComplaint.Id, testEmployee.Id, response);
            AssertSuccessServiceResult(ret);
            Assert.Equal(response, testComplaint.Answer);
        }
        #endregion

    }

}
