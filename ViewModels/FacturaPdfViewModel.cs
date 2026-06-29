using RestauranteApp.Models;

namespace RestauranteApp.ViewModels;

public class FacturaPdfViewModel
{
    public Factura Factura { get; set; } = null!;
    public DatosNegocio DatosNegocio { get; set; } = null!;
}
