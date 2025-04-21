using AutoMapper;
using DataAccessLayer.DTO;
using NordicBank.ViewModels;

namespace NordicBank.MappingProfiles
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            // Customer
            CreateMap<CustomerDTO, CustomerViewModel>().ReverseMap();
            CreateMap<ViewCustomerDTO, ViewCustomerViewModel>().ReverseMap();
            CreateMap<CustomerDTO, CustomerDetailsViewModel>();

            // Account
            CreateMap<AccountDTO, AccountViewModel>().ReverseMap();
            CreateMap<AccountSummaryDTO, AccountSummaryViewModel>().ReverseMap();

            // Transaction
            CreateMap<TransactionDTO, TransactionViewModel>().ReverseMap();

            // Other
            CreateMap<CountryOverviewDTO, CountryOverviewViewModel>().ReverseMap();
            CreateMap<TopCustomerDTO, TopCustomerViewModel>().ReverseMap();
        }
    }
}
