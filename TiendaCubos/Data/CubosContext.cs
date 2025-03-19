using Microsoft.EntityFrameworkCore;
using TiendaCubos.Models;

namespace TiendaCubos.Data
{
    public class CubosContext: DbContext
    {
        public CubosContext(DbContextOptions<CubosContext> options): base(options) { }

        public DbSet<Cubo> Cubo { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Compra> Compras { get; set; }

    }
}
