using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace mlm_ref.Infrastructure.Database
{
public class DbConnectionFactory
    {
        private readonly IConfiguration _config;

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection(string? connectionName = null)
        {
            var connName = connectionName ?? "DefaultConnection";
            var connectionString = _config.GetConnectionString(connName)
                ?? throw new InvalidOperationException($"Connection string '{connName}' not found.");
            
            Console.WriteLine($"check connection string: {connectionString}");

            // detect database type (MySQL or SQL Server)
            if (connectionString.Contains("3310", StringComparison.OrdinalIgnoreCase) || connectionString.Contains("mysql", StringComparison.OrdinalIgnoreCase))
                return new MySqlConnection(connectionString);

            if (connectionString.Contains("sqlserver", StringComparison.OrdinalIgnoreCase))
                return new MySqlConnection(connectionString);

            throw new NotSupportedException("Unsupported database type in connection string.");
        }
    }
}