using API_Adm_Festa_Junina.Model;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;

namespace API_Adm_Festa_Junina.Repository
{
    public class DBFestaJuninaContext : DbContext
    {
        public DBFestaJuninaContext(DbContextOptions<DBFestaJuninaContext> options) : base(options)
        {
        }

        public DbSet<ingresso> ingresso { get; set; }
        public DbSet<lote> lote { get; set; }
        public DbSet<pedidos> pedidos { get; set; }
        public DbSet<perguntas> perguntas { get; set; }
        public DbSet<respostas> respostas { get; set; }
        public DbSet<usuario> usuario { get; set; }
        public DbSet<cliente> cliente { get; set; }
        public DbSet<entrada> entrada{ get; set; }
    }
}
