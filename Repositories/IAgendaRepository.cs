using AgendaTel.Models;
using System.Collections;
using System.Collections.Generic;

namespace AgendaTel.Repositories
{
    public interface IAgendaRepository
    {
        IEnumerable<Contato> GetAll();
        Contato GetById(int id);
        Contato GetByNome(string nome);
        bool Update(Contato existingContato, Contato newContato);
        bool Delete(int id);
        Contato Create (Contato contato);
    }
}
