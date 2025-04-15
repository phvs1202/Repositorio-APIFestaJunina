using System.Threading.Tasks;

public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario, string assunto, string conteudoHtml);
}
