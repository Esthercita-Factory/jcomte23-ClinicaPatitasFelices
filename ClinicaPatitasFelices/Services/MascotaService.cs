using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.Services;

/// <summary>
/// Reglas de negocio de las mascotas. Los modelos son contenedores de datos, asi
/// que la validacion y la normalizacion se hacen aqui, antes de tocar el repositorio.
/// </summary>
public class MascotaService : IMascotaService
{
    /// <summary>Hasta esta edad se considera cachorro para efectos de la clinica.</summary>
    private const int EdadMaximaDeCachorroEnMeses = 12;

    private readonly IMascotaRepository _mascotaRepository;

    public MascotaService(IMascotaRepository mascotaRepository)
    {
        ArgumentNullException.ThrowIfNull(mascotaRepository);

        _mascotaRepository = mascotaRepository;
    }

    public Mascota CrearMascota(string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo)
    {
        var mascotaNueva = new Mascota(
            ValidarTexto(nombre, nameof(nombre)),
            especie,
            ValidarTexto(raza, nameof(raza)),
            ValidarFechaDeNacimiento(fechaDeNacimiento),
            sexo);

        _mascotaRepository.Registrar(mascotaNueva);

        return mascotaNueva;
    }

    public List<Mascota> ConsultarMascotas()
    {
        return _mascotaRepository.ObtenerTodas();
    }

    public Mascota? ConsultarMascota(Guid id)
    {
        return _mascotaRepository.ObtenerPorId(id);
    }

    public Mascota? BuscarMascotaPorNombre(string nombre)
    {
        return string.IsNullOrWhiteSpace(nombre) ? null : _mascotaRepository.ObtenerPorNombre(nombre);
    }

    public List<Mascota> BuscarMascotasPorRaza(string raza)
    {
        return string.IsNullOrWhiteSpace(raza) ? [] : _mascotaRepository.FiltrarPorRaza(raza);
    }

    public List<Mascota> BuscarMascotasPorEspecie(Especie especie)
    {
        return _mascotaRepository.FiltrarPorEspecie(especie);
    }

    public List<Mascota> BuscarCachorros()
    {
        return _mascotaRepository.FiltrarPorRangoDeEdad(0, EdadMaximaDeCachorroEnMeses);
    }

    public List<Mascota> BuscarMascotasPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(edadMinimaEnMeses);
        ArgumentOutOfRangeException.ThrowIfNegative(edadMaximaEnMeses);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(edadMinimaEnMeses, edadMaximaEnMeses);

        return _mascotaRepository.FiltrarPorRangoDeEdad(edadMinimaEnMeses, edadMaximaEnMeses);
    }

    public bool ActualizarMascota(
        Guid id,
        string nombre,
        Especie especie,
        string raza,
        DateOnly fechaDeNacimiento,
        Sexo sexo)
    {
        var mascota = _mascotaRepository.ObtenerPorId(id);

        if (mascota is null)
        {
            return false;
        }

        mascota.Nombre = ValidarTexto(nombre, nameof(nombre));
        mascota.Especie = especie;
        mascota.Raza = ValidarTexto(raza, nameof(raza));
        mascota.FechaDeNacimiento = ValidarFechaDeNacimiento(fechaDeNacimiento);
        mascota.Sexo = sexo;

        return _mascotaRepository.Actualizar(mascota);
    }

    public bool AnotarPeso(Guid id, decimal pesoEnKg)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pesoEnKg);

        var mascota = _mascotaRepository.ObtenerPorId(id);

        if (mascota is null)
        {
            return false;
        }

        mascota.PesoEnKg = pesoEnKg;

        return _mascotaRepository.Actualizar(mascota);
    }

    public bool MarcarComoEsterilizada(Guid id)
    {
        var mascota = _mascotaRepository.ObtenerPorId(id);

        if (mascota is null || mascota.EstaEsterilizada)
        {
            return false;
        }

        mascota.EstaEsterilizada = true;

        return _mascotaRepository.Actualizar(mascota);
    }

    /// <summary>
    /// Retira la mascota y la desvincula de su dueño, para no dejar al cliente
    /// apuntando a una mascota que ya no existe.
    /// </summary>
    public bool RetirarMascota(Guid id)
    {
        var mascota = _mascotaRepository.ObtenerPorId(id);

        if (mascota is null)
        {
            return false;
        }

        mascota.Dueno?.Mascotas.Remove(mascota);
        mascota.Dueno = null;

        return _mascotaRepository.Eliminar(id);
    }

    public int ContarMascotas()
    {
        return _mascotaRepository.Contar();
    }

    private static string ValidarTexto(string valor, string nombreDelParametro)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor, nombreDelParametro);

        return valor.Trim();
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
}
