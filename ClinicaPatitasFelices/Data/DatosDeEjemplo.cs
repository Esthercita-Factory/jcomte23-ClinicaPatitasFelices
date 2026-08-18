using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Services;

namespace ClinicaPatitasFelices.Data;

/// <summary>
/// Llena el almacen con datos de prueba. Los repositorios ya no traen datos propios:
/// se siembra desde aqui, a traves de los servicios, para que todo pase por sus
/// validaciones. Se ejecuta desde el punto de composicion, una sola vez al arrancar.
/// </summary>
public static class DatosDeEjemplo
{
    public static void Sembrar(IClienteService clienteService, IMascotaService mascotaService)
    {
        SembrarMascotas(mascotaService);
        SembrarClientes(clienteService);
        VincularMascotasConSusDuenos(clienteService, mascotaService);
    }

    public static void SembrarMascotas(IMascotaService mascotaService)
    {
        mascotaService.CrearMascota("Firulais", Especie.Perro, "Criollo", HaceMeses(36), Sexo.Macho);
        mascotaService.CrearMascota("Luna", Especie.Perro, "Labrador Retriever", HaceMeses(18), Sexo.Hembra);
        mascotaService.CrearMascota("Rocky", Especie.Perro, "Bulldog Frances", HaceMeses(42), Sexo.Macho);
        mascotaService.CrearMascota("Michi", Especie.Gato, "Siames", HaceMeses(24), Sexo.Macho);
        mascotaService.CrearMascota("Toby", Especie.Perro, "Beagle", HaceMeses(60), Sexo.Macho);
        mascotaService.CrearMascota("Nala", Especie.Perro, "Golden Retriever", HaceMeses(12), Sexo.Hembra);
        mascotaService.CrearMascota("Simba", Especie.Gato, "Persa", HaceMeses(30), Sexo.Macho);
        mascotaService.CrearMascota("Max", Especie.Perro, "Pastor Aleman", HaceMeses(54), Sexo.Macho);
        mascotaService.CrearMascota("Kira", Especie.Perro, "Husky Siberiano", HaceMeses(27), Sexo.Hembra);
        mascotaService.CrearMascota("Pelusa", Especie.Conejo, "Angora", HaceMeses(9), Sexo.Hembra);
        mascotaService.CrearMascota("Bruno", Especie.Perro, "Rottweiler", HaceMeses(48), Sexo.Macho);
        mascotaService.CrearMascota("Canela", Especie.Perro, "Cocker Spaniel", HaceMeses(21), Sexo.Hembra);
        mascotaService.CrearMascota("Coco", Especie.Perro, "Chihuahua", HaceMeses(15), Sexo.Macho);
        mascotaService.CrearMascota("Sasha", Especie.Perro, "Border Collie", HaceMeses(33), Sexo.Hembra);
        mascotaService.CrearMascota("Manchas", Especie.Perro, "Dalmata", HaceMeses(39), Sexo.Macho);
        mascotaService.CrearMascota("Nube", Especie.Perro, "Bichon Maltes", HaceMeses(6), Sexo.Hembra);
        mascotaService.CrearMascota("Zeus", Especie.Perro, "Gran Danes", HaceMeses(45), Sexo.Macho);
        mascotaService.CrearMascota("Mia", Especie.Gato, "Bengali", HaceMeses(11), Sexo.Hembra);
        mascotaService.CrearMascota("Duque", Especie.Perro, "Schnauzer", HaceMeses(66), Sexo.Macho);
        mascotaService.CrearMascota("Pepa", Especie.Perro, "Salchicha", HaceMeses(29), Sexo.Hembra);
    }

    public static void SembrarClientes(IClienteService clienteService)
    {
        clienteService.CrearCliente(
            "1030512345", "Javier", "Combita", "3001112233", "javier@correo.com", "Calle 12 #4-56");
        clienteService.CrearCliente(
            "52987654", "Marcela", "Rojas", "3104445566", "marcela@correo.com", "Carrera 7 #80-21");
        clienteService.CrearCliente(
            "79123456", "Andres", "Quintero", "3208889900", null, "Av. Siempre Viva 742");
        clienteService.CrearCliente(
            "41556677", "Lucia", "Barrera", "3013334455", "lucia@correo.com", null);
    }

    /// <summary>Las mascotas que no aparecen aqui quedan sin dueño a proposito.</summary>
    public static void VincularMascotasConSusDuenos(IClienteService clienteService, IMascotaService mascotaService)
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

    private static DateOnly HaceMeses(int meses)
    {
        return DateOnly.FromDateTime(DateTime.Today).AddMonths(-meses);
    }
}
