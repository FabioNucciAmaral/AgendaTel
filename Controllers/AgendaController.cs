using AgendaTel.Models;
using AgendaTel.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Windows.Markup;

namespace AgendaTel.Controllers
{
    public class AgendaController : ControllerBase
    {
        private readonly IAgendaRepository agendaRepository;
        public AgendaController(IAgendaRepository agendaRepository)
        {
            this.agendaRepository = agendaRepository;
        }

        [HttpGet("api/contatos")] 
        public IActionResult GetAllContatos()
        {
            return Ok(agendaRepository.GetAll());
        }

        [HttpGet("api/contatos/{id}")]
        public IActionResult GetContatoById(int id)
        {
            var contato = agendaRepository.GetById(id);
            if (contato == null)
            {
                return NotFound();
            }
            return Ok(contato);
        }

        [HttpGet("api/contatos/nome/{nome}")]
        public IActionResult GetContatoByNome(string nome)
        {
            var contato = agendaRepository.GetByNome(nome);

            if (contato == null)
            {
                return NotFound($"Nenhum contato encontrado com o nome '{nome}'.");
            }

            return Ok(contato);
        }

        [HttpPost("api/contatos")]
        public IActionResult CreateContato([FromBody] Contato contato)
        {
            var existingContato = agendaRepository.GetById(contato.Id);

            if (existingContato != null)
            {
                return Conflict(existingContato);
            }

            var existingNome = agendaRepository.GetByNome(contato.Nome);
            if (existingNome != null)
            {
                return Conflict($"O nome '{contato.Nome}' já existe.");
            }

            contato = agendaRepository.Create(contato);

            var url = Url.Action(
                action: nameof(GetContatoById), 
                controller: "Agenda",
                values: new { id = contato.Id },
                protocol: Request.Scheme);

            return Created(url, contato);
        }

        [HttpPut("api/contatos/{id}")]
        public IActionResult UpdateContato([FromBody] Contato contato, int id)
        {
            if (id != contato.Id)
            {
                return BadRequest();
            }

            var existingContato = agendaRepository.GetById(id);
            if (existingContato == null)
            {
                return NotFound();
            }

            agendaRepository.Update(existingContato, contato);
            return Ok(existingContato);

        }

        [HttpDelete("api/contatos/{id}")]
        public IActionResult DeleteContato(int id)
        {
            var existingContato = agendaRepository.GetById(id);
            if (existingContato == null)
            {
                return NotFound();
            }
            agendaRepository.Delete(id);
            return NoContent();
        }

    }
}
