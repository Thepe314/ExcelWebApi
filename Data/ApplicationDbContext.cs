using ExcelUploadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelUploadApi.Data
{
    

    public class ApplicationDbContext :DbContext
{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options){}
    
        public DbSet<Subject> Subjects{get;set;}

         public DbSet<Student> Students {get;set;}

          public DbSet<AppSecret> AppSecrets { get; set; } = null!;

       
    
}
}


