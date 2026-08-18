namespace ClinicaPatitasFelices.Models;

/// <summary>
/// Dueño de una o varias mascotas. Es un contenedor de datos: representa la fila
/// de la tabla Cliente. Las reglas de negocio y la coherencia de la relacion con
/// las mascotas viven en ClienteService.
/// </summary>
public class Cliente
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    
    public DateOnly FechaDeRegistro { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    /// <summary>
    /// Contraparte de <see cref="Mascota.Dueno"/>. Con Entity Framework esta lista
    /// se mapea a la clave foranea ClienteId de la tabla Mascota.
    /// </summary>
    public List<Mascota> Mascotas { get; set; } = [];

    /// <summary>Derivado de Nombre y Apellido; no se persiste.</summary>
    public string NombreCompleto => $"{Nombre} {Apellido}";

    /// <summary>Lo usan Entity Framework y los inicializadores de objeto.</summary>
    public Cliente()
    {
    }

    /// <summary>
    /// Atajo para armar un cliente completo. Solo asigna: la validacion y la
    /// normalizacion de los datos son responsabilidad de ClienteService.
    /// </summary>
    public Cliente(
        string documento,
        string nombre,
        string apellido,
        string telefono,
        string? email = null,
        string? direccion = null)
    {
        Documento = documento;
        Nombre = nombre;
        Apellido = apellido;
        Telefono = telefono;
        Email = email;
        Direccion = direccion;
    }
}
