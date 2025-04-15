using API_Adm_Festa_Junina.Helpers;
using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly DBFestaJuninaContext _context;

        public ClientesController(DBFestaJuninaContext context)
        {
            _context = context;
        }

        [HttpPost("register")] //Cadastro do cliente
        public IActionResult Register([FromBody] cliente cliente)
        {
            try
            {
                // Verifica se já existe um cliente com o mesmo email
                if (_context.cliente.Any(u => u.email == cliente.email))
                    return BadRequest(new { message = "Cliente já existe!" });

                // Criptografa a senha de forma segura
                cliente.senha = PasswordHasher.HashPassword(cliente.senha);

                // Adiciona o cliente no banco (incluindo telefone)
                _context.cliente.Add(cliente);
                _context.SaveChanges();

                return Ok(new { message = "Cliente registrado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao registrar cliente.", erro = ex.Message });
            }
        }

        [HttpPost("login")] //Login do cliente
        public IActionResult Login([FromBody] Model.LoginRequest login)
        {
            try
            {
                var cliente = _context.cliente.FirstOrDefault(u => u.email == login.Email);

                // Verifica se o cliente existe e se a senha está correta
                if (cliente == null || !PasswordHasher.VerifyPassword(login.Senha, cliente.senha))
                    return Unauthorized(new { message = "Email ou senha incorretos!" });

                return Ok(new
                {
                    message = "Login bem-sucedido!",
                    cliente = new
                    {
                        id = cliente.id,
                        nome = cliente.nome,
                        email = cliente.email,
                        telefone = cliente.telefone// inclui telefone na resposta do login, se quiser
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao realizar login.", erro = ex.Message });
            }
        }
    }
}
