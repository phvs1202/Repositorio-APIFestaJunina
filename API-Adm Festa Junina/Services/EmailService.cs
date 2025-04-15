using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "mlsn.592ca79cb1a15199168a9e24bbb091f6b381af20ce6a914f18d1fad03e7ba884";

    public EmailService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string assunto, string conteudoHtml)
    {
        var data = new
        {
            from = new { email = "test-eqvygm0z2wdl0p7w.mlsender.net", name = "SENAI_FESTA_JUNINA" },
            to = new[] { new { email = destinatario } },
            subject = assunto,
            html = conteudoHtml
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.mailersend.com/v1/email", content);
        return response.IsSuccessStatusCode;
    }
}
