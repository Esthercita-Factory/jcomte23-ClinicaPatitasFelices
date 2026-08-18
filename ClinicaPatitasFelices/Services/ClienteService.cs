using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.Services;

/// <summary>
/// Coordina clientes y mascotas. Las operaciones que tocan las dos entidades viven
/// aqui y no en los repositorios: cada repositorio se ocupa de una sola entidad.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMascotaRepository _mascotaRepository;

    public ClienteService(IClienteRepository clienteRepository, IMascotaRepository mascotaRepository)
    {
        ArgumentNullException.ThrowIfNull(clienteRepository);
        ArgumentNullException.ThrowIfNull(mascotaRepository);

        _clienteRepository = clienteRepository;
        _mascotaRepository = mascotaRepository;
    }

    public Cliente? CrearCliente(
        string documento,
        string nombre,
        string apellido,
        string telefono,
        string? email,
        string? direccion)
    {
        // Si algun dato es invalido, el constructor de Cliente lanza y la excepcion sube.
        var clienteNuevo = new Cliente(documento, nombre, apellido, telefono, email, direccion);

        // El documento repetido si es un caso esperable del negocio, no un error.
        if (_clienteRepository.ExisteDocumento(clienteNuevo.Documento))
        {
            return null;
        }

        _clienteRepository.Registrar(clienteNuevo);

        return clienteNuevo;
    }

    public List<Cliente> ConsultarClientes()
    {
        return _clienteRepository.ObtenerTodos();
    }

    public Cliente? ConsultarCliente(Guid id)
    {
        return _clienteRepository.ObtenerPorId(id);
    }

    public Cliente? ConsultarClientePorDocumento(string documento)
    {
        return string.IsNullOrWhiteSpace(documento) ? null : _clienteRepository.ObtenerPorDocumento(documento);
    }

    public List<Cliente> BuscarClientesPorNombre(string textoDeBusqueda)
    {
        return _clienteRepository.FiltrarPorNombre(textoDeBusqueda);
    }

    public bool ActualizarCliente(
        Guid id,
        string nombre,
        string apellido,
        string telefono,
        string? email,
        string? direccion)
    {
        return _clienteRepository.Actualizar(id, nombre, apellido, telefono, email, direccion);
    }

    public bool RetirarCliente(Guid id)
    {
        return _clienteRepository.Eliminar(id);
    }

    // Coordinacion entre clientes y mascotas
    public bool AsignarMascota(Guid clienteId, Guid mascotaId)
    {
        var cliente = _clienteRepository.ObtenerPorId(clienteId);
        var mascota = _mascotaRepository.ObtenerPorId(mascotaId);

        if (cliente is null || mascota is null)
        {
            return false;
        }

        return cliente.AgregarMascota(mascota);
    }

    public bool DesasignarMascota(Guid clienteId, Guid mascotaId)
    {
        var cliente = _clienteRepository.ObtenerPorId(clienteId);
        var mascota = _mascotaRepository.ObtenerPorId(mascotaId);

        if (cliente is null || mascota is null)
        {
            return false;
        }

        return cliente.QuitarMascota(mascota);
    }

    public bool CrearMascotaParaCliente(Guid clienteId, Mascota mascotaNueva)
    {
        ArgumentNullException.ThrowIfNull(mascotaNueva);

        var cliente = _clienteRepository.ObtenerPorId(clienteId);

        if (cliente is null)
        {
            return false;
        }

        _mascotaRepository.Registrar(mascotaNueva);

        return cliente.AgregarMascota(mascotaNueva);
    }

    public List<Mascota> ConsultarMascotasDe(Guid clienteId)
    {
        return _clienteRepository.ObtenerMascotasDe(clienteId);
    }

    public Cliente? ConsultarDuenoDe(Guid mascotaId)
    {
        return _clienteRepository.ObtenerDuenoDeMascota(mascotaId);
    }

    public int ContarClientes()
    {
        return _clienteRepository.Contar();
    }
}
