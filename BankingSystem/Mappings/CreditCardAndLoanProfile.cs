using AutoMapper;
using BankingSystem.Data.models;
using BankingSystem.DTOS.CreditCards_Loans.RequestDTOs;
using BankingSystem.DTOS.CreditCards_Loans.ResponseDto;

namespace BankingSystem.Mappings
{
    public class CreditCardAndLoanProfile : Profile
    {
        public CreditCardAndLoanProfile()
        {
            CreateMap<ApplyForLoanDto, Loan>()
                .ForMember(dest => dest.InterestRate, opt => opt.MapFrom(src => 14));
            CreateMap<Loan, LoanResponseDto>();

            CreateMap<IssueCreditCardDto, CreditCard>();
            CreateMap<CreditCard, CreditCardResponseDto>()
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)));
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;

            var lastFour = cardNumber.Substring(cardNumber.Length - 4);
            return $"**** **** **** {lastFour}";
        }
    }
}
