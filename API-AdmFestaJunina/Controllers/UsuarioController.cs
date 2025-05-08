using API_Adm_Festa_Junina.Model;
using API_Adm_Festa_Junina.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using API_Adm_Festa_Junina.Helpers;
using ZstdSharp.Unsafe;
using System.Text.Json;
using Org.BouncyCastle.Crypto.Prng;

namespace API_Adm_Festa_Junina.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly DBFestaJuninaContext _dbContext;
        public UsuarioController(DBFestaJuninaContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet] //Trazer todos os usuários
        public async Task<ActionResult<IEnumerable<usuario>>> Get()
        {
            var usuarios = await _dbContext.usuario.ToListAsync();
            return Ok(usuarios);
        }

        [HttpPut("AtualizarPerfil/{id}")]
        public async Task<ActionResult<usuario>> AtualizarPerfil(int id, [FromBody] usuario usuarioAtualizado)
        {
            var usuario = _dbContext.usuario.Where(i => i.id == id).FirstOrDefault();

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            usuario.nome = usuarioAtualizado.nome;
            usuario.caminho_foto = usuarioAtualizado.caminho_foto;
            usuario.email = usuarioAtualizado.email;
            usuario.senha = usuarioAtualizado.senha;
            usuario.telefone = usuarioAtualizado.telefone;
            usuario.perfil_id = usuarioAtualizado.perfil_id;
            
            _dbContext.usuario.Update(usuario);
            await _dbContext.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPost("LoginUser")] //Trazer um usuário especifico
        public IActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                var usuario = _dbContext.usuario.FirstOrDefault(u => u.email == login.Email);

                // Verifica se o usuario existe e se a senha está correta
                if (usuario == null || !PasswordHasher.VerifyPassword(login.Senha, usuario.senha))
                    return Unauthorized(new { message = "Email ou senha incorretos!" });

                return Ok(new
                {
                    message = "Login bem-sucedido!",
                    cliente = new
                    {
                        id = usuario.id,
                        nome = usuario.nome,
                        email = usuario.email,
                        telefone = usuario.telefone,
                        tipo_perfil = usuario.perfil_id
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao realizar login.", erro = ex.Message });
            }
        }

        [HttpPost("CadastroUser")] //Cadastrar Adminstrador ou Gerencia
        public async Task<ActionResult<usuario>> CriarUser([FromBody] usuario User)
        {
            try
            {
                var a = await _dbContext.usuario.Where(i => i.email == User.email).FirstOrDefaultAsync();
                if (a != null)
                    return BadRequest("Email já existente, crie outro.");

                User.senha = PasswordHasher.HashPassword(User.senha);

                _dbContext.usuario.Add(User);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(User);
        }

        [HttpDelete("ExcluirUsuario/{id}")]
        public async Task<ActionResult> ExcluirUsuario(int id)
        {
            var usuario = await _dbContext.usuario.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            _dbContext.usuario.Remove(usuario);
            await _dbContext.SaveChangesAsync();

            return Ok("Usuário excluído com sucesso.");
        }
    }
}
