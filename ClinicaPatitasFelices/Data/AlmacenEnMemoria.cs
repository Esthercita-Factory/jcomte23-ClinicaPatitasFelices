using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Data;

public class AlmacenEnMemoria
{
    public List<Cliente> Clientes { get; } = [];
    public List<Mascota> Mascotas { get; } = [];
}
