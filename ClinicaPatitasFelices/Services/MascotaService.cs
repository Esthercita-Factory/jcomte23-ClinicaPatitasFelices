using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.Services;

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

    /// <summary>
    /// Las reglas de integridad viven en el modelo: si algun dato es invalido, el
    /// constructor de <see cref="Mascota"/> lanza y la excepcion sube tal cual.
    /// </summary>
    public Mascota CrearMascota(string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo)
    {
        var mascotaNueva = new Mascota(nombre, especie, raza, fechaDeNacimiento, sexo);

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
        return _mascotaRepository.Actualizar(id, nombre, especie, raza, fechaDeNacimiento, sexo);
    }

    public bool AnotarPeso(Guid id, decimal pesoEnKg)
    {
        var mascota = _mascotaRepository.ObtenerPorId(id);

        if (mascota is null)
        {
            return false;
        }

        mascota.RegistrarPeso(pesoEnKg);

        return true;
    }

    public bool MarcarComoEsterilizada(Guid id)
    {
        var mascota = _mascotaRepository.ObtenerPorId(id);

        if (mascota is null || mascota.EstaEsterilizada)
        {
            return false;
        }

        mascota.MarcarComoEsterilizada();

        return true;
    }

    public bool RetirarMascota(Guid id)
    {
        return _mascotaRepository.Eliminar(id);
    }

    public int ContarMascotas()
    {
        return _mascotaRepository.Contar();
    }
}
