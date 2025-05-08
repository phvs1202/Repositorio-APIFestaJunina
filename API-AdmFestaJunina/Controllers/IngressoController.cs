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
using QRCoder;
using System.Drawing.Imaging;

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

        [HttpPut("AlterarStatus/{id}")] //Alterar status do ingresso
        public async Task<ActionResult<ingresso>> Atualizar(int id, [FromBody] ingresso ingresso)
        {
            var ingressoAtual = await _dbContext.ingresso.FindAsync(id);

            if (ingressoAtual == null)
                return NotFound();

            if (ingressoAtual.status_id== 1)
            {
                ingressoAtual.status_id = 2;
            }
            else
            {
                return BadRequest("O status do ingresso não pode ser alterado.");
            }

            await _dbContext.SaveChangesAsync();
            return Ok(ingressoAtual);
        }

        [HttpGet("ContagemIngressos")] //Contagem de ingressos
        public async Task<ActionResult<IEnumerable<ingresso>>> ContagemIngressos()
        {
            var lotes = await _dbContext.ingresso.ToListAsync();
            return Ok(lotes.Count());
        }

        [HttpGet("ContagemIngressosPorTipo")] // Contagem de ingressos por tipo
        public async Task<ActionResult<object>> ContagemIngressosPorTipo()
        {
            var contagemIngressos = await _dbContext.ingresso
                .GroupBy(i => i.tipo_ingresso_id)
                .Select(g => new
                {
                    Tipo = g.Key,
                    Quantidade = g.Count()
                }).ToListAsync();

            // Montando o JSON de resposta
            var resultado = new
            {
                Colaborador = contagemIngressos.FirstOrDefault(x => x.Tipo == 1)?.Quantidade ?? 0,
                Comunidade = contagemIngressos.FirstOrDefault(x => x.Tipo == 2)?.Quantidade ?? 0,
                Crianca = contagemIngressos.FirstOrDefault(x => x.Tipo == 3)?.Quantidade ?? 0,
                Familiar = contagemIngressos.FirstOrDefault(x => x.Tipo == 4)?.Quantidade ?? 0,
                Aluno = contagemIngressos.FirstOrDefault(x => x.Tipo == 5)?.Quantidade ?? 0
            };

            return Ok(resultado);
        }


        [HttpPost("ReservaIngressos")] //Reservar ingresso
        public async Task<ActionResult<ingresso>> CriarUser([FromBody] List<ingresso> ingresso)
        {
            try
            {
                var novoGuid = Guid.NewGuid(); // Para diferenciar cada pedido de ingresso, implementei um GUID
                var lote = _dbContext.lote.Where(i => i.ativo == 1).FirstOrDefault(); // Vê qual lote está ativo
                if (lote == null)
                {
                    return BadRequest("Não há lote ativo disponível.");
                }

                foreach (var ingressos in ingresso)
                {
                    var idCliente = ingressos.cliente_id;
                    var idTipo = ingressos.tipo_ingresso_id;
                    var cliente = _dbContext.cliente.Where(i => i.id == idCliente).FirstOrDefault();
                    var tipo = _dbContext.tipo_ingresso.Where(i => i.id == idTipo).FirstOrDefault();

                    if (cliente == null || tipo == null)
                    {
                        return BadRequest("Cliente ou tipo de ingresso inválido.");
                    }

                    string guid = Guid.NewGuid().ToString();
                    var dataFormatada = DateTime.Now.ToString("ddMMyyyyHHmmss");
                    var conteudoCodigo = $"792-CodigoUnico-{guid}-Cliente-{cliente.nome}-HoraEdia{dataFormatada}-TipoDoIngresso-{tipo.nome}"; // Conteúdo do QR Code

                    ingressos.qrcode = conteudoCodigo;
                    ingressos.guid = novoGuid;
                    ingressos.status_id = 1;
                    _dbContext.ingresso.Add(ingressos);  // Adiciona o ingresso à base de dados
                }

                await _dbContext.SaveChangesAsync(); // Salva os ingressos no banco

                // Gerar o QR Code para cada ingresso
                foreach (var ingressos in ingresso)
                {
                    var conteudoCodigo = ingressos.qrcode;  // Use o conteúdo gerado para o ingresso específico

                    // Gerar o QR Code
                    QRCodeGenerator GeradorQR = new QRCodeGenerator();
                    var qrData = GeradorQR.CreateQrCode(conteudoCodigo, QRCodeGenerator.ECCLevel.Q);
                    using var qrCode = new QRCode(qrData);
                    using var qrImage = qrCode.GetGraphic(20);

                    // Garantir que o diretório existe
                    string pastaRaiz = Path.Combine(Directory.GetCurrentDirectory(), "QRCodeImagens");
                    if (!Directory.Exists(pastaRaiz))
                    {
                        Directory.CreateDirectory(pastaRaiz); // Cria a pasta se não existir
                    }

                    // Gerar nome único para o arquivo, baseado no conteúdo do QR Code
                    string nomeArquivo = $"{ingressos.qrcode}.png";  // Usa um GUID para garantir que o nome seja único
                    string caminhoCompleto = Path.Combine(pastaRaiz, nomeArquivo);

                    // Salvar a imagem do QR Code
                    qrImage.Save(caminhoCompleto, ImageFormat.Png);
                }

                // Criar o pedido
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
                await _dbContext.SaveChangesAsync();  // Salva o pedido no banco

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}\nDetalhes: {ex.InnerException?.Message ?? "Nenhuma exceção interna"}");
            }

            return Ok(ingresso);
        }


        [HttpGet("ConsultarIngresso/{id}")] //Consultar ingresso específico
        public async Task<ActionResult<IEnumerable<ingresso>>> consultar(int id)
        {
            var ingresso = await _dbContext.ingresso.Where(i => i.cliente_id == id).ToListAsync();
            return Ok(ingresso);
        }

        [HttpDelete("CancelarIngresso/{id}")]
        public async Task<ActionResult> CancelarIngresso(int id)
        {
            var ingresso = await _dbContext.ingresso.FindAsync(id);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            if (ingresso.status_id == 2)
                return BadRequest("Ingresso já foi validado e não pode ser cancelado.");

            // Marcar como cancelado (status_id = 3, por exemplo)
            ingresso.status_id = 3;

            _dbContext.Entry(ingresso).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return Ok("Ingresso cancelado com sucesso.");
        }
    }
}