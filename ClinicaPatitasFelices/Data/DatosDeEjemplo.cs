using ClinicaPatitasFelices.Services;

namespace ClinicaPatitasFelices.Data;

/// <summary>
/// Crea los clientes de ejemplo y los vincula con las mascotas del almacen.
/// Se ejecuta desde el punto de composicion, una sola vez al arrancar.
/// </summary>
public static class DatosDeEjemplo
{
    public static void Sembrar(IClienteService clienteService, IMascotaService mascotaService)
    {
        // Los clientes se crean por el servicio para que pasen por sus validaciones.
        clienteService.CrearCliente(
            "1030512345", "Javier", "Combita", "3001112233", "javier@correo.com", "Calle 12 #4-56");
        clienteService.CrearCliente(
            "52987654", "Marcela", "Rojas", "3104445566", "marcela@correo.com", "Carrera 7 #80-21");
        clienteService.CrearCliente(
            "79123456", "Andres", "Quintero", "3208889900", null, "Av. Siempre Viva 742");
        clienteService.CrearCliente(
            "41556677", "Lucia", "Barrera", "3013334455", "lucia@correo.com", null);

        // Las mascotas que no aparecen aqui quedan sin dueño a proposito.
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
