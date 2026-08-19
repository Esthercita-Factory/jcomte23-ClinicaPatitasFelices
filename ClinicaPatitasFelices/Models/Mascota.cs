namespace ClinicaPatitasFelices.Models;

public class Mascota
{
    public Guid Id { get; }
    public string Nombre { get; set; }
    public Especie Especie { get; set; }
    public string Raza { get; set; }
    public DateOnly FechaDeNacimiento { get; set; }
    public Sexo Sexo { get; set; }

    /// <summary>Se registra en la consulta, no al dar de alta la mascota.</summary>
    public decimal? PesoEnKg { get; set; }

    public bool EstaEsterilizada { get; set; }

    /// <summary>
    /// Contraparte de <see cref="Cliente.Mascotas"/>. Es null mientras la mascota
    /// no tenga dueño asignado. Con Entity Framework se mapea a la clave foranea
    /// ClienteId de esta misma tabla.
    /// </summary>
    public Cliente? Dueno { get; set; }

    public Mascota(string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Especie = especie;
        Raza = raza;
        FechaDeNacimiento = fechaDeNacimiento;
        Sexo = sexo;
    }

    /// <summary>
    /// La edad se calcula contra la fecha actual, nunca se almacena: una edad
    /// guardada quedaria desactualizada al dia siguiente.
    /// </summary>
    public int CalcularEdadEnMeses()
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var meses = ((hoy.Year - FechaDeNacimiento.Year) * 12) + hoy.Month - FechaDeNacimiento.Month;

        if (hoy.Day < FechaDeNacimiento.Day)
        {
            meses--;
        }

        return Math.Max(0, meses);
    }

    /// <summary>Devuelve la edad en texto, por ejemplo "1 año y 2 meses".</summary>
    public string DescribirEdad()
    {
        var edadEnMeses = CalcularEdadEnMeses();
        var (anios, meses) = (edadEnMeses / 12, edadEnMeses % 12);

        if (anios == 0)
        {
            return $"{meses} {(meses == 1 ? "mes" : "meses")}";
        }

        var textoAnios = $"{anios} {(anios == 1 ? "año" : "años")}";

        return meses == 0 ? textoAnios : $"{textoAnios} y {meses} {(meses == 1 ? "mes" : "meses")}";
    }
}
