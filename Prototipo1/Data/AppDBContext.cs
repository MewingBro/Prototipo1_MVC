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

        public DbSet<Unidad> Unidad { get; set; }
        public DbSet<Producto> Producto { get; set; }

        public DbSet<TipoFactura> TipoFactura { get; set; }

        public DbSet<Factura> Factura { get; set; }

        public DbSet<FacturaProducto> FacturaProducto { get; set; }

        public DbSet<Inventario> Inventario { get; set; }

        public DbSet<Proyecto> Proyecto  { get; set; }

        public DbSet<Nivel> Nivel { get; set; }

        public DbSet<Aposento> Aposento { get; set; }

        public DbSet<Recinto> Recinto { get; set; }

        public DbSet<RecintoProducto> RecintoProducto { get; set; }

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

            modelBuilder.Entity<Unidad>().HasData(
                new Unidad { IdUnidad = 1, NombreUnidad = "BOLSA" },
                new Unidad { IdUnidad = 2, NombreUnidad = "CUBETA" },
                new Unidad { IdUnidad = 3, NombreUnidad = "M²" },
                new Unidad { IdUnidad = 4, NombreUnidad = "PIEZA" },
                new Unidad { IdUnidad = 5, NombreUnidad = "UNIDAD" }

                );

            modelBuilder.Entity<TipoFactura>().HasData(
                new TipoFactura { IdTipoFactura = 1, NombreTipoFactura = "Entrada" },
                new TipoFactura { IdTipoFactura = 2, NombreTipoFactura = "Salida" }
                );

        }

    }
}
