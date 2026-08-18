using ClinicaPatitasFelices.Data;
using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Repositories;

public class MascotaRepository : IMascotaRepository
{
    private readonly List<Mascota> _mascotas;

    public MascotaRepository(AlmacenEnMemoria almacen)
    {
        _mascotas = almacen.Mascotas;
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
            .Where(mascota =>
            {
                var edadEnMeses = mascota.CalcularEdadEnMeses();

                return edadEnMeses >= edadMinimaEnMeses && edadEnMeses <= edadMaximaEnMeses;
            })
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
}
