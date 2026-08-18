using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public class MascotaRepository : IMascotaRepository
{
    // Cada instancia tiene su propio almacen. En la aplicacion se registra una sola
    // instancia (singleton) desde el punto de composicion.
    private readonly List<Mascota> _mascotas;

    public MascotaRepository()
    {
        _mascotas =
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
    public void RegistrarMascota(Mascota mascotaNueva)
    {
        ArgumentNullException.ThrowIfNull(mascotaNueva);

        _mascotas.Add(mascotaNueva);
    }

    // READ
    public List<Mascota> ListMascotas()
    {
        // Copia: quien la reciba no debe poder alterar el almacen.
        return [.. _mascotas];
    }

    public Mascota? BuscarPorId(Guid id)
    {
        return _mascotas.FirstOrDefault(mascota => mascota.Id == id);
    }

    public Mascota? BuscarPorNombre(string nombre)
    {
        return _mascotas.FirstOrDefault(mascota => SonIguales(mascota.Nombre, nombre));
    }

    public List<Mascota> BuscarPorRaza(string raza)
    {
        var razaBuscada = (raza ?? string.Empty).Trim();

        return _mascotas
            .Where(mascota => mascota.Raza.Contains(razaBuscada, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Mascota> BuscarPorEspecie(Especie especie)
    {
        return _mascotas.Where(mascota => mascota.Especie == especie).ToList();
    }

    public List<Mascota> BuscarPorDueno(Guid clienteId)
    {
        return _mascotas.Where(mascota => mascota.Dueno?.Id == clienteId).ToList();
    }

    public List<Mascota> BuscarPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses)
    {
        return _mascotas
            .Where(mascota => mascota.EdadEnMeses >= edadMinimaEnMeses && mascota.EdadEnMeses <= edadMaximaEnMeses)
            .ToList();
    }

    // UPDATE
    public bool ActualizarMascota(
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
    public bool EliminarMascota(Guid id)
    {
        var mascotaExistente = BuscarPorId(id);

        if (mascotaExistente is null)
        {
            return false;
        }

        // Se desvincula del dueño para no dejar al cliente apuntando a una mascota borrada.
        mascotaExistente.Dueno?.QuitarMascota(mascotaExistente);

        return _mascotas.Remove(mascotaExistente);
    }

    // VALIDACIONES / UTILIDADES
    public bool ExisteId(Guid id)
    {
        return _mascotas.Any(mascota => mascota.Id == id);
    }

    public bool ExisteNombre(string nombre)
    {
        return _mascotas.Any(mascota => SonIguales(mascota.Nombre, nombre));
    }

    public int ContarMascotas()
    {
        return _mascotas.Count;
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
