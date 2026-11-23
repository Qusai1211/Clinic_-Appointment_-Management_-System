using Clininc_Management_System.Core.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace Clininc_Management_System.infra.Common
{
    public class DbContext : IDbContext
    {
        private DbConnection? _connection;
        private readonly IConfiguration _configuration;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private DbConnection GetOpenConnection()
        {
            if (_connection == null)
            {
                var connStr = _configuration.GetConnectionString("DBConnectionString")
                    ?? _configuration["ConnectionStrings:DBConnectionString"];

                if (string.IsNullOrWhiteSpace(connStr) || connStr.Contains("YOUR_DB_PASSWORD") || connStr.Contains("YOUR_DB_USERNAME"))
                {
                    throw new InvalidOperationException("Connection string 'DBConnectionString' is not configured. Please provide a valid Oracle connection string in appsettings.json or environment variables.");
                }

                _connection = new OracleConnection(connStr);
                _connection.Open();
            }
            else if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        public DbConnection connection1 => GetOpenConnection();
        public DbConnection Connection => GetOpenConnection();

        public void Dispose()
        {
            if (_connection != null)
            {
                try
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                    _connection.Dispose();
                }
                catch
                {
                    // Ignore disposal errors
                }
                finally
                {
                    _connection = null;
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}
