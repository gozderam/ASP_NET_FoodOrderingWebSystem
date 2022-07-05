using AutoMapper;
using Common.DTO;
using RestaurantManagerApplication.Models;
using Common.DTO;
using System.Globalization;
using System;

namespace RestaurantManagerApplication.MappingProfiles
{
    public class RestaurantManagmentMappingProfile : Profile
    {
        private DateTime SuperExtraConverter(string date)
        {
            DateTime ret;
            if (DateTime.TryParse(date, out ret)) return ret;
            else return new DateTime();
        }
        public RestaurantManagmentMappingProfile()
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            CreateMap<AddressDTO, AddressModel>()
                .ForMember(m => m.PostalCode, dto => dto.MapFrom(t => t.postcode));
            CreateMap<AddressModel, AddressDTO>()
                .ForMember(dto => dto.postcode, m => m.MapFrom(t => t.PostalCode));
            CreateMap<MenuPositionModel, PositionFromMenuDTO>();
            CreateMap<PositionFromMenuDTO, MenuPositionModel>();
            CreateMap<NewMenuPositionModel, NewPositionFromMenuDTO>(); 

            CreateMap<MenuSectionModel, SectionDTO>();
            CreateMap<SectionDTO, MenuSectionModel>();

            CreateMap<DiscountCodeDTO, DiscountCodeModel>()
                .ForMember(discountCode => discountCode.Percent, imc => imc.MapFrom(t => t.percent / 100.0))
                .ForMember(discountCode => discountCode.DateFrom, imc => imc.MapFrom(t => SuperExtraConverter(t.dateFrom)))
                .ForMember(discountCode => discountCode.DateTo, imc => imc.MapFrom(t => SuperExtraConverter(t.dateTo)));

            CreateMap<DiscountCodeModel, NewDiscountCodeDTO>()
                .ForMember(newDiscountCodeDTO => newDiscountCodeDTO.dateFrom, imc => imc.MapFrom(t => t.DateFrom.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)))
                .ForMember(newDiscountCodeDTO => newDiscountCodeDTO.dateTo, imc => imc.MapFrom(t => t.DateTo.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)));

            CreateMap<NewRestaurantModel, NewRestaurantDTO>()
                .ForMember(m => m.contactInformation, dto => dto.MapFrom(t => t.Contact));
            CreateMap<RestaurantDTO, RestaurantModel>();
            CreateMap<RestaurantModel, RestaurantDTO>();

            CreateMap<ComplaintRDTO, ComplaintModel>()
                .ForMember(complaintDTO => complaintDTO.AttendingEmployee, imc => imc.MapFrom(t => t.employee));
            CreateMap<ComplaintModel, ComplaintRDTO>()
                .ForMember(complaintDTO => complaintDTO.employee, imc => imc.MapFrom(t => t.AttendingEmployee));

            CreateMap<OrderRDTO, OrderModel>()
                .ForMember(dto => dto.discountcode, mod => mod.MapFrom(t => t.discountcode));
            CreateMap<OrderModel, OrderRDTO>();

            CreateMap<EmployeeDTO, EmployeeModel>();
            CreateMap<EmployeeModel, EmployeeDTO>();

            CreateMap<NewRestaurantEmployeeModel, NewEmployeeDTO>();

            CreateMap<MenuPositionModel,NewMenuPositionModel>();

            CreateMap<ReviewRDTO, ReviewViewModel>();

            CreateMap<EmployeeDTO, EmployeeStatusModel>()
                .ForMember(employeeModel => employeeModel.isRestaurateur, imc => imc.MapFrom(t => t.isRestaurateur))
                .ForMember(employeeModel => employeeModel.restaurantStatus, imc => imc.MapFrom(t => t.restaurant.state))
                .ForMember(employeeModel => employeeModel.restaurantId, imc => imc.MapFrom(t => t.restaurant.id));
        }
    }
}
