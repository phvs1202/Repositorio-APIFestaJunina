using Org.BouncyCastle.Asn1.Crmf;

namespace API_Adm_Festa_Junina.Model
{
    public class usuario
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string caminho_foto { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string telefone {get; set;}
        public int perfil_id { get; set; }
        
    }
}
