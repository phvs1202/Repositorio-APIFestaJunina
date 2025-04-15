using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.Sig;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngressoController : ControllerBase
    {
        private readonly DBFestaJuninaContext _dbContext;
        public IngressoController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todos os ingressos
        public async Task<ActionResult<IEnumerable<ingresso>>> Get()
        {
            var lotes = await _dbContext.ingresso.ToListAsync();
            return Ok(lotes);
        }

        [HttpPut("{id}")] //Alterar status dos ingressos
        public async Task<ActionResult<ingresso>> Atualizar(int id, [FromBody] ingresso ingresso)
        {
            var ingressoAtual = await _dbContext.ingresso.FindAsync(id);

            if (ingresso == null)
                return NotFound();

            _dbContext.Entry(ingressoAtual).CurrentValues.SetValues(ingresso);
            await _dbContext.SaveChangesAsync();

            return Ok(ingresso);
        }
    }
}
