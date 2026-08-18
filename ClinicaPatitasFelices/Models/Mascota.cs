namespace ClinicaPatitasFelices.Models;

/// <summary>
/// Un animal atendido por la clinica. Es un contenedor de datos: representa la fila
/// de la tabla Mascota. La especie es un dato (<see cref="Models.Especie"/>), no un
/// subtipo. Las reglas de negocio viven en MascotaService.
/// </summary>
public class Mascota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public Especie Especie { get; set; }
    public string Raza { get; set; } = string.Empty;
    public DateOnly FechaDeNacimiento { get; set; }
    public Sexo Sexo { get; set; }
    public decimal? PesoEnKg { get; set; }
    public bool EstaEsterilizada { get; set; }

    /// <summary>
    /// Contraparte de <see cref="Cliente.Mascotas"/>. Es null mientras la mascota
    /// no tenga dueño asignado. Con Entity Framework se mapea a la clave foranea
    /// ClienteId de esta misma tabla.
    /// </summary>
    public Cliente? Dueno { get; set; }

    /// <summary>
    /// Derivado de la fecha de nacimiento contra la fecha actual; no se persiste.
    /// Una edad almacenada quedaria desactualizada al dia siguiente.
    /// </summary>
    public int EdadEnMeses
    {
        get
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var meses = ((hoy.Year - FechaDeNacimiento.Year) * 12) + hoy.Month - FechaDeNacimiento.Month;

            if (hoy.Day < FechaDeNacimiento.Day)
            {
                meses--;
            }

            return Math.Max(0, meses);
        }
    }

    /// <summary>Derivado de <see cref="EdadEnMeses"/>; no se persiste.</summary>
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
}
