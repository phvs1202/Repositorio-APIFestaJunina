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

        [HttpPut("AlterarStatus/{id}")] //Alterar status do pedido
        public async Task<ActionResult<pedidos>> Atualizar(int id, [FromBody] pedidos pedido)
        {
            var pedidoAtual = await _dbContext.pedidos.FindAsync(id);

            if (pedidoAtual == null)
                return NotFound();

            if (pedidoAtual.status_id == 1)
            {
                pedidoAtual.status_id = 2;
            }
            else
            {
                return BadRequest("O status do pedido não pode ser alterado.");
            }

            await _dbContext.SaveChangesAsync();
            return Ok(pedidoAtual);
        }


        [HttpGet("ContagemPedidos")] //Contagem de pedidos
        public async Task<ActionResult<IEnumerable<pedidos>>> ContagemPedidos()
        {
            var pedidos = await _dbContext.pedidos.ToListAsync();
            return Ok(pedidos.Count());
        }
    }
}
