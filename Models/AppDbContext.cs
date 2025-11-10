 
using Microsoft.EntityFrameworkCore;
using RestauranteApp.ViewModels;

namespace RestauranteApp.Models;

public class AppDbContext : DbContext
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
    public DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public DbSet<DatosNegocio> DatosNegocios { get; set; }



        // --- AGREGAR ESTE MÉTODO ---
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sembrar datos para la tabla DatosNegocio
        modelBuilder.Entity<DatosNegocio>().HasData(
            new DatosNegocio
            {
                DatosNegocioId = 1, // Debes especificar el ID manualmente
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
    }

}
