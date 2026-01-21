using Microsoft.EntityFrameworkCore;
using OmniChannel.API.Models;

namespace OmniChannel.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Client> Clients { get; set; }
    }
}
