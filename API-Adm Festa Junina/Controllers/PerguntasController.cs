using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerguntasController : ControllerBase
    {
        private readonly DBFestaJuninaContext _dbContext;
        public PerguntasController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todas as perguntas
        public async Task<ActionResult<IEnumerable<perguntas>>> Get()
        {
            var perguntas = await _dbContext.perguntas.ToListAsync();
            return Ok(perguntas);
        }

        [HttpPost("CadastrarPerguntas")] //Cadastrar perguntas
        public async Task<ActionResult<respostas>> CriarRespostas([FromBody] List<perguntas> perguntas)
        {
            try
            {
                foreach (var pergunta in perguntas)
                {
                    _dbContext.perguntas.Add(pergunta);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}\nDetalhes: {ex.InnerException?.Message ?? "Nenhuma exceção interna"}");
            }
            return Ok(perguntas);
        }
    }
}
