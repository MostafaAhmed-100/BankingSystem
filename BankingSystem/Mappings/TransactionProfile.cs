using AutoMapper;
using BankingSystem.DTOS.Transactions_TransfersDomain.Response_DTOs;
using System.Transactions;

namespace BankingSystem.Mappings
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionResponseDto>();
        }
    }
}
