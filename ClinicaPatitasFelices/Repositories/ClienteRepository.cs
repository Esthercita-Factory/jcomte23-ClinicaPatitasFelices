using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly List<Cliente> _clientes;

    // Se necesita para resolver la relacion cliente-mascota. Se recibe como
    // interfaz para poder sustituirlo (por una implementacion con base de datos
    // o por un doble de prueba) sin tocar esta clase.
    private readonly IMascotaRepository _mascotaRepository;

    public ClienteRepository(IMascotaRepository mascotaRepository)
    {
        ArgumentNullException.ThrowIfNull(mascotaRepository);

        _mascotaRepository = mascotaRepository;

        _clientes =
        [
            new Cliente("1030512345", "Javier", "Combita", "3001112233", "javier@correo.com", "Calle 12 #4-56"),
            new Cliente("52987654", "Marcela", "Rojas", "3104445566", "marcela@correo.com", "Carrera 7 #80-21"),
            new Cliente("79123456", "Andres", "Quintero", "3208889900", direccion: "Av. Siempre Viva 742"),
            new Cliente("41556677", "Lucia", "Barrera", "3013334455", "lucia@correo.com")
        ];

        AsignarMascotasDeEjemplo();
    }

    // CREATE
    /// <returns>false si ya existe un cliente con el mismo documento.</returns>
    public bool RegistrarCliente(Cliente clienteNuevo)
    {
        ArgumentNullException.ThrowIfNull(clienteNuevo);

        if (ExisteDocumento(clienteNuevo.Documento))
        {
            return false;
        }

        _clientes.Add(clienteNuevo);

        return true;
    }

    // READ
    public List<Cliente> ListClientes()
    {
        return [.. _clientes];
    }

    public Cliente? BuscarPorId(Guid id)
    {
        return _clientes.FirstOrDefault(cliente => cliente.Id == id);
    }

    public Cliente? BuscarPorDocumento(string documento)
    {
        var documentoBuscado = (documento ?? string.Empty).Trim();

        return _clientes.FirstOrDefault(
            cliente => string.Equals(cliente.Documento, documentoBuscado, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Busca por coincidencia parcial en nombre o apellido.</summary>
    public List<Cliente> BuscarPorNombre(string textoDeBusqueda)
    {
        var texto = (textoDeBusqueda ?? string.Empty).Trim();

        if (texto.Length == 0)
        {
            return [];
        }

        return _clientes
            .Where(cliente => cliente.NombreCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Cliente? BuscarDuenoDeMascota(Guid mascotaId)
    {
        return _clientes.FirstOrDefault(cliente => cliente.TieneMascota(mascotaId));
    }

    // UPDATE
    public bool ActualizarCliente(
        Guid id,
        string nombre,
        string apellido,
        string telefono,
        string? email,
        string? direccion)
    {
        var clienteExistente = BuscarPorId(id);

        if (clienteExistente is null)
        {
            return false;
        }

        clienteExistente.ActualizarDatosPersonales(nombre, apellido);
        clienteExistente.ActualizarDatosDeContacto(telefono, email, direccion);

        return true;
    }

    // DELETE
    /// <summary>
    /// Elimina al cliente pero NO a sus mascotas: quedan registradas en la clinica
    /// sin dueño asignado, listas para reasignarse.
    /// </summary>
    public bool EliminarCliente(Guid id)
    {
        var clienteExistente = BuscarPorId(id);

        if (clienteExistente is null)
        {
            return false;
        }

        foreach (var mascota in clienteExistente.Mascotas.ToList())
        {
            clienteExistente.QuitarMascota(mascota);
        }

        return _clientes.Remove(clienteExistente);
    }

    // RELACION CON MASCOTAS
    /// <summary>
    /// Vincula una mascota ya registrada con un cliente. Si la mascota tenia otro
    /// dueño, se transfiere.
    /// </summary>
    public bool AsignarMascota(Guid clienteId, Guid mascotaId)
    {
        var cliente = BuscarPorId(clienteId);
        var mascota = _mascotaRepository.BuscarPorId(mascotaId);

        if (cliente is null || mascota is null)
        {
            return false;
        }

        return cliente.AgregarMascota(mascota);
    }

    public bool DesasignarMascota(Guid clienteId, Guid mascotaId)
    {
        var cliente = BuscarPorId(clienteId);
        var mascota = _mascotaRepository.BuscarPorId(mascotaId);

        if (cliente is null || mascota is null)
        {
            return false;
        }

        return cliente.QuitarMascota(mascota);
    }

    public List<Mascota> ListarMascotasDe(Guid clienteId)
    {
        var cliente = BuscarPorId(clienteId);

        return cliente is null ? [] : [.. cliente.Mascotas];
    }

    // VALIDACIONES / UTILIDADES
    public bool ExisteId(Guid id)
    {
        return _clientes.Any(cliente => cliente.Id == id);
    }

    public bool ExisteDocumento(string documento)
    {
        return BuscarPorDocumento(documento) is not null;
    }

    public int ContarClientes()
    {
        return _clientes.Count;
    }

    /// <summary>
    /// Reparte algunas de las mascotas del seed entre los clientes del seed para que
    /// la relacion quede visible al arrancar. Las mascotas que no aparecen aqui
    /// quedan sin dueño a proposito.
    /// </summary>
    private void AsignarMascotasDeEjemplo()
    {
        Vincular("1030512345", "Firulais", "Michi");
        Vincular("52987654", "Luna", "Nala", "Pelusa");
        Vincular("79123456", "Rocky");
        Vincular("41556677", "Simba", "Mia");
    }

    private void Vincular(string documento, params string[] nombresDeMascotas)
    {
        var cliente = BuscarPorDocumento(documento);

        if (cliente is null)
        {
            return;
        }

        foreach (var nombre in nombresDeMascotas)
        {
            var mascota = _mascotaRepository.BuscarPorNombre(nombre);

            if (mascota is not null)
            {
                cliente.AgregarMascota(mascota);
            }
        }
    }
}
