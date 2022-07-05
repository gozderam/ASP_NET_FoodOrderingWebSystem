using AutoMapper;
using Common.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using WebApiApplication.Database.POCO;



namespace WebApiApplication.MappingProfiles
{
    public class FoodDeliveryMappingProfile : Profile
    {
        private const double HUNDRED = 100.0;
        public FoodDeliveryMappingProfile()
        {
            // if in mapped and to map objects properties have the same names you dont need to do this (e.g. automatically mapped for Surname)

            #region Admin
            CreateMap<Admin, AdministratorDTO>();

            CreateMap<NewAdministratorDTO, Admin>();
            #endregion

            #region Address
            CreateMap<AddressDTO, ClientAddress>()
                 .ForMember(clientAddress => clientAddress.PostalCode, imc => imc.MapFrom(t => t.postcode));

            CreateMap<ClientAddress, AddressDTO>()
                .ForMember(adressDTO => adressDTO.postcode, clientAddress => clientAddress.MapFrom(t => t.PostalCode));

            CreateMap<AddressDTO, ClientAddress>()
                .ForMember(adress => adress.PostalCode, addressDTO => addressDTO.MapFrom(t => t.postcode));

            CreateMap<RestaurantAddress, AddressDTO>()
                 .ForMember(addressDTO => addressDTO.postcode, restaurantAddress => restaurantAddress.MapFrom(t => t.PostalCode));
            #endregion

            #region Customer
            CreateMap<NewCustomerDTO, Client>();

            CreateMap<Client, CustomerADTO>();

            CreateMap<Client, CustomerCDTO>();
            #endregion

            #region Employee
            CreateMap<RestaurantEmployee, EmployeeDTO>();

            CreateMap<NewEmployeeDTO, RestaurantEmployee>();
            #endregion

            #region Review
            CreateMap<NewReviewDTO, Review>()
                  .ForMember(review => review.Rate, imc => imc.MapFrom(t => t.rating));

            CreateMap<Review, ReviewDTO>()
                 .ForMember(reviewDTO => reviewDTO.rating, imc => imc.MapFrom(t => t.Rate))
                 .ForMember(reviewDTO => reviewDTO.restaurantId, imc => imc.MapFrom(t => t.Restaurant.Id))
                 .ForMember(reviewDTO => reviewDTO.customerId, imc => imc.MapFrom(t => t.Client.Id));

            CreateMap<Review, ReviewRDTO>()
                .ForMember(reviewRDTO => reviewRDTO.rating, imc => imc.MapFrom(t => t.Rate));
            #endregion

            #region Restaurant
            CreateMap<MenuSection, SectionDTO>()
                 .ForMember(sectionDTO => sectionDTO.restaurantId, imc => imc.MapFrom(t => t.Restaurant.Id));

            CreateMap<MenuPosition, PositionFromMenuDTO>();

            CreateMap<Restaurant, RestaurantDTO>()
                .ForMember(restaurantDTO => restaurantDTO.contactInformation, imc => imc.MapFrom(t => t.Contact))
                .ForMember(restaurantDTO => restaurantDTO.state, imc => imc.MapFrom(t => t.State))
                .ForMember(restaurantDTO => restaurantDTO.rating, imc => imc.MapFrom(t => t.Rate))
                .ForMember(restaurantDTO => restaurantDTO.owing, imc => imc.MapFrom(t => t.ToPay))
                .ForMember(restaurantDTO => restaurantDTO.aggregatePayment, imc => imc.MapFrom(t => t.TotalPayment))
                .ForMember(restaurantDTO => restaurantDTO.dateOfJoining, imc => imc.MapFrom(t => t.DateOfJoining.ToString()));

            CreateMap<Restaurant, RestaurantCDTO>()
                .ForMember(restaurantDTO => restaurantDTO.contactInformation, imc => imc.MapFrom(t => t.Contact))
                .ForMember(restaurantDTO => restaurantDTO.state, imc => imc.MapFrom(t => t.State))
                .ForMember(restaurantDTO => restaurantDTO.rating, imc => imc.MapFrom(t => t.Rate));

            CreateMap<AddressDTO, RestaurantAddress>()
                .ForMember(restaurantAddress => restaurantAddress.PostalCode, imc => imc.MapFrom(t => t.postcode));

            CreateMap<Restaurant, NewRestaurantDTO>()
                .ForMember(resDTO => resDTO.contactInformation, res => res.MapFrom(r => r.Contact));

            CreateMap<NewRestaurantDTO, Restaurant>()
                .ForMember(restaurant => restaurant.Contact, newRestaurant => newRestaurant.MapFrom(r => r.contactInformation));

            CreateMap<NewPositionFromMenuDTO, MenuPosition>();

            CreateMap<SectionDTO, MenuSection>();
            CreateMap<PositionFromMenuDTO, MenuPosition>();
            CreateMap<MenuPosition, PositionFromMenuDTO>();
            CreateMap<MenuPosition[], PositionFromMenuDTO[]>();
            #endregion

            #region DiscountCode
            CreateMap<DiscountCode, DiscountCodeDTO>()
                .ForMember(discountCodeDTO => discountCodeDTO.percent, imc => imc.MapFrom(t => t.Percent * HUNDRED))
                .ForMember(discountCodeDTO => discountCodeDTO.dateFrom, imc => imc.MapFrom(t => t.DateFrom.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)))
                .ForMember(discountCodeDTO => discountCodeDTO.dateTo, imc => imc.MapFrom(t => t.DateTo.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)))
                .ForMember(discountCodeDTO => discountCodeDTO.restaurantId, imc => imc.MapFrom(t => !t.AppliesToAllRestaurants ? t.AppliedToRestaurant.Id : (int?)null));

            CreateMap<NewDiscountCodeDTO, DiscountCode>()
                .ForMember(discountCode => discountCode.Percent, imc => imc.MapFrom(t => t.percent / HUNDRED))
                .ForMember(discountCode => discountCode.DateFrom, imc => imc.MapFrom(t => DateTime.Parse(t.dateFrom)))
                .ForMember(discountCode => discountCode.DateTo, imc => imc.MapFrom(t => DateTime.Parse(t.dateTo)))
                .ForMember(discountCode => discountCode.AppliesToAllRestaurants, imc => imc.MapFrom(t => t.restaurantId == null ? true : false));
            #endregion

            #region Complaint
            CreateMap<Complaint, ComplaintDTO>()
                  .ForMember(complaintDTO => complaintDTO.response, imc => imc.MapFrom(t => t.Answer))
                  .ForMember(complaintDTO => complaintDTO.open, imc => imc.MapFrom(t => t.IsOpened))
                  .ForMember(complaintDTO => complaintDTO.orderId, imc => imc.MapFrom(t => t.OrderForeignKey))
                  .ForMember(complaintDTO => complaintDTO.customerId, imc => imc.MapFrom(t => t.Client.Id));

            CreateMap<Complaint, ComplaintRDTO>()
                .ForMember(complaintDTO => complaintDTO.response, imc => imc.MapFrom(t => t.Answer))
                .ForMember(complaintDTO => complaintDTO.open, imc => imc.MapFrom(t => t.IsOpened))
                .ForMember(complaintDTO => complaintDTO.orderId, imc => imc.MapFrom(t => t.OrderForeignKey))
                .ForMember(complaintDTO => complaintDTO.employee, imc => imc.MapFrom(t => t.AttendingEmployee));


            CreateMap<NewComplaintDTO, Complaint>()
                    .ForMember(complaint => complaint.Content, imc => imc.MapFrom(t => t.content))
                    .ForMember(complaint => complaint.OrderForeignKey, imc => imc.MapFrom(t => t.orderId));

            #endregion


            #region Order
            CreateMap<Order, OrderCDTO>()
                .ForMember(orderDTO => orderDTO.state, order => order.MapFrom(s => s.OrderState))
                .ForMember(orderDTO => orderDTO.positions, order => order.MapFrom(s => s.OrdersMenuPositions.FlattenMenuPositionList()));

            CreateMap<Order, OrderADTO>()
                .ForMember(orderDTO => orderDTO.state, order => order.MapFrom(s => s.OrderState))
                .ForMember(orderDTO => orderDTO.customer, order => order.MapFrom(s => s.Client));

            CreateMap<Order, OrderRDTO>()
                .ForMember(orderDTO => orderDTO.state, order => order.MapFrom(s => s.OrderState))
                .ForMember(orderDTO => orderDTO.positions, order => order.MapFrom(s => s.OrdersMenuPositions.FlattenMenuPositionList()))
                .ForMember(orderDTO => orderDTO.employee, order => order.MapFrom(s => s.ResponsibleEmployee));

            CreateMap<NewOrderDTO, Order>()
                .ForMember(order => order.PaymentMethod, orderDTO => orderDTO.MapFrom(s => s.paymentMethod))
                .ForMember(order => order.Date, orderDTO => orderDTO.MapFrom(s => DateTime.Parse(s.date)));
            #endregion

        }

        #region private methods
        private static PaymentMethodDTO StringToEnum(string PaymentMethod)
        {
            Enum.TryParse(PaymentMethod, true, out PaymentMethodDTO r);
            return r;
        }
        #endregion
    }


    internal static class MenuPositonEnumebrableExtensions
    {
        internal static IEnumerable<MenuPosition> FlattenMenuPositionList(this IEnumerable<Order_MenuPosition> ordersMenuPositions)
        {
            var res = new List<MenuPosition>();
            foreach (var omp in ordersMenuPositions)
                for (int i = 0; i < omp.PositionsInOrder; i++)
                    res.Add(omp.MenuPosition);
            return res;
        }
    }
}
