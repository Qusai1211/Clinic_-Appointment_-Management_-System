using System.Data.Common;

namespace Clininc_Management_System.Core.common
{
    public interface IDbContext : IDisposable
    {
        DbConnection connection1 { get; }
    }
}
