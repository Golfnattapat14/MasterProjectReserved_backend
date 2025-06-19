using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResDb;

namespace ResDb.Controllers;
public class DatabaseContext : DbContext
{
    public DbSet<MasterProjectReservedWord> MasterProjectReservedWord { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DatabaseContext()
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=203.147.62.46;Database=ThaihealthEGrantingDB;User ID=sa;Password=P@ssw0rd1234!@#$;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MasterProjectReservedWord>().ToTable("MasterProjectReservedWord");
        modelBuilder.Entity<MasterProjectReservedWord>().HasKey(r => r.Id);

    }
}