using AutoMapper;
using BankingSystem.Data.models;
using BankingSystem.DTOS.Account_DTOs.Request_DTOs;
using BankingSystem.DTOS.Account_DTOs.ResponseDto;

namespace BankingSystem.Mappings
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountResponseDto>();
            CreateMap<Account, AccountWithTransactionsDto>();
        }
    }
}
