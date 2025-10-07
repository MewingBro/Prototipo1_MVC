using Microsoft.EntityFrameworkCore;
using Prototipo1.Models;

namespace Prototipo1.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) 
        {
                
        }

        public DbSet<Familia> Familia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Familia>().HasData(
                new Familia { IdFamilia = 1, NombreFamilia = "BAÑOS" },
                new Familia { IdFamilia = 2, NombreFamilia = "BASES" },
                new Familia { IdFamilia = 3, NombreFamilia = "MADERAS" },
                new Familia { IdFamilia = 4, NombreFamilia = "PAREDES" },
                new Familia { IdFamilia = 5, NombreFamilia = "PAREDES Y PISO" },
                new Familia { IdFamilia = 6, NombreFamilia = "PARQUEO" },
                new Familia { IdFamilia = 7, NombreFamilia = "SEGURIDAD" },
                new Familia { IdFamilia = 8, NombreFamilia = "SOPORTE" },
                new Familia { IdFamilia = 9, NombreFamilia = "SUELOS" },
                new Familia { IdFamilia = 10, NombreFamilia = "VARIOS" },
                new Familia { IdFamilia = 11, NombreFamilia = "Vidrios" },
                new Familia { IdFamilia = 12, NombreFamilia = "PUERTAS " }

                );
        }
    }
}
