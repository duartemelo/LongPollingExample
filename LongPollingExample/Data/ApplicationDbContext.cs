using LongPollingExample.Entities;
using Microsoft.EntityFrameworkCore;

namespace LongPollingExample.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais, como chaves primárias compostas, índices, etc.
            base.OnModelCreating(modelBuilder);
        }
    }
}
