using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clininc_Management_System.Core.common
{
    public interface IDbContext
    {
        public DbConnection connection1 { get; }
    }
}
