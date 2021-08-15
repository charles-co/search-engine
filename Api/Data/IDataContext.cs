using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Document.Models;

namespace Document.Data
{
    public interface IDataContext
    {
        DbSet<Doc> Documents { get; init; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class DataContext: DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Doc> Documents { get; init; }
    }
}