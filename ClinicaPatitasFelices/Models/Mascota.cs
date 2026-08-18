namespace ClinicaPatitasFelices.Models;

/// <summary>
/// Un animal atendido por la clinica. La especie es un dato (<see cref="Models.Especie"/>),
/// no un subtipo: perros, gatos y conejos comparten exactamente el mismo comportamiento
/// dentro del sistema.
/// </summary>
public class Mascota
{
    public Guid Id { get; }
    public string Nombre { get; private set; }
    public Especie Especie { get; private set; }
    public string Raza { get; private set; }
    public DateOnly FechaDeNacimiento { get; private set; }
    public Sexo Sexo { get; private set; }
    public decimal? PesoEnKg { get; private set; }
    public bool EstaEsterilizada { get; private set; }

    /// <summary>Dueño de la mascota. Es null mientras no se le asigne un cliente.</summary>
    public Cliente? Dueno { get; private set; }

    /// <summary>
    /// Se calcula contra la fecha actual, nunca se almacena: una edad guardada
    /// queda desactualizada al día siguiente.
    /// </summary>
    public int EdadEnMeses => CalcularEdadEnMeses(FechaDeNacimiento, DateOnly.FromDateTime(DateTime.Today));

    public string EdadDescriptiva
    {
        get
        {
            var (anios, meses) = (EdadEnMeses / 12, EdadEnMeses % 12);

            if (anios == 0)
            {
                return $"{meses} {(meses == 1 ? "mes" : "meses")}";
            }

            var textoAnios = $"{anios} {(anios == 1 ? "año" : "años")}";

            return meses == 0 ? textoAnios : $"{textoAnios} y {meses} {(meses == 1 ? "mes" : "meses")}";
        }
    }

    public Mascota(
        string nombre,
        Especie especie,
        string raza,
        DateOnly fechaDeNacimiento,
        Sexo sexo = Sexo.Desconocido,
        decimal? pesoEnKg = null,
        bool estaEsterilizada = false)
    {
        Id = Guid.NewGuid();
        Nombre = ValidarNombre(nombre);
        Especie = especie;
        Raza = ValidarRaza(raza);
        FechaDeNacimiento = ValidarFechaDeNacimiento(fechaDeNacimiento);
        Sexo = sexo;
        PesoEnKg = ValidarPeso(pesoEnKg);
        EstaEsterilizada = estaEsterilizada;
    }

    public void ActualizarDatos(string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo)
    {
        Nombre = ValidarNombre(nombre);
        Especie = especie;
        Raza = ValidarRaza(raza);
        FechaDeNacimiento = ValidarFechaDeNacimiento(fechaDeNacimiento);
        Sexo = sexo;
    }

    public void RegistrarPeso(decimal pesoEnKg)
    {
        PesoEnKg = ValidarPeso(pesoEnKg);
    }

    public void MarcarComoEsterilizada()
    {
        EstaEsterilizada = true;
    }

    /// <summary>
    /// Solo lo llama <see cref="Cliente"/> para mantener las dos puntas de la relacion
    /// sincronizadas. No es parte de la API publica del modelo.
    /// </summary>
    internal void AsignarDueno(Cliente? dueno)
    {
        Dueno = dueno;
    }

    public override string ToString()
    {
        return $"{Nombre} ({Especie}, {Raza}) - {EdadDescriptiva}";
    }

    private static int CalcularEdadEnMeses(DateOnly nacimiento, DateOnly hoy)
    {
        var meses = ((hoy.Year - nacimiento.Year) * 12) + hoy.Month - nacimiento.Month;

        if (hoy.Day < nacimiento.Day)
        {
            meses--;
        }

        return Math.Max(0, meses);
    }

    private static string ValidarNombre(string nombre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        // Se conserva el texto tal como lo escribio el usuario; las comparaciones
        // se hacen sin distinguir mayusculas en el repositorio.
        return nombre.Trim();
    }

    private static string ValidarRaza(string raza)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(raza);

        return raza.Trim();
    }

    private static DateOnly ValidarFechaDeNacimiento(DateOnly fechaDeNacimiento)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);

        if (fechaDeNacimiento > hoy)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fechaDeNacimiento),
                fechaDeNacimiento,
                "La fecha de nacimiento no puede estar en el futuro.");
        }

        return fechaDeNacimiento;
    }

    private static decimal? ValidarPeso(decimal? pesoEnKg)
    {
        if (pesoEnKg is null)
        {
            return null;
        }

        if (pesoEnKg <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pesoEnKg), pesoEnKg, "El peso debe ser mayor que cero.");
        }

        return pesoEnKg;
    }
}
