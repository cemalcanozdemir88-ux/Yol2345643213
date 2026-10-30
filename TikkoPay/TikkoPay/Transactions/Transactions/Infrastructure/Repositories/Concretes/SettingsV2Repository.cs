using Dapper;
using System.Globalization;
using System.Reflection;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;

namespace Transactions.Infrastructure.Repositories.Concretes
{
    public class SettingsV2Repository : ISettingsV2Repository
    {
        private readonly DapperContext _dapperContext;

        public SettingsV2Repository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<SettingsV2>> GetAllAsync()
        {
            using var connection = _dapperContext.CreateConnection();

            string query = @"
                            SELECT
                                *
                            FROM SettingsV2
                            ";

            var parameters = new DynamicParameters();

            return await connection.QueryAsync<SettingsV2>(query, parameters);
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

        public async Task<decimal> GetMaxFastTutar()
        {
            var settings = await GetSettingsAsync();
            return settings.MaxFastTutar;
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
