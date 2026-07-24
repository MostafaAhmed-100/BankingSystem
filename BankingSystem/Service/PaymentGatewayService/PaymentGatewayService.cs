using BankingSystem.Constants;
using BankingSystem.DTOS.PaymentGateway.RequestDto;
using BankingSystem.DTOS.PaymentGateway.ResponseDto;
using BankingSystem.DTOS.PaymentGatewayDTOs.RequestDto;
using BankingSystem.DTOS.Shared;
using BankingSystem.Exceptions;
using BankingSystem.Repository.GenericRepository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Service.PaymentGatewayService
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentGatewayService> _logger;

        public PaymentGatewayService(IUnitOfWork unitOfWork, ILogger<PaymentGatewayService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<PaymentResultResponseDto>> ProcessExternalPaymentAsync(ChargeCardRequestDto requestDto)
        {
            if (requestDto.Amount <= 0)
            {
                throw new BadRequestException("Payment amount must be greater than zero.");
            }

            bool isAlreadyCharged = await _unitOfWork.Transactions.ExistsByReferenceIdAsync(requestDto.ReferenceId);
            if (isAlreadyCharged)
            {
                _logger.LogWarning("Idempotency Alert: Attempted double charge for ReferenceId {ReferenceId}.", requestDto.ReferenceId);
                throw new ConflictException("A payment for this order has already been processed.");
            }

            var creditCard = await _unitOfWork.CreditCards.GetCardByNumberAsync(requestDto.CardNumber, trackChanges: false);

            if (creditCard == null || !creditCard.IsActive)
            {
                _logger.LogWarning("Gateway payment failed: Card {CardNumber} not found or inactive.", MaskCardNumber(requestDto.CardNumber));
                throw new NotFoundException("Invalid card details or card is inactive.");
            }

            if (creditCard.CVV != requestDto.CVV ||
                creditCard.ExpireDate.Month != requestDto.ExpireDate.Month ||
                creditCard.ExpireDate.Year != requestDto.ExpireDate.Year)
            {
                _logger.LogWarning("Gateway payment failed: Authentication mismatch for Card {CardNumber}.", MaskCardNumber(requestDto.CardNumber));
                throw new UnauthorizedException("Invalid card authentication details. Please check your CVV and Expiry Date.");
            }

            if (creditCard.ExpireDate < DateTime.UtcNow)
            {
                throw new BadRequestException("This card has expired.");
            }

            if (requestDto.Amount > creditCard.Limit)
            {
                throw new BadRequestException("Payment amount exceeds the credit card limit.");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(creditCard.AccountNumber, trackChanges: true);

                if (account == null || !account.IsActive)
                {
                    throw new BadRequestException("The account associated with this card is inactive or not found.");
                }

                if (account.Balance < requestDto.Amount)
                {
                    throw new BadRequestException("Insufficient funds in the account.");
                }

                account.Balance -= requestDto.Amount;

                var paymentTransaction = new Data.models.Transaction
                {
                    Amount = requestDto.Amount,
                    Type = TransactionType.GatewayPayment,
                    TransactionDate = DateTime.UtcNow,
                    AccountNumber = account.AccountNumber,
                    ReferenceId = requestDto.ReferenceId
                };

                await _unitOfWork.Transactions.AddAsync(paymentTransaction);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Gateway payment of {Amount} processed successfully for Merchant: {StoreName}. RefId: {RefId}.",
                    requestDto.Amount, requestDto.StoreName, requestDto.ReferenceId);

                return new ApiResponseDto<PaymentResultResponseDto>
                {
                    Message = "Payment processed successfully.",
                    Data = new PaymentResultResponseDto
                    {
                        IsSuccessful = true,
                        TransactionId = paymentTransaction.Id,
                        Message = "Success"
                    }
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency conflict during gateway payment for Card {CardNumber}.", MaskCardNumber(requestDto.CardNumber));
                throw new ConflictException("Payment could not be processed due to a concurrent update. Please try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error during gateway payment for Merchant: {StoreName}.", requestDto.StoreName);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> RefundExternalPaymentAsync(RefundRequestDto requestDto)
        {
            if (requestDto.Amount <= 0)
            {
                throw new BadRequestException("Refund amount must be greater than zero.");
            }

            bool isAlreadyRefunded = await _unitOfWork.Transactions.ExistsByReferenceIdAsync(requestDto.OriginalTransactionId);

            if (isAlreadyRefunded)
            {
                _logger.LogWarning("Double Refund Alert: Attempted to refund Transaction {OriginalTxId} again.", requestDto.OriginalTransactionId);
                throw new ConflictException("This transaction has already been refunded.");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var originalTransaction = await _unitOfWork.Transactions.GetByIdAsync(requestDto.OriginalTransactionId, trackChanges: false);

                if (originalTransaction == null || originalTransaction.Type != "GatewayPayment")
                {
                    throw new NotFoundException("Original payment transaction not found.");
                }

                if (requestDto.Amount > originalTransaction.Amount)
                {
                    throw new BadRequestException("Refund amount cannot exceed the original payment amount.");
                }

                var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(originalTransaction.AccountNumber, trackChanges: true);
                if (account == null || !account.IsActive)
                {
                    throw new NotFoundException("Account associated with this transaction not found or inactive.");
                }

                account.Balance += requestDto.Amount;

                var refundTransaction = new Data.models.Transaction
                {
                    Amount = requestDto.Amount,
                    Type = TransactionType.Refund,
                    TransactionDate = DateTime.UtcNow,
                    AccountNumber = account.AccountNumber,
                    ReferenceId = requestDto.OriginalTransactionId 
                };

                await _unitOfWork.Transactions.AddAsync(refundTransaction);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Refund of {Amount} processed successfully for OriginalTxId: {OriginalTxId}.",
                    requestDto.Amount, requestDto.OriginalTransactionId);

                return new ApiResponseDto<string>
                {
                    Message = "Refund processed successfully.",
                    Data = null
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                throw new ConflictException("Refund could not be processed due to a concurrent update. Please try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #region Private Helper
        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;
            return new string('*', cardNumber.Length - 4) + cardNumber.Substring(cardNumber.Length - 4);
        }
        #endregion
    }
}
