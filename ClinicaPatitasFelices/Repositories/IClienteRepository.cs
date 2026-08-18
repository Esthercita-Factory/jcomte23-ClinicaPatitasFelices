using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public interface IClienteRepository
{
    // CREATE
    bool Registrar(Cliente clienteNuevo);

    // READ
    List<Cliente> ObtenerTodos();
    Cliente? ObtenerPorId(Guid id);
    Cliente? ObtenerPorDocumento(string documento);
    List<Cliente> FiltrarPorNombre(string textoDeBusqueda);
    Cliente? ObtenerDuenoDeMascota(Guid mascotaId);

    // UPDATE
    bool Actualizar(Guid id, string nombre, string apellido, string telefono, string? email, string? direccion);

    // DELETE
    bool Eliminar(Guid id);

    // CONSULTA DE LA RELACION
    // Asignar y desasignar mascotas es coordinacion entre dos entidades: vive en ClienteService.
    List<Mascota> ObtenerMascotasDe(Guid clienteId);

    // VALIDACIONES / UTILIDADES
    bool ExisteId(Guid id);
    bool ExisteDocumento(string documento);
    int Contar();
}
