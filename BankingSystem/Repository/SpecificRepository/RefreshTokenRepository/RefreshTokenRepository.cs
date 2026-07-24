using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.SpecificRepository.RefreshTokenRepository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _AppDbcontext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
