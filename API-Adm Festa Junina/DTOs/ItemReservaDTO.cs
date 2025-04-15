using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace siteapi.DTOs
{
    public class ItemReservaDTO
    {
        public int TipoIngressoId { get; set; }
        public int Quantidade { get; set; }
        public int? LoteId { get; set; } // Opcional
        public int? EntradaId { get; set; } // Opcional
    }
}