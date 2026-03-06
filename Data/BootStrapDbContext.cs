using Microsoft.EntityFrameworkCore;
using ExcelUploadApi.Data;

public class BootstrapDbContext : DbContext
{
    public BootstrapDbContext(DbContextOptions<BootstrapDbContext> options)
        : base(options) { }

    public DbSet<AppSecret> AppSecrets { get; set; }
}