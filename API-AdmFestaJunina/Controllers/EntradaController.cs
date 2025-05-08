using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntradaController : ControllerBase
    {
        private readonly DBFestaJuninaContext _dbContext;
        public EntradaController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todas as entradas
        public async Task<ActionResult<IEnumerable<entrada>>> Get()
        {
            var entrada = await _dbContext.entrada.ToListAsync();
            return Ok(entrada);
        }

        [HttpGet("ContagemEntrada")] //Contar número de presentes e ausentes
        public async Task<ActionResult<IEnumerable<entrada>>> Contagem()
        {
            var entrada = await _dbContext.entrada.ToListAsync();
            var ingressos = await _dbContext.ingresso.ToListAsync();

            var numeroPresentes = ingressos.Count - entrada.Count;

            return Ok(new
            {
                message = "Número de pessoas presentes na festa",
                NumeroDeIngressosVendidos = ingressos.Count,
                NumeroDePresentes = numeroPresentes
            });
        }
    }
}
