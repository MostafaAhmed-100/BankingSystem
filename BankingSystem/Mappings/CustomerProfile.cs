using AutoMapper;
using BankingSystem.Data.models;
using BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs;

namespace BankingSystem.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<RegisterCustomerDto, Customer>();
        }
    }
}
