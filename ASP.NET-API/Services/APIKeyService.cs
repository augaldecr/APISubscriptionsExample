using ASP.NET_API.Data;
using Shared.Entities;

namespace ASP.NET_API.Services
{
    public class APIKeyService
    {
        private readonly ApplicationDbContext _context;

        public APIKeyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateKey(string userId, KeyType keyType)
        {
            var key = GenerateKey();

            var apiKey = new APIKey
            {
                Key = key,
                UserId = userId,
                KeyType = keyType,
                Active = true
            };

            await _context.APIKeys.AddAsync(apiKey);
            await _context.SaveChangesAsync();
        }

        public string GenerateKey() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
