using Microsoft.EntityFrameworkCore;
using PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models;

namespace PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Define a DbSet for Claims
        public DbSet<Claims> Claims { get; set; }
    }
}
