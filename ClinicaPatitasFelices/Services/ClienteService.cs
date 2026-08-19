using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.Services;

/// <summary>
/// Reglas de negocio de los clientes y coordinacion con las mascotas. Como los
/// modelos son contenedores de datos, este servicio es el unico responsable de que
/// Cliente.Mascotas y Mascota.Dueno se mantengan sincronizados.
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
        var clienteNuevo = new Cliente(
            ValidarTexto(documento, nameof(documento)),
            ValidarTexto(nombre, nameof(nombre)),
            ValidarTexto(apellido, nameof(apellido)),
            ValidarTexto(telefono, nameof(telefono)),
            NormalizarOpcional(email),
            NormalizarOpcional(direccion));

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
        var cliente = _clienteRepository.ObtenerPorId(id);

        if (cliente is null)
        {
            return false;
        }

        cliente.Nombre = ValidarTexto(nombre, nameof(nombre));
        cliente.Apellido = ValidarTexto(apellido, nameof(apellido));
        cliente.Telefono = ValidarTexto(telefono, nameof(telefono));
        cliente.Email = NormalizarOpcional(email);
        cliente.Direccion = NormalizarOpcional(direccion);

        return _clienteRepository.Actualizar(cliente);
    }

    /// <summary>
    /// Retira al cliente. Sus mascotas siguen registradas en la clinica, pero
    /// quedan sin dueño asignado.
    /// </summary>
    public bool RetirarCliente(Guid id)
    {
        var cliente = _clienteRepository.ObtenerPorId(id);

        if (cliente is null)
        {
            return false;
        }

        foreach (var mascota in cliente.Mascotas.ToList())
        {
            mascota.Dueno = null;
        }

        cliente.Mascotas.Clear();

        return _clienteRepository.Eliminar(id);
    }

    // Coordinacion entre clientes y mascotas
    /// <summary>
    /// Vincula una mascota ya registrada con un cliente, actualizando las dos puntas
    /// de la relacion. Si la mascota tenia otro dueño, se transfiere.
    /// </summary>
    public bool AsignarMascota(Guid clienteId, Guid mascotaId)
    {
        var cliente = _clienteRepository.ObtenerPorId(clienteId);
        var mascota = _mascotaRepository.ObtenerPorId(mascotaId);

        if (cliente is null || mascota is null)
        {
            return false;
        }

        if (cliente.Mascotas.Any(registrada => registrada.Id == mascota.Id))
        {
            return false;
        }

        // Se saca del dueño anterior: una mascota tiene un solo dueño a la vez.
        mascota.Dueno?.Mascotas.Remove(mascota);

        cliente.Mascotas.Add(mascota);
        mascota.Dueno = cliente;

        return true;
    }

    public bool DesasignarMascota(Guid clienteId, Guid mascotaId)
    {
        var cliente = _clienteRepository.ObtenerPorId(clienteId);
        var mascota = _mascotaRepository.ObtenerPorId(mascotaId);

        if (cliente is null || mascota is null)
        {
            return false;
        }

        if (!cliente.Mascotas.Remove(mascota))
        {
            return false;
        }

        mascota.Dueno = null;

        return true;
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

        cliente.Mascotas.Add(mascotaNueva);
        mascotaNueva.Dueno = cliente;

        return true;
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

    private static string ValidarTexto(string valor, string nombreDelParametro)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valor, nombreDelParametro);

        return valor.Trim();
    }

    /// <summary>
    /// Email y direccion siguen siendo opcionales para quien llama, pero Cliente los
    /// guarda como string no anulable: lo que no venga se almacena como cadena vacia.
    /// </summary>
    private static string NormalizarOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? string.Empty : valor.Trim();
    }
}