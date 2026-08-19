using ClinicaPatitasFelices.Data;
using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly List<Cliente> _clientes;

    public ClienteRepository(AlmacenEnMemoria almacen)
    {
        ArgumentNullException.ThrowIfNull(almacen);

        _clientes = almacen.Clientes;
    }
    
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
        return _clientes.ToList();
    }

    public Cliente? ObtenerPorId(Guid id)
    {
        return _clientes.FirstOrDefault(cliente => cliente.Id == id);
    }

    public Cliente? ObtenerPorDocumento(string documento)
    {
        return _clientes.FirstOrDefault(cliente => string.Equals(cliente.Documento, documento, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Busca por coincidencia parcial en nombre o apellido.</summary>
    public List<Cliente> FiltrarPorNombre(string textoDeBusqueda)
    {
        if (textoDeBusqueda.Length == 0)
        {
            return [];
        }

        return _clientes
            .Where(cliente => cliente.NombreCompleto.Contains(textoDeBusqueda, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Cliente? ObtenerDuenoDeMascota(Guid mascotaId)
    {
        return _clientes.FirstOrDefault(
            cliente => cliente.Mascotas.Any(mascota => mascota.Id == mascotaId));
    }

    // UPDATE
    public bool Actualizar(Cliente cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);
        
        var registrado = ObtenerPorId(cliente.Id);

        if (registrado is null)
        {
            return false;
        }

        registrado.Nombre = cliente.Nombre;
        registrado.Apellido = cliente.Apellido;
        registrado.Documento = cliente.Documento;
        registrado.Telefono = cliente.Telefono;
        registrado.Email = cliente.Email;
        registrado.Direccion = cliente.Direccion;
        
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
