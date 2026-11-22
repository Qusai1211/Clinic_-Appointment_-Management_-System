using Clinic_Managemeant.Core.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic_Management.Infra.Common
{
    public class DbContext: IDbContext
    {
        private DbConnection connection;
        private readonly IConfiguration _configuration;
        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new OracleConnection(_configuration["ConnectionStrings:DBConnectionString"]);
                }
                else if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return connection;
            }
        }

    }
}
