using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace siteapi.DTOs
{
    public class ReservaIngressoDTO
    {
        public int UsuarioId { get; set; } // ID do usuário que está reservando
        public int ClienteId { get; set; } // ID do cliente associado ao ingresso
        public List<ItemReservaDTO> Itens { get; set; }
    }
}