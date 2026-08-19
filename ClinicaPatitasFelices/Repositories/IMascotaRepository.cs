using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public interface IMascotaRepository
{
    // CREATE
    void Registrar(Mascota mascotaNueva);

    // READ
    List<Mascota> ObtenerTodas();
    Mascota? ObtenerPorId(Guid id);
    Mascota? ObtenerPorNombre(string nombre);
    List<Mascota> FiltrarPorRaza(string raza);
    List<Mascota> FiltrarPorEspecie(Especie especie);
    List<Mascota> FiltrarPorDueno(Guid clienteId);
    List<Mascota> FiltrarPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses);

    // UPDATE
    bool Actualizar(Mascota mascota);

    // DELETE
    bool Eliminar(Guid id);

    // VALIDACIONES / UTILIDADES
    bool ExisteId(Guid id);
    bool ExisteNombre(string nombre);
    int Contar();
}
