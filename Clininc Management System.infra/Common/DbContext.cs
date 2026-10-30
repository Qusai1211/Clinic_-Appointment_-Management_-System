using Clininc_Management_System.Core.common;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.infra.Common
{
    public class DbContext:IDbContext
    {
        private DbConnection? connection;
        private readonly IConfiguration configuration;
        public DbContext(IConfiguration configuration1)
        {
            configuration = configuration1;
        }
        public DbConnection connection1
        {
            get
            {
                if (connection == null) {
                    connection = new OracleConnection(configuration["ConnectionStrings:DBConnectionString"]);
                    connection.Open();

                        }else if(connection.State != ConnectionState.Open)
                {
                    connection.Open();

                }
                return connection;
            }
        }

        internal static bool VerifyPassword(string password, string passwordhash, out bool needsRehash)
        {
            throw new NotImplementedException();
        }
    }
}
 