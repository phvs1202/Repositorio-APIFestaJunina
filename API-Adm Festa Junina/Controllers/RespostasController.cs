using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using System.Collections.Immutable;
using System.Runtime;
using ZstdSharp.Unsafe;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RespostasController : ControllerBase
    {
        private readonly DBFestaJuninaContext _dbContext;
        public RespostasController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todas as respostas
        public async Task<ActionResult<IEnumerable<respostas>>> Get()
        {
            var respostas = await _dbContext.respostas.ToListAsync();
            return Ok(respostas);
        }

        [HttpPost("CadastrarRespostas")] //Cadastrar respostas
        public async Task<ActionResult<respostas>> CriarRespostas([FromBody] List<respostas> respostas)
        {
            try
            {
                foreach(var resposta in respostas)
                {
                    _dbContext.respostas.Add(resposta);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}\nDetalhes: {ex.InnerException?.Message ?? "Nenhuma exceção interna"}");
            }
            return Ok(respostas);
        }

        [HttpGet("ContagemRespostas")] //Contagem de respostas
        public async Task<ActionResult<IEnumerable<respostas>>> Contagem()
        {
            var perguntas  = await _dbContext.perguntas.Where(i => i.tipo_perguntas_id == 2).ToListAsync();

            var resultado = new List<object>();

            foreach(var pergunta in perguntas)
            {
                var respostas = await _dbContext.respostas
                    .Where(i => i.perguntas_id == pergunta.id)
                    .ToListAsync();

                var otimas = respostas.Count(i => i.resposta == "Ótimo");
                var boas = respostas.Count(i => i.resposta == "Bom");
                var meh = respostas.Count(i => i.resposta == "Meh");
                var ruins = respostas.Count(i => i.resposta == "Ruim");

                resultado.Add(new
                {
                    pergunta = pergunta.nome,
                    otimas,
                    boas,
                    meh,
                    ruins
                });
            }

            return Ok(resultado);
        }
    }
}
