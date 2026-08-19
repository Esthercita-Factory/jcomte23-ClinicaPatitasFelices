using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Services;

public interface IMascotaService
{
    /// <exception cref="ArgumentException">Si algun dato de la mascota no es valido.</exception>
    Mascota CrearMascota(string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo);

    List<Mascota> ConsultarMascotas();
    Mascota? ConsultarMascota(Guid id);
    Mascota? BuscarMascotaPorNombre(string nombre);
    List<Mascota> BuscarMascotasPorRaza(string raza);
    List<Mascota> BuscarMascotasPorEspecie(Especie especie);
    List<Mascota> BuscarCachorros();

    /// <exception cref="ArgumentOutOfRangeException">Si el rango de edades no es valido.</exception>
    List<Mascota> BuscarMascotasPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses);

    bool ActualizarMascota(Guid id, string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo);

    /// <returns>false si no existe la mascota.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si el peso no es mayor que cero.</exception>
    bool AnotarPeso(Guid id, decimal pesoEnKg);

    /// <returns>false si no existe la mascota o si ya figuraba como esterilizada.</returns>
    bool MarcarComoEsterilizada(Guid id);

    bool RetirarMascota(Guid id);

    int ContarMascotas();
}
