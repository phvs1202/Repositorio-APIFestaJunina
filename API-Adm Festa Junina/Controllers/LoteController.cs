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

        [HttpPost] //Cadastrar lote
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
    }
}
