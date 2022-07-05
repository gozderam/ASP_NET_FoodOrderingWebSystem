using AdminApplication.Models;
using AutoMapper;
using Common.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApplication.MappingProfiles
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            CreateMap<ComplaintDTO, ComplaintModel>();
            CreateMap<CustomerADTO, UserDataModel>();
            CreateMap<AddressDTO, AddressModel>();
            CreateMap<DiscountCodeDTO, DiscountCodeModel>();
            CreateMap<NewAdminModel, NewAdministratorDTO>();

            CreateMap<NewDiscountCodeModel, NewDiscountCodeDTO>()
                .ForMember(newDiscountCodeDTO => newDiscountCodeDTO.dateFrom, imc => imc.MapFrom(t => t.DateFrom.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)))
                .ForMember(newDiscountCodeDTO => newDiscountCodeDTO.dateTo, imc => imc.MapFrom(t => t.DateTo.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture)));

            CreateMap<OrderADTO, OrderModel>();
        }
    }
}
