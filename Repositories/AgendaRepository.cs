using AgendaTel.Models;
using System.Collections.Generic;
using System.Linq;

namespace AgendaTel.Repositories
{
    public class AgendaRepository : IAgendaRepository
    {

        private readonly AgendaDbContext dbContext;
        public AgendaRepository(AgendaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public IEnumerable<Contato> GetAll()
        {
            return dbContext.Agenda;
        }

        public Contato GetById(int id)
        {
            return dbContext.Agenda.Find(id);
        }
        public Contato Create(Contato agenda)
        {
            dbContext.Agenda.Add(agenda);
            dbContext.SaveChanges();
            return agenda;
        }

        public Contato GetByNome(string nome)
        {
            return dbContext.Agenda.FirstOrDefault(c =>
                c.Nome.ToLower() == nome.ToLower());
        }

        public bool Update(Contato existingContato, Contato newContato)
        {
            existingContato.Nome = newContato.Nome;
            existingContato.Telefone = newContato.Telefone;
            dbContext.Agenda.Update(existingContato);
            dbContext.SaveChanges();
            return true;
        }
        public bool Delete(int id)
        {
            var contato = dbContext.Agenda.Find(id);
            if (contato != null)
            {
                dbContext.Agenda.Remove(contato);
                dbContext.SaveChanges();

            }
            return true;


        }
    }
}
