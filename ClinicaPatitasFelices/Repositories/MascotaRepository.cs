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
            new Mascota { Nombre = "Firulais", Especie = Especie.Perro, Raza = "Criollo", FechaDeNacimiento = HaceMeses(36), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Luna", Especie = Especie.Perro, Raza = "Labrador Retriever", FechaDeNacimiento = HaceMeses(18), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Rocky", Especie = Especie.Perro, Raza = "Bulldog Frances", FechaDeNacimiento = HaceMeses(42), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Michi", Especie = Especie.Gato, Raza = "Siames", FechaDeNacimiento = HaceMeses(24), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Toby", Especie = Especie.Perro, Raza = "Beagle", FechaDeNacimiento = HaceMeses(60), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Nala", Especie = Especie.Perro, Raza = "Golden Retriever", FechaDeNacimiento = HaceMeses(12), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Simba", Especie = Especie.Gato, Raza = "Persa", FechaDeNacimiento = HaceMeses(30), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Max", Especie = Especie.Perro, Raza = "Pastor Aleman", FechaDeNacimiento = HaceMeses(54), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Kira", Especie = Especie.Perro, Raza = "Husky Siberiano", FechaDeNacimiento = HaceMeses(27), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Pelusa", Especie = Especie.Conejo, Raza = "Angora", FechaDeNacimiento = HaceMeses(9), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Bruno", Especie = Especie.Perro, Raza = "Rottweiler", FechaDeNacimiento = HaceMeses(48), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Canela", Especie = Especie.Perro, Raza = "Cocker Spaniel", FechaDeNacimiento = HaceMeses(21), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Coco", Especie = Especie.Perro, Raza = "Chihuahua", FechaDeNacimiento = HaceMeses(15), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Sasha", Especie = Especie.Perro, Raza = "Border Collie", FechaDeNacimiento = HaceMeses(33), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Manchas", Especie = Especie.Perro, Raza = "Dalmata", FechaDeNacimiento = HaceMeses(39), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Nube", Especie = Especie.Perro, Raza = "Bichon Maltes", FechaDeNacimiento = HaceMeses(6), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Zeus", Especie = Especie.Perro, Raza = "Gran Danes", FechaDeNacimiento = HaceMeses(45), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Mia", Especie = Especie.Gato, Raza = "Bengali", FechaDeNacimiento = HaceMeses(11), Sexo = Sexo.Hembra },
            new Mascota { Nombre = "Duque", Especie = Especie.Perro, Raza = "Schnauzer", FechaDeNacimiento = HaceMeses(66), Sexo = Sexo.Macho },
            new Mascota { Nombre = "Pepa", Especie = Especie.Perro, Raza = "Salchicha", FechaDeNacimiento = HaceMeses(29), Sexo = Sexo.Hembra }
        ];
    }

    // CREATE
    public void Registrar(Mascota mascotaNueva)
    {
        ArgumentNullException.ThrowIfNull(mascotaNueva);

        _mascotas.Add(mascotaNueva);
    }

    // READ
    public List<Mascota> ObtenerTodas()
    {
        // Copia: quien la reciba no debe poder alterar el almacen.
        return [.. _mascotas];
    }

    public Mascota? ObtenerPorId(Guid id)
    {
        return _mascotas.FirstOrDefault(mascota => mascota.Id == id);
    }

    public Mascota? ObtenerPorNombre(string nombre)
    {
        return _mascotas.FirstOrDefault(mascota => SonIguales(mascota.Nombre, nombre));
    }

    public List<Mascota> FiltrarPorRaza(string raza)
    {
        var razaBuscada = (raza ?? string.Empty).Trim();

        return _mascotas
            .Where(mascota => mascota.Raza.Contains(razaBuscada, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Mascota> FiltrarPorEspecie(Especie especie)
    {
        return _mascotas.Where(mascota => mascota.Especie == especie).ToList();
    }

    public List<Mascota> FiltrarPorDueno(Guid clienteId)
    {
        return _mascotas.Where(mascota => mascota.Dueno?.Id == clienteId).ToList();
    }

    public List<Mascota> FiltrarPorRangoDeEdad(int edadMinimaEnMeses, int edadMaximaEnMeses)
    {
        return _mascotas
            .Where(mascota => mascota.EdadEnMeses >= edadMinimaEnMeses && mascota.EdadEnMeses <= edadMaximaEnMeses)
            .ToList();
    }

    // UPDATE
    public bool Actualizar(Mascota mascota)
    {
        ArgumentNullException.ThrowIfNull(mascota);

        var indice = _mascotas.FindIndex(registrada => registrada.Id == mascota.Id);

        if (indice < 0)
        {
            return false;
        }

        _mascotas[indice] = mascota;

        return true;
    }

    // DELETE
    public bool Eliminar(Guid id)
    {
        var mascotaExistente = ObtenerPorId(id);

        if (mascotaExistente is null)
        {
            return false;
        }

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

    public int Contar()
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
