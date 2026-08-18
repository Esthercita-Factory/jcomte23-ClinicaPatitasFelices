using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public static class MascotaRepository
{
    // La lista no se expone directamente para que nadie pueda reemplazarla ni
    // modificarla saltandose las operaciones del repositorio.
    private static readonly List<Mascota> Mascotas;

    static MascotaRepository()
    {
        Mascotas =
        [
            new Mascota("Firulais", Especie.Perro, "Criollo", HaceMeses(36), Sexo.Macho),
            new Mascota("Luna", Especie.Perro, "Labrador Retriever", HaceMeses(18), Sexo.Hembra),
            new Mascota("Rocky", Especie.Perro, "Bulldog Frances", HaceMeses(42), Sexo.Macho),
            new Mascota("Michi", Especie.Gato, "Siames", HaceMeses(24), Sexo.Macho),
            new Mascota("Toby", Especie.Perro, "Beagle", HaceMeses(60), Sexo.Macho),
            new Mascota("Nala", Especie.Perro, "Golden Retriever", HaceMeses(12), Sexo.Hembra),
            new Mascota("Simba", Especie.Gato, "Persa", HaceMeses(30), Sexo.Macho),
            new Mascota("Max", Especie.Perro, "Pastor Aleman", HaceMeses(54), Sexo.Macho),
            new Mascota("Kira", Especie.Perro, "Husky Siberiano", HaceMeses(27), Sexo.Hembra),
            new Mascota("Pelusa", Especie.Conejo, "Angora", HaceMeses(9), Sexo.Hembra),
            new Mascota("Bruno", Especie.Perro, "Rottweiler", HaceMeses(48), Sexo.Macho),
            new Mascota("Canela", Especie.Perro, "Cocker Spaniel", HaceMeses(21), Sexo.Hembra),
            new Mascota("Coco", Especie.Perro, "Chihuahua", HaceMeses(15), Sexo.Macho),
            new Mascota("Sasha", Especie.Perro, "Border Collie", HaceMeses(33), Sexo.Hembra),
            new Mascota("Manchas", Especie.Perro, "Dalmata", HaceMeses(39), Sexo.Macho),
            new Mascota("Nube", Especie.Perro, "Bichon Maltes", HaceMeses(6), Sexo.Hembra),
            new Mascota("Zeus", Especie.Perro, "Gran Danes", HaceMeses(45), Sexo.Macho),
            new Mascota("Mia", Especie.Gato, "Bengali", HaceMeses(11), Sexo.Hembra),
            new Mascota("Duque", Especie.Perro, "Schnauzer", HaceMeses(66), Sexo.Macho),
            new Mascota("Pepa", Especie.Perro, "Salchicha", HaceMeses(29), Sexo.Hembra)
        ];
    }

    // CREATE
    public static void RegistrarMascota(Mascota mascotaNueva)
    {
        ArgumentNullException.ThrowIfNull(mascotaNueva);

        Mascotas.Add(mascotaNueva);
    }

    // READ
    public static List<Mascota> ListMascotas()
    {
        // Copia: quien la reciba no debe poder alterar el almacen.
        return [.. Mascotas];
    }

    public static Mascota? BuscarPorId(Guid id)
    {
        return Mascotas.FirstOrDefault(mascota => mascota.Id == id);
    }

    public static Mascota? BuscarPorNombre(string nombre)
    {
        return Mascotas.FirstOrDefault(mascota => SonIguales(mascota.Nombre, nombre));
    }

    public static List<Mascota> BuscarPorRaza(string raza)
    {
        var razaBuscada = (raza ?? string.Empty).Trim();

        return Mascotas
            .Where(mascota => mascota.Raza.Contains(razaBuscada, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<Mascota> BuscarPorEspecie(Especie especie)
    {
        return Mascotas.Where(mascota => mascota.Especie == especie).ToList();
    }

    public static List<Mascota> BuscarPorDueno(Guid clienteId)
    {
        return Mascotas.Where(mascota => mascota.Dueno?.Id == clienteId).ToList();
    }

    public static List<Mascota> BuscarPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses)
    {
        return Mascotas
            .Where(mascota => mascota.EdadEnMeses >= edadMinimaEnMeses && mascota.EdadEnMeses <= edadMaximaEnMeses)
            .ToList();
    }

    // UPDATE
    public static bool ActualizarMascota(
        Guid id,
        string nombre,
        Especie especie,
        string raza,
        DateOnly fechaDeNacimiento,
        Sexo sexo)
    {
        var mascotaExistente = BuscarPorId(id);

        if (mascotaExistente is null)
        {
            return false;
        }

        mascotaExistente.ActualizarDatos(nombre, especie, raza, fechaDeNacimiento, sexo);

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

        // Se desvincula del dueño para no dejar al cliente apuntando a una mascota borrada.
        mascotaExistente.Dueno?.QuitarMascota(mascotaExistente);

        return Mascotas.Remove(mascotaExistente);
    }

    // VALIDACIONES / UTILIDADES
    public static bool ExisteId(Guid id)
    {
        return Mascotas.Any(mascota => mascota.Id == id);
    }

    public static bool ExisteNombre(string nombre)
    {
        return Mascotas.Any(mascota => SonIguales(mascota.Nombre, nombre));
    }

    public static int ContarMascotas()
    {
        return Mascotas.Count;
    }

    private static bool SonIguales(string valorGuardado, string valorBuscado)
    {
        return string.Equals(valorGuardado, (valorBuscado ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static DateOnly HaceMeses(int meses)
    {
        return DateOnly.FromDateTime(DateTime.Today).AddMonths(-meses);
    }
}
