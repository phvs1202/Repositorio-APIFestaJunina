namespace API_Adm_Festa_Junina.Model
{
    public class respostas
    {
        public int id { get; set; }
        public string resposta { get; set; }
        public DateTime data { get; set; }
        public int perguntas_id { get; set; }
        public int perguntas_tipo_perguntas_id { get; set;}
    }
}
