using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public interface IClienteRepository
{
    // CREATE
    public bool Registrar(Cliente clienteNuevo);

    // READ
    public List<Cliente> ObtenerTodos();
    public Cliente? ObtenerPorId(Guid id);
    public Cliente? ObtenerPorDocumento(string documento);
    public List<Cliente> FiltrarPorNombre(string textoDeBusqueda);
    public Cliente? ObtenerDuenoDeMascota(Guid mascotaId);

    // UPDATE
    public bool Actualizar(Cliente cliente);

    // DELETE
    public bool Eliminar(Guid id);

    // CONSULTA DE LA RELACION
    public List<Mascota> ObtenerMascotasDe(Guid clienteId);

    // VALIDACIONES / UTILIDADES
    public bool ExisteId(Guid id);
    public bool ExisteDocumento(string documento);
    public int Contar();
}
