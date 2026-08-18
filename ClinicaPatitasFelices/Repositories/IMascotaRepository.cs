using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public interface IMascotaRepository
{
    // CREATE
    void RegistrarMascota(Mascota mascotaNueva);

    // READ
    List<Mascota> ListMascotas();
    Mascota? BuscarPorId(Guid id);
    Mascota? BuscarPorNombre(string nombre);
    List<Mascota> BuscarPorRaza(string raza);
    List<Mascota> BuscarPorEspecie(Especie especie);
    List<Mascota> BuscarPorDueno(Guid clienteId);
    List<Mascota> BuscarPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses);

    // UPDATE
    bool ActualizarMascota(Guid id, string nombre, Especie especie, string raza, DateOnly fechaDeNacimiento, Sexo sexo);

    // DELETE
    bool EliminarMascota(Guid id);

    // VALIDACIONES / UTILIDADES
    bool ExisteId(Guid id);
    bool ExisteNombre(string nombre);
    int ContarMascotas();
}
