namespace API_Adm_Festa_Junina.Model
{
    public class pedidos
    {
        public int id { get; set; }
        public DateTime data { get; set; }
        public decimal valor { get; set; }
        public int cliente_id { get; set; }
        public int status_id { get; set; }
        public Guid guid { get; set; }
    }
}
