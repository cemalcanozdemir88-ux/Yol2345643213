using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AppDbContext _dbContext;

        public SettingsRepository(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<Settings> GetFirst()
        {
            return await _dbContext.Set<Settings>().FirstAsync();
        }
    }
}
