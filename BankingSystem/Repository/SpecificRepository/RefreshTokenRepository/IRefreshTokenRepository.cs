using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.SpecificRepository.RefreshTokenRepository
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        public Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
