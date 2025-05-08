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

        [HttpPost("CadastrarLote")]
        public async Task<ActionResult<lote>> CriarLote([FromBody] lote Lote)
        {
            try
            {
                // Verifica se as datas foram fornecidas
                if (string.IsNullOrEmpty(Lote.data_inicio) || string.IsNullOrEmpty(Lote.data_termino))
                    return BadRequest("As datas de início e término são obrigatórias.");

                // Verifica se a data de término é anterior à data de início
                if (string.Compare(Lote.data_termino, Lote.data_inicio) < 0)
                    return BadRequest("A data de término não pode ser anterior à data de início.");

                // Salva no banco de dados
                _dbContext.lote.Add(Lote);
                await _dbContext.SaveChangesAsync();

                return Ok(Lote);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
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

        // Método GET para buscar lote por id
        [HttpGet("{id}")]
        public async Task<ActionResult<lote>> GetLotePorId(int id)
        {
            var lote = await _dbContext.lote.FindAsync(id);

            if (lote == null)
            {
                return NotFound("Lote não encontrado.");
            }

            return Ok(lote);
        }
    }
}
