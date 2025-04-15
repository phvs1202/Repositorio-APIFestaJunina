using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using siteapi.Models;
using siteapi.DTOs;
using siteapi.Repository;

namespace siteapi.Controllers
{
    [Route("api/ingresso")]
    [ApiController]
    public class IngressosController : ControllerBase
    {
        private readonly DBFestaJuninaContext _context;

        public IngressosController(DBFestaJuninaContext context)
        {
            _context = context;
        }

        // Reservar ingressos
        [HttpPost("reservar")]
        public IActionResult ReservarIngressos([FromBody] ReservaIngressoDTO reserva)
        {
            try
            {
                foreach (var item in reserva.Itens)
                {
                    // Buscar o Tipo de Ingresso
                    var tipoIngresso = _context.TipoIngresso.Find(item.TipoIngressoId);
                    if (tipoIngresso == null)
                    {
                        return BadRequest(new { message = $"Tipo de ingresso com ID {item.TipoIngressoId} não encontrado." });
                    }

                    for (int i = 0; i < item.Quantidade; i++)
                    {
                        var ingresso = new Ingresso
                        {
                            Tipo_Ingresso_Id = tipoIngresso.Id,
                            Usuario_Id = reserva.UsuarioId,
                            Data = DateTime.Now,
                            Status_Id = 2,  // Exemplo: "Não Validado"
                            Lote_Id = item.LoteId ?? 1,  // Usando LoteId da requisição, se não houver, usa 1
                            Entrada_Id = item.EntradaId ?? 1,  // Usando EntradaId da requisição, se não houver, usa 1
                            Cliente_Id = reserva.ClienteId,
                            QrCode = Guid.NewGuid().ToString() // Gerar um QR Code único
                        };

                        // Adicionar o ingresso ao contexto
                        _context.Ingresso.Add(ingresso);
                    }
                }

                // Salvar as alterações no banco de dados
                _context.SaveChanges();

                return Ok(new { message = "Ingressos reservados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao reservar ingresso.", erro = ex.Message });
            }
        }


        // Consultar ingressos por usuário
        [HttpGet("consulta/{clienteId}")]
        public IActionResult GetIngressosPorCliente(int clienteId)
        {
            var ingressos = _context.Ingresso
                .Where(i => i.Cliente_Id == clienteId)
                .Include(i => i.TipoIngresso) // Incluindo o TipoIngresso
                .Include(i => i.Status)  // Incluindo o Status
                .ToList();

            if (ingressos == null || !ingressos.Any())
            {
                return NotFound(new { message = "Nenhum ingresso encontrado para este cliente." });
            }

            return Ok(ingressos);
        }
    }
}
