using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly List<Cliente> _clientes;

    public ClienteRepository()
    {
        _clientes =
        [
            new Cliente("1030512345", "Javier", "Combita", "3001112233", "javier@correo.com", "Calle 12 #4-56"),
            new Cliente("52987654", "Marcela", "Rojas", "3104445566", "marcela@correo.com", "Carrera 7 #80-21"),
            new Cliente("79123456", "Andres", "Quintero", "3208889900", direccion: "Av. Siempre Viva 742"),
            new Cliente("41556677", "Lucia", "Barrera", "3013334455", "lucia@correo.com")
        ];
    }

    // CREATE
    /// <returns>false si ya existe un cliente con el mismo documento.</returns>
    public bool Registrar(Cliente clienteNuevo)
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
    public List<Cliente> ObtenerTodos()
    {
        return [.. _clientes];
    }

    public Cliente? ObtenerPorId(Guid id)
    {
        return _clientes.FirstOrDefault(cliente => cliente.Id == id);
    }

    public Cliente? ObtenerPorDocumento(string documento)
    {
        var documentoBuscado = (documento ?? string.Empty).Trim();

        return _clientes.FirstOrDefault(
            cliente => string.Equals(cliente.Documento, documentoBuscado, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Busca por coincidencia parcial en nombre o apellido.</summary>
    public List<Cliente> FiltrarPorNombre(string textoDeBusqueda)
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

    public Cliente? ObtenerDuenoDeMascota(Guid mascotaId)
    {
        return _clientes.FirstOrDefault(cliente => cliente.TieneMascota(mascotaId));
    }

    // UPDATE
    public bool Actualizar(
        Guid id,
        string nombre,
        string apellido,
        string telefono,
        string? email,
        string? direccion)
    {
        var clienteExistente = ObtenerPorId(id);

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
    public bool Eliminar(Guid id)
    {
        var clienteExistente = ObtenerPorId(id);

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

    // CONSULTA DE LA RELACION
    public List<Mascota> ObtenerMascotasDe(Guid clienteId)
    {
        var cliente = ObtenerPorId(clienteId);

        return cliente is null ? [] : [.. cliente.Mascotas];
    }

    // VALIDACIONES / UTILIDADES
    public bool ExisteId(Guid id)
    {
        return _clientes.Any(cliente => cliente.Id == id);
    }

    public bool ExisteDocumento(string documento)
    {
        return ObtenerPorDocumento(documento) is not null;
    }

    public int Contar()
    {
        return _clientes.Count;
    }
}
