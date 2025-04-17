using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;

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

        [HttpGet("Contagem")] //Contagem de ingressos
        public async Task<ActionResult<IEnumerable<ingresso>>> ContagemIngressos()
        {
            var lotes = await _dbContext.ingresso.ToListAsync();
            return Ok(lotes.Count());
        }

        [HttpPost("Reserva")] //Reservar ingresso
        public async Task<ActionResult<ingresso>> CriarUser([FromBody] List<ingresso> ingresso)
        {
            try
            {
                var novoGuid = Guid.NewGuid(); //Para diferenciar cada pedido de ingresso, implementei um GUID

                var lote = _dbContext.lote.Where(i => i.ativo == 1).FirstOrDefault(); //Vê qual lote está ativo
                var listaIngressos = await _dbContext.ingresso.ToListAsync(); //Lista de ingressos

                foreach (var ingressos in ingresso)
                {
                    ingressos.guid = novoGuid;
                    ingressos.status_id = 1;
                    _dbContext.ingresso.Add(ingressos); 
                }

                await _dbContext.SaveChangesAsync();

                var ingressosDoPedido = await _dbContext.ingresso
                    .Where(i => i.guid == novoGuid)
                    .ToListAsync();

                var valorTotal = ingressosDoPedido.Count() * lote.valor_un;
                var clienteId = ingressosDoPedido.Select(i => i.cliente_id).FirstOrDefault();

                var pedidos = new pedidos
                {
                    data = DateTime.Now,
                    valor = valorTotal,
                    cliente_id = clienteId,
                    status_id = 1,
                    guid = novoGuid
                };

                _dbContext.pedidos.Add(pedidos);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}, {ex.InnerException.Message}");
            }
            return Ok(ingresso);
        }

        [HttpGet("Consultar/{id}")] //Consultar ingresso específico
        public async Task<ActionResult<IEnumerable<ingresso>>> consultar(int id)
        {
            var ingresso = await _dbContext.ingresso.Where(i => i.cliente_id == id).ToListAsync();
            return Ok(ingresso);
        }
    }
}