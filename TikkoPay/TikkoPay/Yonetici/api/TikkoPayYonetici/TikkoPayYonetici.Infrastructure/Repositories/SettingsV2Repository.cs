using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Infrastructure.Repositories
{
    public class SettingsV2Repository : ISettingsV2Repository
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AppDbContext _dbContext;

        public SettingsV2Repository(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        private async Task<IEnumerable<SettingsV2>> GetAllAsync()
        {
            return await _dbContext.Set<SettingsV2>().ToListAsync();
        }

        public async Task<Settings> GetSettingsAsync()
        {
            var rows = await GetAllAsync();
            var values = rows
                .Where(item => !string.IsNullOrWhiteSpace(item.Key))
                .ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);

            var settings = new Settings();

            foreach (var property in typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.Name == nameof(Settings.Id) || !property.CanWrite)
                {
                    continue;
                }

                if (!values.TryGetValue(property.Name, out var value) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                try
                {
                    property.SetValue(settings, ConvertValue(value, property.PropertyType));
                }
                catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException)
                {
                    throw new InvalidOperationException($"SettingsV2 değeri geçersiz: {property.Name}", exception);
                }
            }

            return settings;
        }

        private static object ConvertValue(string value, Type type)
        {
            if (type == typeof(string))
            {
                return value;
            }

            if (type == typeof(Guid))
            {
                return Guid.Parse(value);
            }

            if (type == typeof(decimal))
            {
                return decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
            }

            if (type == typeof(int))
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
    }
}
