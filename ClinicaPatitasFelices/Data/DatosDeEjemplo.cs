using ClinicaPatitasFelices.Services;

namespace ClinicaPatitasFelices.Data;

/// <summary>
/// Vincula las mascotas y los clientes que cada repositorio crea por separado.
/// Se ejecuta desde el punto de composicion, una sola vez al arrancar.
/// </summary>
public static class DatosDeEjemplo
{
    public static void Sembrar(IClienteService clienteService, IMascotaService mascotaService)
    {
        Vincular(clienteService, mascotaService, "1030512345", "Firulais", "Michi");
        Vincular(clienteService, mascotaService, "52987654", "Luna", "Nala", "Pelusa");
        Vincular(clienteService, mascotaService, "79123456", "Rocky");
        Vincular(clienteService, mascotaService, "41556677", "Simba", "Mia");
    }

    private static void Vincular(
        IClienteService clienteService,
        IMascotaService mascotaService,
        string documento,
        params string[] nombresDeMascotas)
    {
        var cliente = clienteService.ConsultarClientePorDocumento(documento);

        if (cliente is null)
        {
            return;
        }

        foreach (var nombre in nombresDeMascotas)
        {
            var mascota = mascotaService.BuscarMascotaPorNombre(nombre);

            if (mascota is not null)
            {
                clienteService.AsignarMascota(cliente.Id, mascota.Id);
            }
        }
    }
}
