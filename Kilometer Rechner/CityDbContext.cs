

using System.IO;

using Kilometer_Rechner.Models;

using Microsoft.EntityFrameworkCore;

namespace Kilometer_Rechner
{
    public class CityDbContext : DbContext
    {
        public DbSet<CityModel> Cities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Initial Catalog=KilometerRechner;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}