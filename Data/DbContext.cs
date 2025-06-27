using Microsoft.EntityFrameworkCore;
using ResDb;

namespace ResDb;

public class DatabaseContext : DbContext
{
    public DbSet<MasterProjectReservedWord_BK> MasterProjectReservedWord_BK { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}