using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RestauranteApp.ViewModels;

namespace RestauranteApp.Models;

public class AppDbContext : IdentityDbContext<IdentityUser>
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Cliente> Clientes {get; set;}
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Mesa> Mesas { get; set; }
    public DbSet<Reservacion> Reservaciones { get; set; }
    public DbSet<CategoriaMenu> CategoriasMenu { get; set; }
    public DbSet<ItemMenu> ItemsMenu { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallesPedido { get; set; }
    public DbSet<Factura> Facturas { get; set; }
    public DbSet<Gasto> Gastos { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Ingrediente> Ingredientes { get; set; }
    public DbSet<ItemMenuIngrediente> ItemMenuIngredientes { get; set; }
    public DbSet<DetalleFactura> DetalleFacturas { get; set; }
    public DbSet<DatosNegocio> DatosNegocios { get; set; }
    public DbSet<BackupRegistro> BackupRegistros { get; set; }
    public DbSet<MovimientoStock> MovimientosStock { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sembrar datos para la tabla DatosNegocio
        modelBuilder.Entity<DatosNegocio>().HasData(
            new DatosNegocio
            {
                DatosNegocioId = 1,
                Nombre = "Nombre de tu Restaurante",
                Telefono = "Tu Teléfono",
                Ruc = "Tu RUC",
                DireccionNegocio = "Tu Dirección"
            }
        );

        modelBuilder.Entity<DetalleFactura>()
            .HasOne(d => d.Factura)
            .WithMany(f => f.DetalleFacturas)
            .HasForeignKey(d => d.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DetalleFactura>()
            .Property(p => p.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<DetalleFactura>()
            .Property(p => p.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Factura>()
            .Property(p => p.MontoTotal).HasPrecision(18, 2);

        // Configuración de ItemMenuIngrediente
        modelBuilder.Entity<ItemMenuIngrediente>()
            .HasOne(imi => imi.ItemMenu)
            .WithMany(im => im.ItemMenuIngredientes)
            .HasForeignKey(imi => imi.ItemMenuId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemMenuIngrediente>()
            .HasOne(imi => imi.Ingrediente)
            .WithMany()
            .HasForeignKey(imi => imi.IngredienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ItemMenuIngrediente>()
            .Property(p => p.Cantidad).HasPrecision(18, 3);

        // Configuración de MovimientoStock
        modelBuilder.Entity<MovimientoStock>()
            .HasOne(m => m.Ingrediente)
            .WithMany(i => i.MovimientosStock)
            .HasForeignKey(m => m.IngredienteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovimientoStock>()
            .HasOne(m => m.Factura)
            .WithMany()
            .HasForeignKey(m => m.FacturaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MovimientoStock>()
            .HasOne(m => m.DetalleFactura)
            .WithMany()
            .HasForeignKey(m => m.DetalleFacturaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MovimientoStock>()
            .Property(p => p.Cantidad).HasPrecision(18, 3);
    }
}
