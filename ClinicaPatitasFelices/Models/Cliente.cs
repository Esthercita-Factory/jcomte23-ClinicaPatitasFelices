namespace ClinicaPatitasFelices.Models;

public class Cliente
{
    public Guid Id { get; }
    public string Nombre { get; set; } 
    public string Apellido { get; set; } 
    /// <summary>Derivado de Nombre y Apellido; no se persiste.</summary>
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string Documento { get; set; } 
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string Direccion { get; set; }
    public DateOnly FechaDeRegistro { get; }

    /// <summary>
    /// Contraparte de <see cref="Mascota.Dueno"/>. Con Entity Framework esta lista
    /// se mapea a la clave foranea ClienteId de la tabla Mascota.
    /// </summary>
    public List<Mascota> Mascotas { get; } = [];
    
    public Cliente(
        string documento,
        string nombre,
        string apellido,
        string telefono,
        string email,
        string direccion)
    {
        Id = Guid.NewGuid();
        Documento = documento;
        Nombre = nombre;
        Apellido = apellido;
        Telefono = telefono;
        Email = email;
        Direccion = direccion;
        FechaDeRegistro = DateOnly.FromDateTime(DateTime.Today);
    }
}