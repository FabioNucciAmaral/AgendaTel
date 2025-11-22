using AgendaTel.Models;
using AgendaTel.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net.Mime;
using System.Net.Sockets;
using System.Windows.Markup;

namespace AgendaTel.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AgendaController : ControllerBase
    {
        private readonly IAgendaRepository agendaRepository;
        public AgendaController(IAgendaRepository agendaRepository)
        {
            this.agendaRepository = agendaRepository;
        }


        /// <summary>
        /// Endpoint para obter todos os contatos da agenda.
        /// </summary>
        /// <returns>Retorna uma lista com todos os contatos da Agenda.</returns>
        /// <response code="200">Retorna a lista completa de contatos.</response>
        [HttpGet("api/contatos")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Contato>))]
        public IActionResult GetAllContatos()
        {
            return Ok(agendaRepository.GetAll());
        }


        /// <summary>
        /// Endpoint para consultar um contato pelo seu ID.
        /// </summary>
        /// <returns>Retorna o contato cujo ID é o informado ou 404 caso não seja encontrado.</returns>
        /// <param name="id"> O ID do contato a ser consultado</param>
        /// <response code="200">O contato foi encontrado e seu conteúdo está disponível na resposta.</response>
        /// <response code="404">O contato não foi encontrado (ID inexistente na Agenda).</response>
        [HttpGet("api/contatos/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Contato))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetContatoById(int id)
        {
            var contato = agendaRepository.GetById(id);
            if (contato == null)
            {
                return NotFound();
            }
            return Ok(contato);
        }


        /// <summary>
        /// Endpoint para consultar um contato pelo seu nome.
        /// </summary>
        /// <returns>Retorna o contato cujo Nome é o informado na Query String ou 404 caso não seja encontrado.</returns>
        /// <param name="nome">O Nome do contato a ser consultado</param>
        /// <response code="200">O contato foi encontrado e seu conteúdo está disponível na resposta.</response>
        /// <response code="404">O contato não foi encontrado (Nome Inexistente na Agenda).</response>
        /// <response code="400">O parâmetro 'nome' é obrigatório.</response>
        [HttpGet("api/contatos/pesquisar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Contato))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetContatoByNome([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var contato = agendaRepository.GetByNome(nome);

            if (contato == null)
            {
                return NotFound($"Nenhum contato encontrado com o nome '{nome}'.");
            }

            return Ok(contato);
        }


        /// <summary>
        /// Endpoint para criar um novo contato na agenda.
        /// </summary>
        /// <returns>Retorna 409 (Confict) caso um contato com o mesmo ID ou Nome já exista ou 201 (Created) caso o cadastro tenha sido bem-sucedido.</returns>
        /// <param name="contato">Os dados do contato a ser criado</param>
        /// <response code="201">O contato foi criado com sucesso.</response>
        /// <response code="409">O contato não pôde ser criado porque já existe outro contato com o mesmo ID ou Nome</response>
        [HttpPost("api/contatos")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Contato))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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


        /// <summary>
        /// Endpoint para atualizar um contato existente na agenda.
        /// </summary>
        /// /// <returns>
        /// Retorna 400 (Bad Request) caso os IDs sejam divergentes, 404 (Not Found) caso o contato não exista
        /// ou 200 (Ok) caso o contato tenha sido encontrado e atualizado</returns>
        /// <param name="contato">Os dados do contato a ser atualizado</param>
        /// <param name="id">O ID do contato a ser atualizado</param>
        /// <response code="200">O contato foi atualizado com sucesso.</response>
        /// <response code="400">O ID informado na URL é diferente do ID do contato no corpo da requisição.</response>
        /// <response code="404">O contato não foi encontrado (ID inexistente na Agenda).</response>
        [HttpPut("api/contatos/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Contato))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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


        /// <summary>
        /// Endpoint para deletar um contato da agenda.
        /// </summary>
        /// <returns>Retorna 404 (Not Found) caso o contato não exista ou 204 (No Content) caso o contato tenha sido deletado com sucesso.</returns>
        /// <param name="id"></param>
        /// <response code="204">O contato foi deletado com sucesso.</response>
        /// <response code="404">O contato não foi encontrado (ID inexistente na Agenda).</response>
        [HttpDelete("api/contatos/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
