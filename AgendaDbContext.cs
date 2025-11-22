using AgendaTel.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaTel
{
    public class AgendaDbContext : DbContext
    {
        public DbSet<Contato> Agenda => Set<Contato>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\AgendaTel\\agenda.db");
            base.OnConfiguring(optionsBuilder);
        }
    }

}
