using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RestauranteApp.ViewModels;
using RestauranteApp.Models.Enums;

namespace RestauranteApp.Models;

public class AppDbContext : IdentityDbContext<IdentityUser>
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    // Entidades del sistema
    public DbSet<Cliente> Clientes {get; set;}
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<Mesa> Mesas { get; set; }
    public DbSet<Reservacion> Reservaciones { get; set; }
    public DbSet<CategoriaMenu> CategoriasMenu { get; set; }
    public DbSet<ItemMenu> ItemsMenu { get; set; }
    
    // Entidades POS (nuevo modelo)
    public DbSet<Orden> Ordenes { get; set; }
    public DbSet<DetalleOrden> DetallesOrden { get; set; }
    public DbSet<Factura> Facturas { get; set; }
    public DbSet<DetalleFactura> DetalleFacturas { get; set; }
    
    // Entidades existentes (mantenidas por compatibilidad)
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallesPedido { get; set; }
    public DbSet<Gasto> Gastos { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Ingrediente> Ingredientes { get; set; }
    public DbSet<DatosNegocio> DatosNegocios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== SEMBRADO DE DATOS INICIALES ====================
        
        // Datos del negocio (solo si no existen)
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

        // NOTA: Las mesas se deben gestionar desde la interfaz de administración
        // No hacemos seed automático de mesas para evitar conflictos con datos existentes

        // ==================== CONFIGURACIÓN DE RELACIONES ====================

        // Relación Orden -> Mesa (1:N)
        modelBuilder.Entity<Orden>()
            .HasOne(o => o.Mesa)
            .WithMany(m => m.Ordenes)
            .HasForeignKey(o => o.MesaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Orden -> Empleado (N:1)
        modelBuilder.Entity<Orden>()
            .HasOne(o => o.Empleado)
            .WithMany()
            .HasForeignKey(o => o.EmpleadoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación Orden -> Cliente (N:1)
        modelBuilder.Entity<Orden>()
            .HasOne(o => o.Cliente)
            .WithMany()
            .HasForeignKey(o => o.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación DetalleOrden -> Orden (N:1)
        modelBuilder.Entity<DetalleOrden>()
            .HasOne(d => d.Orden)
            .WithMany(o => o.Detalles)
            .HasForeignKey(d => d.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación DetalleOrden -> ItemMenu (N:1)
        modelBuilder.Entity<DetalleOrden>()
            .HasOne(d => d.ItemMenu)
            .WithMany()
            .HasForeignKey(d => d.ItemMenuId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Factura -> Orden (1:1)
        modelBuilder.Entity<Factura>()
            .HasOne(f => f.Orden)
            .WithOne(o => o.Factura)
            .HasForeignKey<Factura>(f => f.OrdenId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Factura -> Cliente (N:1)
        modelBuilder.Entity<Factura>()
            .HasOne(f => f.Cliente)
            .WithMany()
            .HasForeignKey(f => f.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación DetalleFactura -> Factura (N:1)
        modelBuilder.Entity<DetalleFactura>()
            .HasOne(d => d.Factura)
            .WithMany(f => f.DetalleFacturas)
            .HasForeignKey(d => d.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);

        // ==================== CONFIGURACIÓN DE PRECISIÓN NUMÉRICA ====================

        modelBuilder.Entity<Orden>()
            .Property(o => o.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Orden>()
            .Property(o => o.MontoImpuesto).HasPrecision(18, 2);
        modelBuilder.Entity<Orden>()
            .Property(o => o.Descuento).HasPrecision(18, 2);
        modelBuilder.Entity<Orden>()
            .Property(o => o.Total).HasPrecision(18, 2);

        modelBuilder.Entity<DetalleOrden>()
            .Property(d => d.PrecioUnitario).HasPrecision(18, 2);

        modelBuilder.Entity<Factura>()
            .Property(f => f.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Factura>()
            .Property(f => f.MontoImpuesto).HasPrecision(18, 2);
        modelBuilder.Entity<Factura>()
            .Property(f => f.Descuento).HasPrecision(18, 2);
        modelBuilder.Entity<Factura>()
            .Property(f => f.MontoTotal).HasPrecision(18, 2);

        modelBuilder.Entity<DetalleFactura>()
            .Property(p => p.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<DetalleFactura>()
            .Property(p => p.Subtotal).HasPrecision(18, 2);

        // ==================== ÍNDICES PARA MEJORAR RENDIMIENTO ====================

        // Índice para buscar orden activa por mesa rápidamente
        modelBuilder.Entity<Orden>()
            .HasIndex(o => new { o.MesaId, o.Estado })
            .HasDatabaseName("IX_Orden_Mesa_Estado");

        // Índice para buscar detalles de una orden
        modelBuilder.Entity<DetalleOrden>()
            .HasIndex(d => d.OrdenId)
            .HasDatabaseName("IX_DetalleOrden_OrdenId");
    }
}
