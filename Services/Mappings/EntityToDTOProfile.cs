using AutoMapper;
using DataAccessLayer.DTO;
using DataAccessLayer.Models;

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

        }

    }
}
