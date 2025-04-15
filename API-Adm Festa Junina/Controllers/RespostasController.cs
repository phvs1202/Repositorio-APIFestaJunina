using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost] //Cadastrar respostas
        public async Task<ActionResult<respostas>> CriarRespostas([FromBody] respostas respostas)
        {
            try
            {
                _dbContext.respostas.Add(respostas);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(respostas);
        }
    }
}
