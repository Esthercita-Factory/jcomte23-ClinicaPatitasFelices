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
        return new Mascota
        {
            Nombre = "Simba",
            Especie = Especie.Gato,
            Raza = "Persa",
            FechaDeNacimiento = DateOnly.FromDateTime(DateTime.Today).AddMonths(-edadEnMeses)
        };
    }

    [Test]
    public void EdadEnMeses_SeCalculaContraLaFechaActual()
    {
        Assert.That(MascotaDe(30).EdadEnMeses, Is.EqualTo(30));
    }

    [Test]
    public void EdadEnMeses_NuncaEsNegativa()
    {
        var mascota = new Mascota { FechaDeNacimiento = DateOnly.FromDateTime(DateTime.Today).AddDays(5) };

        Assert.That(mascota.EdadEnMeses, Is.EqualTo(0));
    }

    [Test]
    public void EdadDescriptiva_SeExpresaEnAniosYMeses()
    {
        Assert.That(MascotaDe(14).EdadDescriptiva, Is.EqualTo("1 año y 2 meses"));
    }

    [Test]
    public void EdadDescriptiva_OmiteLosAniosCuandoNoLlegaAlPrimero()
    {
        Assert.That(MascotaDe(3).EdadDescriptiva, Is.EqualTo("3 meses"));
    }

    [Test]
    public void EdadDescriptiva_OmiteLosMesesCuandoElAnioEsExacto()
    {
        Assert.That(MascotaDe(24).EdadDescriptiva, Is.EqualTo("2 años"));
    }

    [Test]
    public void MascotaNueva_LlegaSinDuenoYConIdPropio()
    {
        var mascota = new Mascota();

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
        var cliente = new Cliente { Nombre = "Javier", Apellido = "Combita" };

        Assert.That(cliente.NombreCompleto, Is.EqualTo("Javier Combita"));
    }

    [Test]
    public void ClienteNuevo_LlegaConListaVaciaDeMascotasYConIdPropio()
    {
        var cliente = new Cliente();

        Assert.Multiple(() =>
        {
            Assert.That(cliente.Mascotas, Is.Empty);
            Assert.That(cliente.Id, Is.Not.EqualTo(Guid.Empty));
        });
    }
}
