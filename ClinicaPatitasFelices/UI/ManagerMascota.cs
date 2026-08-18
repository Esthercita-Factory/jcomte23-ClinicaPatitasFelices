using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.UI;

public static class ManagerMascota
{
    public static void CrearUnaMascota()
    {
        var nombre = EntradaDeConsola.LeerTextoObligatorio("por favor ingrese el nombre de la mascota: ");
        var especie = EntradaDeConsola.LeerOpcionDeLista<Especie>("  Especie:");
        var raza = EntradaDeConsola.LeerTextoObligatorio("por favor ingrese la raza de la mascota: ");
        var fechaDeNacimiento = EntradaDeConsola.LeerFechaDeNacimiento("fecha de nacimiento (dd/mm/aaaa): ");
        var sexo = EntradaDeConsola.LeerOpcionDeLista<Sexo>("  Sexo:");

        var mascotaNueva = new Mascota(nombre, especie, raza, fechaDeNacimiento, sexo);

        MascotaRepository.RegistrarMascota(mascotaNueva);

        Console.WriteLine($"\n  >> Mascota registrada: {mascotaNueva}\n");
    }

    public static void MostrarTodasLasMascotas()
    {
        var mascotasDeLaBaseDeDatos = MascotaRepository.ListMascotas();

        if (mascotasDeLaBaseDeDatos.Count == 0)
        {
            Console.WriteLine("\n  >> No hay mascotas registradas.\n");
            return;
        }

        foreach (var mascota in mascotasDeLaBaseDeDatos)
        {
            MostrarDetalles(mascota);
            Console.WriteLine("--------------");
        }
    }

    /// <summary>
    /// El formato de salida es responsabilidad de la capa de presentacion,
    /// no del modelo.
    /// </summary>
    public static void MostrarDetalles(Mascota mascota)
    {
        Console.WriteLine($"Id: {mascota.Id}");
        Console.WriteLine($"Nombre: {mascota.Nombre}");
        Console.WriteLine($"Especie: {mascota.Especie}");
        Console.WriteLine($"Raza: {mascota.Raza}");
        Console.WriteLine($"Nacimiento: {mascota.FechaDeNacimiento:dd/MM/yyyy} ({mascota.EdadDescriptiva})");
        Console.WriteLine($"Sexo: {mascota.Sexo}");
        Console.WriteLine($"Peso: {(mascota.PesoEnKg is null ? "sin registrar" : $"{mascota.PesoEnKg} kg")}");
        Console.WriteLine($"Esterilizada: {(mascota.EstaEsterilizada ? "si" : "no")}");
        Console.WriteLine($"Dueño: {mascota.Dueno?.NombreCompleto ?? "sin asignar"}");
    }
}
