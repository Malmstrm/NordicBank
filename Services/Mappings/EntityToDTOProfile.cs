using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;

namespace Services.Mappings
{
    public class EntityToDTOProfile : Profile
    {

        public EntityToDTOProfile()
        {
            // Customer
            CreateMap<Customer, CustomerDTO>()
                .ForMember(c => c.Status, opt => opt.MapFrom(src => src.CustomerStatus))
                .ReverseMap()
                .ForMember(c => c.CustomerStatus, opt => opt.MapFrom(src => src.Status));
            CreateMap<Customer, ViewCustomerDTO>()
                .ForMember(c => c.Status, opt => opt.MapFrom(src=> src.CustomerStatus));

            // Account
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Account, AccountSummaryDTO>().ReverseMap();

            // Transaction
            CreateMap<Transaction, TransactionDTO>().ReverseMap();

            // Others
            CreateMap<SuspiciousTransaction, SuspiciousTransactionDTO>().ReverseMap();
            CreateMap<ScanLog, ScanHistoryDTO>().ReverseMap();
            CreateMap<IdentityUser, UserDTO>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                    src.LockoutEnd == null || src.LockoutEnd <= DateTime.Now));

        }

    }
}
