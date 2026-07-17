using Microsoft.EntityFrameworkCore;
using EducationCenter.Api.Models;
namespace EducationCenter.Api.Data
   
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
    }
}
