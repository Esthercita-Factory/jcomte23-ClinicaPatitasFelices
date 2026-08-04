using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.UI;

public class ManagerMascota
{
    
    public static void CrearUnaMascota()
    {
        Console.Write("por favor ingrese el nombre de la mascota: ");
        string nombre = Console.ReadLine();
        
        Console.Write("por favor ingrese la raza de la mascota: ");
        string raza = Console.ReadLine();
        
        Console.Write("por favor ingrese la edade en meses de la mascota ");
        int edadEnMeses = Convert.ToInt32(Console.ReadLine());
        
        var mascotaNueva = new Mascota(nombre, raza,  edadEnMeses);

        MascotaRepository.RegistrarMascota(mascotaNueva);
    }

    public static void MostrarTodasLasMascotas()
    {
        var mascotasDeLaBaseDeDatos=MascotaRepository.ListMascotas();

        foreach (var mascota in mascotasDeLaBaseDeDatos)
        {
            mascota.MostrarDetalles();
            Console.WriteLine("--------------");
        }
    }
    
}