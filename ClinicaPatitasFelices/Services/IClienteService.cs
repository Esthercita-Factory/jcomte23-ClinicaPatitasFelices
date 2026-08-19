using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Services;

public interface IClienteService
{
    /// <returns>El cliente creado, o null si ya existe otro con el mismo documento.</returns>
    /// <exception cref="ArgumentException">Si algun dato del cliente no es valido.</exception>
    Cliente? CrearCliente(string documento, string nombre, string apellido, string telefono, string? email, string? direccion);

    List<Cliente> ConsultarClientes();
    Cliente? ConsultarCliente(Guid id);
    Cliente? ConsultarClientePorDocumento(string documento);
    List<Cliente> BuscarClientesPorNombre(string textoDeBusqueda);

    bool ActualizarCliente(Guid id, string nombre, string apellido, string telefono, string? email, string? direccion);

    /// <summary>
    /// Retira al cliente. Sus mascotas siguen registradas en la clinica, pero
    /// quedan sin dueño asignado.
    /// </summary>
    bool RetirarCliente(Guid id);

    // Coordinacion entre clientes y mascotas
    /// <returns>false si no existe el cliente o la mascota, o si ya estaban vinculados.</returns>
    bool AsignarMascota(Guid clienteId, Guid mascotaId);

    bool DesasignarMascota(Guid clienteId, Guid mascotaId);

    /// <summary>Registra una mascota nueva y la deja vinculada al cliente en un solo paso.</summary>
    /// <returns>false si no existe el cliente.</returns>
    bool CrearMascotaParaCliente(Guid clienteId, Mascota mascotaNueva);

    List<Mascota> ConsultarMascotasDe(Guid clienteId);
    Cliente? ConsultarDuenoDe(Guid mascotaId);

    int ContarClientes();
}
