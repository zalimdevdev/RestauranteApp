using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RestauranteApp.Models;

namespace RestauranteApp.Services;

public class BackupService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly string _backupFolder;

    public BackupService(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        _backupFolder = Path.Combine(_environment.WebRootPath, "backups");
        
        if (!Directory.Exists(_backupFolder))
        {
            Directory.CreateDirectory(_backupFolder);
        }
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<BackupRegistro> CrearBackupCompletoAsync(string? descripcion = null, string? creadoPor = null)
    {
        var backupData = new BackupDataDto
        {
            FechaBackup = DateTime.Now,
            TipoBackup = "Completo",
            Clientes = await _context.Clientes.Select(c => new ClienteDto
            {
                ClienteId = c.ClienteId,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Telefono = c.Telefono,
                Cedula = c.Cedula,
                Direccion = c.Direccion,
                FechaRegistro = c.FechaRegistro
            }).ToListAsync(),
            Empleados = await _context.Empleados.Select(e => new EmpleadoDto
            {
                EmpleadoId = e.EmpleadoId,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Puesto = e.Puesto,
                FechaContratacion = e.FechaContratacion,
                Salario = e.Salario,
                Telefono = e.Telefono
            }).ToListAsync(),
            Mesas = await _context.Mesas.Select(m => new MesaDto
            {
                MesaId = m.MesaId,
                NumeroMesa = m.NumeroMesa,
                Capacidad = m.Capacidad,
                Estado = m.Estado
            }).ToListAsync(),
            Reservaciones = await _context.Reservaciones.Select(r => new ReservacionDto
            {
                ReservacionId = r.ReservacionId,
                ClienteId = r.ClienteId,
                MesaId = r.MesaId,
                FechaHora = r.FechaHora,
                NumeroPersonas = r.NumeroPersonas,
                Estado = r.Estado
            }).ToListAsync(),
            CategoriasMenu = await _context.CategoriasMenu.Select(cm => new CategoriaMenuDto
            {
                CategoriaId = cm.CategoriaId,
                NombreCategoria = cm.NombreCategoria,
                DescripcionCategoria = cm.DescripcionCategoria,
                Estado = cm.Estado
            }).ToListAsync(),
            ItemsMenu = await _context.ItemsMenu.Select(im => new ItemMenuDto
            {
                ItemId = im.ItemId,
                NombreItem = im.NombreItem,
                Descripcion = im.Descripcion,
                Precio = im.Precio,
                Estado = im.Estado,
                ImagenUrl = im.ImagenUrl,
                CategoriaId = im.CategoriaId
            }).ToListAsync(),
            Pedidos = await _context.Pedidos.Select(p => new PedidoDto
            {
                PedidoId = p.PedidoId,
                MesaId = p.MesaId,
                EmpleadoId = p.EmpleadoId,
                FechaHora = p.FechaHora,
                EstadoPedido = p.EstadoPedido
            }).ToListAsync(),
            DetallesPedido = await _context.DetallesPedido.Select(dp => new DetallePedidoDto
            {
                DetalleId = dp.DetalleId,
                PedidoId = dp.PedidoId,
                ItemId = dp.ItemId,
                Cantidad = dp.Cantidad,
                PrecioUnitario = dp.PrecioUnitario
            }).ToListAsync(),
            Facturas = await _context.Facturas.Select(f => new FacturaDto
            {
                FacturaId = f.FacturaId,
                ClienteId = f.ClienteId,
                MesaId = f.MesaId,
                MontoTotal = f.MontoTotal,
                MetodoPago = f.MetodoPago,
                FechaPago = f.FechaPago
            }).ToListAsync(),
            DetalleFacturas = await _context.DetalleFacturas.Select(df => new DetalleFacturaDto
            {
                DetalleFacturaId = df.DetalleFacturaId,
                FacturaId = df.FacturaId,
                ItemId = df.ItemId,
                Cantidad = df.Cantidad,
                PrecioUnitario = df.PrecioUnitario,
                Subtotal = df.Subtotal
            }).ToListAsync(),
            Gastos = await _context.Gastos.Select(g => new GastoDto
            {
                GastoId = g.GastoId,
                Descripcion = g.Descripcion,
                Monto = g.Monto,
                FechaGasto = g.FechaGasto,
                CategoriaGasto = g.CategoriaGasto
            }).ToListAsync(),
            Proveedores = await _context.Proveedores.Select(p => new ProveedorDto
            {
                ProveedorId = p.ProveedorId,
                NombreProveedor = p.NombreProveedor,
                Contacto = p.Contacto,
                Telefono = p.Telefono,
                Email = p.Email
            }).ToListAsync(),
            Ingredientes = await _context.Ingredientes.Select(i => new IngredienteDto
            {
                IngredienteId = i.IngredienteId,
                NombreIngrediente = i.NombreIngrediente,
                CantidadStock = i.CantidadStock,
                UnidadMedida = i.UnidadMedida,
                ProveedorId = i.ProveedorId
            }).ToListAsync(),
            DatosNegocios = await _context.DatosNegocios.Select(dn => new DatosNegocioDto
            {
                DatosNegocioId = dn.DatosNegocioId,
                Nombre = dn.Nombre,
                Telefono = dn.Telefono,
                Ruc = dn.Ruc,
                DireccionNegocio = dn.DireccionNegocio,
                LogoUrl = dn.LogoUrl,
                Email = dn.Email
            }).ToListAsync()
        };

        return await GuardarBackupAsync(backupData, descripcion, creadoPor);
    }

    public async Task<BackupRegistro> CrearBackupParcialAsync(string tipo, string? descripcion = null, string? creadoPor = null)
    {
        var backupData = new BackupDataDto
        {
            FechaBackup = DateTime.Now,
            TipoBackup = tipo
        };

        switch (tipo)
        {
            case "Clientes":
                backupData.Clientes = await _context.Clientes.Select(c => new ClienteDto
                {
                    ClienteId = c.ClienteId,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Telefono = c.Telefono,
                    Cedula = c.Cedula,
                    Direccion = c.Direccion,
                    FechaRegistro = c.FechaRegistro
                }).ToListAsync();
                break;
            case "Menus":
                backupData.CategoriasMenu = await _context.CategoriasMenu.Select(cm => new CategoriaMenuDto
                {
                    CategoriaId = cm.CategoriaId,
                    NombreCategoria = cm.NombreCategoria,
                    DescripcionCategoria = cm.DescripcionCategoria,
                    Estado = cm.Estado
                }).ToListAsync();
                backupData.ItemsMenu = await _context.ItemsMenu.Select(im => new ItemMenuDto
                {
                    ItemId = im.ItemId,
                    NombreItem = im.NombreItem,
                    Descripcion = im.Descripcion,
                    Precio = im.Precio,
                    Estado = im.Estado,
                    ImagenUrl = im.ImagenUrl,
                    CategoriaId = im.CategoriaId
                }).ToListAsync();
                break;
            case "Facturas":
                backupData.Facturas = await _context.Facturas.Select(f => new FacturaDto
                {
                    FacturaId = f.FacturaId,
                    ClienteId = f.ClienteId,
                    MesaId = f.MesaId,
                    MontoTotal = f.MontoTotal,
                    MetodoPago = f.MetodoPago,
                    FechaPago = f.FechaPago
                }).ToListAsync();
                backupData.DetalleFacturas = await _context.DetalleFacturas.Select(df => new DetalleFacturaDto
                {
                    DetalleFacturaId = df.DetalleFacturaId,
                    FacturaId = df.FacturaId,
                    ItemId = df.ItemId,
                    Cantidad = df.Cantidad,
                    PrecioUnitario = df.PrecioUnitario,
                    Subtotal = df.Subtotal
                }).ToListAsync();
                break;
            case "Pedidos":
                backupData.Pedidos = await _context.Pedidos.Select(p => new PedidoDto
                {
                    PedidoId = p.PedidoId,
                    MesaId = p.MesaId,
                    EmpleadoId = p.EmpleadoId,
                    FechaHora = p.FechaHora,
                    EstadoPedido = p.EstadoPedido
                }).ToListAsync();
                backupData.DetallesPedido = await _context.DetallesPedido.Select(dp => new DetallePedidoDto
                {
                    DetalleId = dp.DetalleId,
                    PedidoId = dp.PedidoId,
                    ItemId = dp.ItemId,
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = dp.PrecioUnitario
                }).ToListAsync();
                break;
            case "Gastos":
                backupData.Gastos = await _context.Gastos.Select(g => new GastoDto
                {
                    GastoId = g.GastoId,
                    Descripcion = g.Descripcion,
                    Monto = g.Monto,
                    FechaGasto = g.FechaGasto,
                    CategoriaGasto = g.CategoriaGasto
                }).ToListAsync();
                break;
            case "Ingredientes":
                backupData.Ingredientes = await _context.Ingredientes.Select(i => new IngredienteDto
                {
                    IngredienteId = i.IngredienteId,
                    NombreIngrediente = i.NombreIngrediente,
                    CantidadStock = i.CantidadStock,
                    UnidadMedida = i.UnidadMedida,
                    ProveedorId = i.ProveedorId
                }).ToListAsync();
                backupData.Proveedores = await _context.Proveedores.Select(p => new ProveedorDto
                {
                    ProveedorId = p.ProveedorId,
                    NombreProveedor = p.NombreProveedor,
                    Contacto = p.Contacto,
                    Telefono = p.Telefono,
                    Email = p.Email
                }).ToListAsync();
                break;
            default:
                throw new ArgumentException("Tipo de backup no válido");
        }

        return await GuardarBackupAsync(backupData, descripcion, creadoPor);
    }

    private async Task<BackupRegistro> GuardarBackupAsync(BackupDataDto data, string? descripcion, string? creadoPor)
    {
        var fileName = $"backup_{data.TipoBackup}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        var filePath = Path.Combine(_backupFolder, fileName);

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(filePath, json);

        var fileInfo = new FileInfo(filePath);

        var registro = new BackupRegistro
        {
            NombreArchivo = fileName,
            RutaArchivo = $"/backups/{fileName}",
            FechaCreacion = DateTime.Now,
            TamanoBytes = fileInfo.Length,
            TipoBackup = data.TipoBackup,
            Estado = "Completado",
            Descripcion = descripcion,
            CreadoPor = creadoPor
        };

        _context.BackupRegistros.Add(registro);
        await _context.SaveChangesAsync();

        return registro;
    }

    public async Task<List<BackupRegistro>> ObtenerTodosLosBackupsAsync()
    {
        return await _context.BackupRegistros
            .OrderByDescending(b => b.FechaCreacion)
            .ToListAsync();
    }

    public async Task<BackupRegistro?> ObtenerBackupPorIdAsync(int id)
    {
        return await _context.BackupRegistros.FindAsync(id);
    }

    public async Task<bool> RestaurarBackupAsync(int backupId)
    {
        var backup = await _context.BackupRegistros.FindAsync(backupId);
        if (backup == null) return false;

        var filePath = Path.Combine(_backupFolder, backup.NombreArchivo);
        if (!File.Exists(filePath)) return false;

        try
        {
            backup.Estado = "Restaurando";
            await _context.SaveChangesAsync();

            var json = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<BackupDataDto>(json, _jsonOptions);

            if (data == null) return false;

            await RestaurarDatosAsync(data);

            backup.Estado = "Completado";
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            backup.Estado = "Fallido";
            await _context.SaveChangesAsync();
            throw;
        }
    }

    private async Task RestaurarDatosAsync(BackupDataDto data)
    {
        // Usar una transacción para asegurar que todo se restaure o nada
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // IMPORTANTE: Eliminar en orden correcto para evitar violaciones de foreign keys
            // Primero eliminar las tablas que DEPENDEN de otras
            
            // 1. Detalles de Facturas (dependen de Facturas)
            if (data.DetalleFacturas != null && data.DetalleFacturas.Any())
            {
                _context.DetalleFacturas.RemoveRange(_context.DetalleFacturas);
                await _context.SaveChangesAsync();
            }

            // 2. Facturas (dependen de Mesas y Clientes)
            if (data.Facturas != null && data.Facturas.Any())
            {
                _context.Facturas.RemoveRange(_context.Facturas);
                await _context.SaveChangesAsync();
            }

            // 3. Detalles de Pedidos (dependen de Pedidos)
            if (data.DetallesPedido != null && data.DetallesPedido.Any())
            {
                _context.DetallesPedido.RemoveRange(_context.DetallesPedido);
                await _context.SaveChangesAsync();
            }

            // 4. Pedidos (dependen de Mesas y Empleados)
            if (data.Pedidos != null && data.Pedidos.Any())
            {
                _context.Pedidos.RemoveRange(_context.Pedidos);
                await _context.SaveChangesAsync();
            }

            // 5. Reservaciones (dependen de Clientes y Mesas)
            if (data.Reservaciones != null && data.Reservaciones.Any())
            {
                _context.Reservaciones.RemoveRange(_context.Reservaciones);
                await _context.SaveChangesAsync();
            }

            // 6. Ingredientes (dependen de Proveedores)
            if (data.Ingredientes != null && data.Ingredientes.Any())
            {
                _context.Ingredientes.RemoveRange(_context.Ingredientes);
                await _context.SaveChangesAsync();
            }

            // 7. Items del Menú (dependen de Categorías)
            if (data.ItemsMenu != null && data.ItemsMenu.Any())
            {
                _context.ItemsMenu.RemoveRange(_context.ItemsMenu);
                await _context.SaveChangesAsync();
            }

            // AHORA eliminar las tablas de las que dependían las anteriores
            
            // 8. Proveedores
            if (data.Proveedores != null && data.Proveedores.Any())
            {
                var proveedores = data.Proveedores.Select(d => new Proveedor
                {
                    ProveedorId = d.ProveedorId,
                    NombreProveedor = d.NombreProveedor ?? "",
                    Contacto = d.Contacto,
                    Telefono = d.Telefono,
                    Email = d.Email
                }).ToList();
                
                _context.Proveedores.RemoveRange(_context.Proveedores);
                await _context.SaveChangesAsync();
                _context.Proveedores.AddRange(proveedores);
                await _context.SaveChangesAsync();
            }

            // 9. Categorías de Menú
            if (data.CategoriasMenu != null && data.CategoriasMenu.Any())
            {
                var categorias = data.CategoriasMenu.Select(d => new CategoriaMenu
                {
                    CategoriaId = d.CategoriaId,
                    NombreCategoria = d.NombreCategoria ?? "",
                    DescripcionCategoria = d.DescripcionCategoria ?? "",
                    Estado = d.Estado ?? "ACTIVO"
                }).ToList();
                
                _context.CategoriasMenu.RemoveRange(_context.CategoriasMenu);
                await _context.SaveChangesAsync();
                _context.CategoriasMenu.AddRange(categorias);
                await _context.SaveChangesAsync();
            }

            // 10. Mesas (otras tablas dependían de ellas)
            if (data.Mesas != null && data.Mesas.Any())
            {
                var mesas = data.Mesas.Select(d => new Mesa
                {
                    MesaId = d.MesaId,
                    NumeroMesa = d.NumeroMesa,
                    Capacidad = d.Capacidad,
                    Estado = d.Estado ?? "Disponible"
                }).ToList();
                
                _context.Mesas.RemoveRange(_context.Mesas);
                await _context.SaveChangesAsync();
                _context.Mesas.AddRange(mesas);
                await _context.SaveChangesAsync();
            }

            // 11. Empleados
            if (data.Empleados != null && data.Empleados.Any())
            {
                var empleados = data.Empleados.Select(d => new Empleado
                {
                    EmpleadoId = d.EmpleadoId,
                    Nombre = d.Nombre ?? "",
                    Apellido = d.Apellido ?? "",
                    Puesto = d.Puesto ?? "",
                    FechaContratacion = d.FechaContratacion,
                    Salario = d.Salario,
                    Telefono = d.Telefono
                }).ToList();
                
                _context.Empleados.RemoveRange(_context.Empleados);
                await _context.SaveChangesAsync();
                _context.Empleados.AddRange(empleados);
                await _context.SaveChangesAsync();
            }

            // 12. Clientes
            if (data.Clientes != null && data.Clientes.Any())
            {
                var clientes = data.Clientes.Select(d => new Cliente
                {
                    ClienteId = d.ClienteId,
                    Nombre = d.Nombre ?? "",
                    Apellido = d.Apellido ?? "",
                    Telefono = d.Telefono,
                    Cedula = d.Cedula,
                    Direccion = d.Direccion,
                    FechaRegistro = d.FechaRegistro
                }).ToList();
                
                _context.Clientes.RemoveRange(_context.Clientes);
                await _context.SaveChangesAsync();
                _context.Clientes.AddRange(clientes);
                await _context.SaveChangesAsync();
            }

            // 13. Gastos (no depende de nada)
            if (data.Gastos != null && data.Gastos.Any())
            {
                var gastos = data.Gastos.Select(d => new Gasto
                {
                    GastoId = d.GastoId,
                    Descripcion = d.Descripcion ?? "",
                    Monto = d.Monto,
                    FechaGasto = d.FechaGasto,
                    CategoriaGasto = d.CategoriaGasto ?? "Otros"
                }).ToList();
                
                _context.Gastos.RemoveRange(_context.Gastos);
                await _context.SaveChangesAsync();
                _context.Gastos.AddRange(gastos);
                await _context.SaveChangesAsync();
            }

            // 14. Datos del negocio (generalmente solo 1 registro)
            if (data.DatosNegocios != null && data.DatosNegocios.Any())
            {
                var negocios = data.DatosNegocios.Select(d => new DatosNegocio
                {
                    DatosNegocioId = d.DatosNegocioId,
                    Nombre = d.Nombre ?? "",
                    Telefono = d.Telefono,
                    Ruc = d.Ruc,
                    DireccionNegocio = d.DireccionNegocio,
                    LogoUrl = d.LogoUrl,
                    Email = d.Email
                }).ToList();
                
                _context.DatosNegocios.RemoveRange(_context.DatosNegocios);
                await _context.SaveChangesAsync();
                _context.DatosNegocios.AddRange(negocios);
                await _context.SaveChangesAsync();
            }

            // AHORA agregar las entidades en orden inverso de dependencias
            
            // Agregar Pedidos (después de Mesas y Empleados)
            if (data.Pedidos != null && data.Pedidos.Any())
            {
                var pedidos = data.Pedidos.Select(d => new Pedido
                {
                    PedidoId = d.PedidoId,
                    MesaId = d.MesaId ?? 1,
                    EmpleadoId = d.EmpleadoId ?? 1,
                    FechaHora = d.FechaHora,
                    EstadoPedido = d.EstadoPedido ?? "En preparación"
                }).ToList();
                _context.Pedidos.AddRange(pedidos);
                await _context.SaveChangesAsync();
            }

            // Agregar Detalles de Pedidos
            if (data.DetallesPedido != null && data.DetallesPedido.Any())
            {
                var detalles = data.DetallesPedido.Select(d => new DetallePedido
                {
                    DetalleId = d.DetalleId,
                    PedidoId = d.PedidoId,
                    ItemId = d.ItemId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList();
                _context.DetallesPedido.AddRange(detalles);
                await _context.SaveChangesAsync();
            }

            // Agregar Facturas
            if (data.Facturas != null && data.Facturas.Any())
            {
                var facturas = data.Facturas.Select(d => new Factura
                {
                    FacturaId = d.FacturaId,
                    ClienteId = d.ClienteId,
                    MesaId = d.MesaId,
                    MontoTotal = d.MontoTotal,
                    MetodoPago = d.MetodoPago ?? "Efectivo",
                    FechaPago = d.FechaPago
                }).ToList();
                _context.Facturas.AddRange(facturas);
                await _context.SaveChangesAsync();
            }

            // Agregar Detalles de Facturas
            if (data.DetalleFacturas != null && data.DetalleFacturas.Any())
            {
                var detalles = data.DetalleFacturas.Select(d => new DetalleFactura
                {
                    DetalleFacturaId = d.DetalleFacturaId,
                    FacturaId = d.FacturaId,
                    ItemId = d.ItemId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList();
                _context.DetalleFacturas.AddRange(detalles);
                await _context.SaveChangesAsync();
            }

            // Agregar Ingredientes
            if (data.Ingredientes != null && data.Ingredientes.Any())
            {
                var ingredientes = data.Ingredientes.Select(d => new Ingrediente
                {
                    IngredienteId = d.IngredienteId,
                    NombreIngrediente = d.NombreIngrediente ?? "",
                    CantidadStock = d.CantidadStock,
                    UnidadMedida = d.UnidadMedida ?? "",
                    ProveedorId = d.ProveedorId
                }).ToList();
                _context.Ingredientes.AddRange(ingredientes);
                await _context.SaveChangesAsync();
            }

            // Agregar Reservaciones
            if (data.Reservaciones != null && data.Reservaciones.Any())
            {
                var reservaciones = data.Reservaciones.Select(d => new Reservacion
                {
                    ReservacionId = d.ReservacionId,
                    ClienteId = d.ClienteId > 0 ? d.ClienteId : 1,
                    MesaId = d.MesaId > 0 ? d.MesaId : 1,
                    FechaHora = d.FechaHora,
                    NumeroPersonas = d.NumeroPersonas,
                    Estado = d.Estado ?? "Pendiente"
                }).ToList();
                _context.Reservaciones.AddRange(reservaciones);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> EliminarBackupAsync(int id)
    {
        var backup = await _context.BackupRegistros.FindAsync(id);
        if (backup == null) return false;

        var filePath = Path.Combine(_backupFolder, backup.NombreArchivo);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        _context.BackupRegistros.Remove(backup);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<byte[]?> DescargarBackupAsync(int id)
    {
        var backup = await _context.BackupRegistros.FindAsync(id);
        if (backup == null) return null;

        var filePath = Path.Combine(_backupFolder, backup.NombreArchivo);
        if (!File.Exists(filePath)) return null;

        return await File.ReadAllBytesAsync(filePath);
    }
}

public class BackupDataDto
{
    public DateTime FechaBackup { get; set; }
    public string TipoBackup { get; set; } = string.Empty;

    public List<ClienteDto>? Clientes { get; set; }
    public List<EmpleadoDto>? Empleados { get; set; }
    public List<MesaDto>? Mesas { get; set; }
    public List<ReservacionDto>? Reservaciones { get; set; }
    public List<CategoriaMenuDto>? CategoriasMenu { get; set; }
    public List<ItemMenuDto>? ItemsMenu { get; set; }
    public List<PedidoDto>? Pedidos { get; set; }
    public List<DetallePedidoDto>? DetallesPedido { get; set; }
    public List<FacturaDto>? Facturas { get; set; }
    public List<DetalleFacturaDto>? DetalleFacturas { get; set; }
    public List<GastoDto>? Gastos { get; set; }
    public List<ProveedorDto>? Proveedores { get; set; }
    public List<IngredienteDto>? Ingredientes { get; set; }
    public List<DatosNegocioDto>? DatosNegocios { get; set; }
}

// DTOs Planos sin propiedades de navegación
public class ClienteDto { public int ClienteId { get; set; } public string Nombre { get; set; } = ""; public string Apellido { get; set; } = ""; public string? Telefono { get; set; } public string? Cedula { get; set; } public string? Direccion { get; set; } public DateTime FechaRegistro { get; set; } }
public class EmpleadoDto { public int EmpleadoId { get; set; } public string Nombre { get; set; } = ""; public string Apellido { get; set; } = ""; public string Puesto { get; set; } = ""; public DateTime FechaContratacion { get; set; } public decimal Salario { get; set; } public string? Telefono { get; set; } }
public class MesaDto { public int MesaId { get; set; } public int NumeroMesa { get; set; } public int Capacidad { get; set; } public string? Estado { get; set; } }
public class ReservacionDto { public int ReservacionId { get; set; } public int ClienteId { get; set; } public int MesaId { get; set; } public DateTime FechaHora { get; set; } public int NumeroPersonas { get; set; } public string? Estado { get; set; } }
public class CategoriaMenuDto { public int CategoriaId { get; set; } public string NombreCategoria { get; set; } = ""; public string DescripcionCategoria { get; set; } = ""; public string? Estado { get; set; } }
public class ItemMenuDto { public int ItemId { get; set; } public string NombreItem { get; set; } = ""; public string? Descripcion { get; set; } public decimal Precio { get; set; } public string? Estado { get; set; } public string? ImagenUrl { get; set; } public int CategoriaId { get; set; } }
public class PedidoDto { public int PedidoId { get; set; } public int? MesaId { get; set; } public int? EmpleadoId { get; set; } public DateTime FechaHora { get; set; } public string? EstadoPedido { get; set; } }
public class DetallePedidoDto { public int DetalleId { get; set; } public int PedidoId { get; set; } public int ItemId { get; set; } public int Cantidad { get; set; } public decimal PrecioUnitario { get; set; } }
public class FacturaDto { public int FacturaId { get; set; } public int? ClienteId { get; set; } public int? MesaId { get; set; } public decimal MontoTotal { get; set; } public string MetodoPago { get; set; } = ""; public DateTime FechaPago { get; set; } }
public class DetalleFacturaDto { public int DetalleFacturaId { get; set; } public int FacturaId { get; set; } public int ItemId { get; set; } public int Cantidad { get; set; } public decimal PrecioUnitario { get; set; } public decimal Subtotal { get; set; } }
public class GastoDto { public int GastoId { get; set; } public string Descripcion { get; set; } = ""; public decimal Monto { get; set; } public DateTime? FechaGasto { get; set; } public string CategoriaGasto { get; set; } = ""; }
public class ProveedorDto { public int ProveedorId { get; set; } public string NombreProveedor { get; set; } = ""; public string? Contacto { get; set; } public string? Telefono { get; set; } public string? Email { get; set; } }
public class IngredienteDto { public int IngredienteId { get; set; } public string NombreIngrediente { get; set; } = ""; public decimal CantidadStock { get; set; } public string UnidadMedida { get; set; } = ""; public int? ProveedorId { get; set; } }
public class DatosNegocioDto { public int DatosNegocioId { get; set; } public string Nombre { get; set; } = ""; public string? Telefono { get; set; } public string? Ruc { get; set; } public string? DireccionNegocio { get; set; } public string? LogoUrl { get; set; } public string? Email { get; set; } }