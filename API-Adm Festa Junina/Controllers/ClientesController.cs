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
        private readonly IEmailSender _emailSender;

        public ClientesController(DBFestaJuninaContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpPost("CadastrarCliente")] //Cadastro do cliente
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

        [HttpPost("LoginCliente")] //Login do cliente
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

        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] string email)
        {
            var cliente = _context.cliente.FirstOrDefault(c => c.email == email);
            if (cliente == null)
                return NotFound(new { message = "E-mail não encontrado." });

            // Gera nova senha aleatória
            var novaSenha = Guid.NewGuid().ToString().Substring(0, 8); // 8 caracteres

            // Hash da nova senha
            var senhaHash = PasswordHasher.HashPassword(novaSenha);

            // Atualiza no banco
            cliente.senha = senhaHash;
            await _context.SaveChangesAsync();

            // Envia a nova senha por e-mail
            string mensagem = $"<h1>EMAIL DE RECUPERAÇÃO DE CONTA.</h1>Olá, segue neste E-mail a sua nova senha para acessar sua conta novamente.<br>[SENHA NOVA]: <strong style='font-family:consolas;'>{novaSenha}</strong><br><strong>Nunca compartilhe sua senha nova com ninguém!</strong> Nós <strong>nunca vamos pedir suas credenciais por e-mail,</strong> qualquer e-mail relacionado, além deste, <strong>desconfie.</strong><br><p style='font-size:25px;'><i>E-mail seguro.</i></p><br><img src='https://logodownload.org/wp-content/uploads/2019/08/senai-logo.png' alt='Logo SENAI' style='max-width:50%; height:auto;'/>";

            await _emailSender.SendEmailAsync(cliente.email, "FESTA JUNINA SENAI 2025 - Nova senha de acesso da conta.", mensagem);

            return Ok(new { message = "Nova senha enviada para seu e-mail." });
        }
    }
}
