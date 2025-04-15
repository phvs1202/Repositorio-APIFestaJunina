namespace API_Adm_Festa_Junina.Model
{
    public class entrada
    {
        public int id { get; set; }
        public DateTime data { get; set; }
        public int usuario_id { get; set; }
        public int usuario_perfil_id { get; set; }
    }
}
