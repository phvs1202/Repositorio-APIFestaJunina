using Org.BouncyCastle.Asn1.Mozilla;

namespace API_Adm_Festa_Junina.Model
{
    public class ingresso
    {
        public int id { get; set; }
        public string qrcode { get; set; }
        public DateTime data { get; set; }
        public int tipo_ingresso_id { get; set; }
        public int usuario_id { get; set; }
        public int lote_id { get; set; }
        public int status_id { get; set; }  
        public int cliente_id { get; set; }
        public Guid guid { get; set; }
        public decimal valor { get; set; }
    }
}
