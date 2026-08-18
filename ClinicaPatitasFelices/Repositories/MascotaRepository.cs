using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public static class MascotaRepository
{
    public static List<Mascota> Mascotas { get; set; }

    static MascotaRepository()
    {
        Mascotas =
        [
            new Mascota("Firulais", "Criollo", 36),
            new Mascota("Luna", "Labrador Retriever", 18),
            new Mascota("Rocky", "Bulldog Frances", 42),
            new Mascota("Michi", "Siames", 24),
            new Mascota("Toby", "Beagle", 60),
            new Mascota("Nala", "Golden Retriever", 12),
            new Mascota("Simba", "Persa", 30),
            new Mascota("Max", "Pastor Aleman", 54),
            new Mascota("Kira", "Husky Siberiano", 27),
            new Mascota("Pelusa", "Angora", 9),
            new Mascota("Bruno", "Rottweiler", 48),
            new Mascota("Canela", "Cocker Spaniel", 21),
            new Mascota("Coco", "Chihuahua", 15),
            new Mascota("Sasha", "Border Collie", 33),
            new Mascota("Manchas", "Dalmata", 39),
            new Mascota("Nube", "Bichon Maltes", 6),
            new Mascota("Zeus", "Gran Danes", 45),
            new Mascota("Mia", "Bengali", 11),
            new Mascota("Duque", "Schnauzer", 66),
            new Mascota("Pepa", "Salchicha", 29)
        ];
    }

    // CREATE
    public static void RegistrarMascota(Mascota mascotaNueva)
    {
        Mascotas.Add(mascotaNueva);
    }
    
    // READ
    public static List<Mascota> ListMascotas()
    {
        return Mascotas;
    }

    public static Mascota? BuscarPorId(Guid id)
    {
        return Mascotas.FirstOrDefault(mascota => mascota.Id == id);
    }

    public static Mascota? BuscarPorNombre(string nombre)
    {
        var nombreNormalizado = Normalizar(nombre);

        return Mascotas.FirstOrDefault(mascota => mascota.Nombre == nombreNormalizado);
    }

    public static List<Mascota> BuscarPorRaza(string raza)
    {
        var razaNormalizada = Normalizar(raza);

        return Mascotas.Where(mascota => mascota.Raza.Contains(razaNormalizada)).ToList();
    }

    public static List<Mascota> BuscarPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses)
    {
        return Mascotas
            .Where(mascota => mascota.EdadEnMeses >= edadMinimaEnMeses && mascota.EdadEnMeses <= edadMaximaEnMeses)
            .ToList();
    }

    // UPDATE
    public static bool ActualizarMascota(Guid id, string nombre, string raza, int edadEnMeses)
    {
        var mascotaExistente = BuscarPorId(id);

        if (mascotaExistente is null)
        {
            return false;
        }

        mascotaExistente.Nombre = Normalizar(nombre);
        mascotaExistente.Raza = Normalizar(raza);
        mascotaExistente.EdadEnMeses = edadEnMeses;

        return true;
    }

    // DELETE
    public static bool EliminarMascota(Guid id)
    {
        var mascotaExistente = BuscarPorId(id);

        if (mascotaExistente is null)
        {
            return false;
        }

        return Mascotas.Remove(mascotaExistente);
    }

    // VALIDACIONES / UTILIDADES
    public static bool ExisteId(Guid id)
    {
        return Mascotas.Any(mascota => mascota.Id == id);
    }

    public static bool ExisteNombre(string nombre)
    {
        var nombreNormalizado = Normalizar(nombre);

        return Mascotas.Any(mascota => mascota.Nombre == nombreNormalizado);
    }

    public static int ContarMascotas()
    {
        return Mascotas.Count;
    }

    // Deja el texto igual que como lo guarda el constructor de Mascota
    private static string Normalizar(string texto)
    {
        return texto.Trim().ToLower();
    }
}