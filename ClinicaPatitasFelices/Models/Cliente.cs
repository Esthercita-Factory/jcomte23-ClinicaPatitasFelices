namespace ClinicaPatitasFelices.Models;

/// <summary>
/// Dueño de una o varias mascotas. La relacion Cliente 1 -- N Mascota se
/// administra unicamente desde aquí para que las dos puntas nunca queden
/// desincronizadas.
/// </summary>
public class Cliente
{
    private readonly List<Mascota> _mascotas = [];

    public Guid Id { get; }
    public string Documento { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Telefono { get; private set; }
    public string? Email { get; private set; }
    public string? Direccion { get; private set; }
    public DateOnly FechaDeRegistro { get; }

    public string NombreCompleto => $"{Nombre} {Apellido}";

    /// <summary>
    /// Solo lectura: agregar o quitar mascotas se hace con <see cref="AgregarMascota"/>
    /// y <see cref="QuitarMascota"/>, que son los que mantienen la relacion consistente.
    /// </summary>
    public IReadOnlyList<Mascota> Mascotas => _mascotas;

    public int CantidadDeMascotas => _mascotas.Count;

    public Cliente(
        string documento,
        string nombre,
        string apellido,
        string telefono,
        string? email = null,
        string? direccion = null)
    {
        Id = Guid.NewGuid();
        Documento = ValidarTexto(documento, nameof(documento));
        Nombre = ValidarTexto(nombre, nameof(nombre));
        Apellido = ValidarTexto(apellido, nameof(apellido));
        Telefono = ValidarTexto(telefono, nameof(telefono));
        Email = ValidarOpcional(email);
        Direccion = ValidarOpcional(direccion);
        FechaDeRegistro = DateOnly.FromDateTime(DateTime.Today);
    }

    public void ActualizarDatosDeContacto(string telefono, string? email, string? direccion)
    {
        Telefono = ValidarTexto(telefono, nameof(telefono));
        Email = ValidarOpcional(email);
        Direccion = ValidarOpcional(direccion);
    }

    /// <summary>
    /// Registra una mascota a nombre de este cliente. Si la mascota ya tenia otro
    /// dueño, se transfiere.
    /// </summary>
    /// <returns> False si la mascota ya estaba registrada con este mismo cliente.</returns>
    public bool AgregarMascota(Mascota mascota)
    {
        ArgumentNullException.ThrowIfNull(mascota);

        if (_mascotas.Any(registrada => registrada.Id == mascota.Id))
        {
            return false;
        }

        mascota.Dueno?.QuitarMascota(mascota);

        _mascotas.Add(mascota);
        mascota.AsignarDueno(this);

        return true;
    }

    public bool QuitarMascota(Mascota mascota)
    {
        ArgumentNullException.ThrowIfNull(mascota);

        if (!_mascotas.Remove(mascota))
        {
            return false;
        }

        mascota.AsignarDueno(null);

        return true;
    }

    public bool TieneMascota(Guid mascotaId)
    {
        return _mascotas.Any(mascota => mascota.Id == mascotaId);
    }

    public override string ToString()
    {
        return $"{NombreCompleto} (doc. {Documento}) - {CantidadDeMascotas} mascota(s)";
    }

    private static string ValidarTexto(string valor, string nombreDelParametro)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor, nombreDelParametro);

        return valor.Trim();
    }

    private static string? ValidarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
