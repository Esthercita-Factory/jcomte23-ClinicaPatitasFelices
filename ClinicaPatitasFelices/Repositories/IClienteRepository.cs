using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public interface IClienteRepository
{
    // CREATE
    bool RegistrarCliente(Cliente clienteNuevo);

    // READ
    List<Cliente> ListClientes();
    Cliente? BuscarPorId(Guid id);
    Cliente? BuscarPorDocumento(string documento);
    List<Cliente> BuscarPorNombre(string textoDeBusqueda);
    Cliente? BuscarDuenoDeMascota(Guid mascotaId);

    // UPDATE
    bool ActualizarCliente(Guid id, string nombre, string apellido, string telefono, string? email, string? direccion);

    // DELETE
    bool EliminarCliente(Guid id);

    // RELACION CON MASCOTAS
    bool AsignarMascota(Guid clienteId, Guid mascotaId);
    bool DesasignarMascota(Guid clienteId, Guid mascotaId);
    List<Mascota> ListarMascotasDe(Guid clienteId);

    // VALIDACIONES / UTILIDADES
    bool ExisteId(Guid id);
    bool ExisteDocumento(string documento);
    int ContarClientes();
}
