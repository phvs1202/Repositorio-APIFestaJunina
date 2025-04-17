namespace API_Adm_Festa_Junina.Model
{
    public class lote
    {
        public int id { get; set; }
        public int qtd_total { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_termino { get; set; }
        public decimal valor_un { get; set; }
        public int usuario_id { get; set; }
        public int ativo { get; set; }
    }
}
