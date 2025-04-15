namespace API_Adm_Festa_Junina.Model
{
    public class perguntas
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int tipo_perguntas_id { get; set; }
        public int usuario_id { get; set; }
        public int usuario_perfil_id { get; set; }
    }
}
