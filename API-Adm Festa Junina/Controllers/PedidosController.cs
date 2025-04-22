using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly DBFestaJuninaContext _dbContext;
        public PedidosController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todos os pedidos
        public async Task<ActionResult<IEnumerable<pedidos>>> Get()
        {
            var pedidos = await _dbContext.pedidos.ToListAsync();
            return Ok(pedidos);
        }

        [HttpPut("AlterarStatus/{id}")] //Alterar status dos pedidos
        public async Task<ActionResult<pedidos>> Atualizar(int id, [FromBody] pedidos pedidos)
        {
            var pedidoAtual = await _dbContext.pedidos.FindAsync(id);

            if (pedidos == null)
                return NotFound();

            _dbContext.Entry(pedidoAtual).CurrentValues.SetValues(pedidos);
            await _dbContext.SaveChangesAsync();

            return Ok(pedidos);
        }

        [HttpGet("ContagemPedidos")] //Contagem de pedidos
        public async Task<ActionResult<IEnumerable<pedidos>>> ContagemPedidos()
        {
            var pedidos = await _dbContext.pedidos.ToListAsync();
            return Ok(pedidos.Count());
        }
    }
}
