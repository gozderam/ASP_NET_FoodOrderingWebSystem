using AutoMapper;
using ClientApplication.Models;
using ClientApplication.Models.ViewModels;
using Common.DTO;
using System;



namespace ClientApplication.MappingProfiles
{
    public class FoodDeliveryMappingProfile : Profile
    {
        private const int HUNDRED = 100;
        public FoodDeliveryMappingProfile()
        {
            // if in mapped and to map objects properties have the same names you dont need to do this (e.g. automatically mapped for Surname)

            CreateMap<NewCustomerViewModel, NewCustomerDTO>()
                .ForPath(customerDTO => customerDTO.address.city, customerViewModel => customerViewModel.MapFrom(c => c.City))
                .ForPath(customerDTO => customerDTO.address.street, customerViewModel => customerViewModel.MapFrom(c => c.Street))
                .ForPath(customerDTO => customerDTO.address.postcode, customerViewModel => customerViewModel.MapFrom(c => c.Postcode));

            CreateMap<AddressViewModel, AddressDTO>();

            CreateMap<AddressDTO, AddressViewModel>();

            CreateMap<NewOrderViewModel, NewOrderDTO>()
                .ForMember(orderDTO => orderDTO.discountcodeId, newOrder => newOrder.Ignore());

            CreateMap<PositionFromMenuDTO, MenuPositionViewModel>();

            CreateMap<OrderCDTO, OrderViewModel>()
                .ForMember(order => order.state, orderDTO => orderDTO.MapFrom(s => OrderStateToEnum(s.state.ToString().ToLower())));

            CreateMap<NewReviewViewModel, NewReviewDTO>();
            
            CreateMap<ComplaintDTO, ComplaintModel>();

            CreateMap<ReviewDTO, ReviewViewModel>();
            
            CreateMap<RestaurantCDTO, RestaurantViewModel>()
                .ForMember(restaurant => restaurant.state, restaurantDTO => restaurantDTO.MapFrom(s => RestaurantStateToEnum(s.state.ToString().ToLower()))); ;

            CreateMap<NewComplaintViewModel, NewComplaintDTO>();
        }
        public PaymentMethodDTO StringToEnum(string PaymentMethod)
        {
            Enum.TryParse(PaymentMethod, true, out PaymentMethodDTO r);
            return r;
        }

        private static OrderStateDTO OrderStateToEnum(string OrderState)
        {
            Enum.TryParse(OrderState, true, out OrderStateDTO r);
            return r;
        }

        private static RestaurantStateDTO RestaurantStateToEnum(string restaurantState)
        {
            Enum.TryParse(restaurantState, true, out RestaurantStateDTO r);
            return r;
        }
    }
}
