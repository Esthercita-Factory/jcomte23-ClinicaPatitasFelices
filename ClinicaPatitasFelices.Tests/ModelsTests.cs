using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Tests;

/// <summary>
/// Los modelos son contenedores de datos: lo unico que tienen para probar son las
/// propiedades derivadas. Las reglas de negocio se prueban en los servicios.
/// </summary>
public class MascotaTests
{
    private static Mascota MascotaDe(int edadEnMeses)
    {
        return new Mascota(
            "Simba",
            Especie.Gato,
            "Persa",
            DateOnly.FromDateTime(DateTime.Today).AddMonths(-edadEnMeses),
            Sexo.Macho);
    }

    [Test]
    public void CalcularEdadEnMeses_SeCalculaContraLaFechaActual()
    {
        Assert.That(MascotaDe(30).CalcularEdadEnMeses(), Is.EqualTo(30));
    }

    [Test]
    public void CalcularEdadEnMeses_NuncaDevuelveNegativo()
    {
        var mascota = new Mascota(
            "Nube", Especie.Perro, "Criollo", DateOnly.FromDateTime(DateTime.Today).AddDays(5), Sexo.Hembra);

        Assert.That(mascota.CalcularEdadEnMeses(), Is.EqualTo(0));
    }

    [Test]
    public void DescribirEdad_SeExpresaEnAniosYMeses()
    {
        Assert.That(MascotaDe(14).DescribirEdad(), Is.EqualTo("1 año y 2 meses"));
    }

    [Test]
    public void DescribirEdad_OmiteLosAniosCuandoNoLlegaAlPrimero()
    {
        Assert.That(MascotaDe(3).DescribirEdad(), Is.EqualTo("3 meses"));
    }

    [Test]
    public void DescribirEdad_OmiteLosMesesCuandoElAnioEsExacto()
    {
        Assert.That(MascotaDe(24).DescribirEdad(), Is.EqualTo("2 años"));
    }

    [Test]
    public void MascotaNueva_LlegaSinDuenoYConIdPropio()
    {
        var mascota = MascotaDe(12);

        Assert.Multiple(() =>
        {
            Assert.That(mascota.Dueno, Is.Null);
            Assert.That(mascota.Id, Is.Not.EqualTo(Guid.Empty));
        });
    }
}

public class ClienteTests
{
    [Test]
    public void NombreCompleto_UneNombreYApellido()
    {
        var cliente = new Cliente("1030512345", "Javier", "Combita", "3001112233", "", "");

        Assert.That(cliente.NombreCompleto, Is.EqualTo("Javier Combita"));
    }

    [Test]
    public void ClienteNuevo_LlegaConListaVaciaDeMascotasYConIdPropio()
    {
        var cliente = new Cliente("1030512345", "Javier", "Combita", "3001112233", "", "");

        Assert.Multiple(() =>
        {
            Assert.That(cliente.Mascotas, Is.Empty);
            Assert.That(cliente.Id, Is.Not.EqualTo(Guid.Empty));
        });
    }
}
