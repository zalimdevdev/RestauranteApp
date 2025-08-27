using Microsoft.EntityFrameworkCore;

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

}