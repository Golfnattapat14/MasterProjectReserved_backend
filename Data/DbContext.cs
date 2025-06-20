using Microsoft.EntityFrameworkCore;
using ResDb;

namespace ResDb;

public class DatabaseContext : DbContext
{
    public DbSet<MasterProjectReservedWord> MasterProjectReservedWord { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}