using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoteController : Controller
    {
        private readonly DBFestaJuninaContext _dbContext;
        public LoteController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todos os lotes
        public async Task<ActionResult<IEnumerable<lote>>> Get()
        {
            var lotes = await _dbContext.lote.ToListAsync();
            return Ok(lotes);
        }

        [HttpPost("CadastrarLote")] //Cadastrar lote
        public async Task<ActionResult<lote>> CriarLote([FromBody] lote Lote)
        {
            try
            {
                Lote.data_inicio = DateTime.Now;
                Lote.data_termino = DateTime.Now;

                _dbContext.lote.Add(Lote);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return Ok(Lote);
        }

        [HttpPut("EditarLote/{id}")]
        public async Task<ActionResult<lote>> EditarLote(int id, [FromBody] lote loteAtualizado)
        {
            var loteExistente = await _dbContext.lote.FindAsync(id);

            if (loteExistente == null)
                return NotFound("Lote não encontrado.");

            loteExistente.qtd_total = loteAtualizado.qtd_total;
            loteExistente.data_inicio = loteAtualizado.data_inicio;
            loteExistente.data_termino = loteAtualizado.data_termino;
            loteExistente.valor_un = loteAtualizado.valor_un;
            loteExistente.usuario_id = loteAtualizado.usuario_id;
            loteExistente.ativo = loteAtualizado.ativo;

            _dbContext.Entry(loteExistente).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok(loteExistente);
        }

        [HttpDelete("DeletarLote/{id}")]
        public async Task<ActionResult> DeletarLote(int id)
        {
            var lote = await _dbContext.lote.FindAsync(id);

            if (lote == null)
                return NotFound("Lote não encontrado.");

            _dbContext.lote.Remove(lote);
            await _dbContext.SaveChangesAsync();

            return Ok("Lote deletado com sucesso.");
        }
    }
}
