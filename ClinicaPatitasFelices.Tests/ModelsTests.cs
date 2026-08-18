using ClinicaPatitasFelices.Models;

namespace ClinicaPatitasFelices.Tests;

public class ClienteTests
{
    private static Mascota CrearMascota(string nombre = "Firulais", int edadEnMeses = 24)
    {
        var nacimiento = DateOnly.FromDateTime(DateTime.Today).AddMonths(-edadEnMeses);

        return new Mascota(nombre, Especie.Perro, "Criollo", nacimiento, Sexo.Macho);
    }

    private static Cliente CrearCliente(string documento = "1030512345")
    {
        return new Cliente(documento, "Javier", "Combita", "3001234567");
    }

    [Test]
    public void AgregarMascota_VinculaLasDosPuntasDeLaRelacion()
    {
        var cliente = CrearCliente();
        var mascota = CrearMascota();

        var seAgrego = cliente.AgregarMascota(mascota);

        Assert.Multiple(() =>
        {
            Assert.That(seAgrego, Is.True);
            Assert.That(cliente.Mascotas, Has.Count.EqualTo(1));
            Assert.That(mascota.Dueno, Is.SameAs(cliente));
        });
    }

    [Test]
    public void AgregarMascota_UnClientePuedeTenerVarias()
    {
        var cliente = CrearCliente();

        cliente.AgregarMascota(CrearMascota("Luna"));
        cliente.AgregarMascota(CrearMascota("Michi"));
        cliente.AgregarMascota(CrearMascota("Nala"));

        Assert.That(cliente.CantidadDeMascotas, Is.EqualTo(3));
    }

    [Test]
    public void AgregarMascota_NoDuplicaLaMismaMascota()
    {
        var cliente = CrearCliente();
        var mascota = CrearMascota();

        cliente.AgregarMascota(mascota);
        var segundoIntento = cliente.AgregarMascota(mascota);

        Assert.Multiple(() =>
        {
            Assert.That(segundoIntento, Is.False);
            Assert.That(cliente.CantidadDeMascotas, Is.EqualTo(1));
        });
    }

    [Test]
    public void AgregarMascota_TransfiereLaMascotaYLaQuitaDelDuenoAnterior()
    {
        var duenoAnterior = CrearCliente("111");
        var duenoNuevo = CrearCliente("222");
        var mascota = CrearMascota();

        duenoAnterior.AgregarMascota(mascota);
        duenoNuevo.AgregarMascota(mascota);

        Assert.Multiple(() =>
        {
            Assert.That(duenoAnterior.Mascotas, Is.Empty);
            Assert.That(duenoNuevo.Mascotas, Has.Count.EqualTo(1));
            Assert.That(mascota.Dueno, Is.SameAs(duenoNuevo));
        });
    }

    [Test]
    public void QuitarMascota_DejaALaMascotaSinDueno()
    {
        var cliente = CrearCliente();
        var mascota = CrearMascota();
        cliente.AgregarMascota(mascota);

        var seQuito = cliente.QuitarMascota(mascota);

        Assert.Multiple(() =>
        {
            Assert.That(seQuito, Is.True);
            Assert.That(cliente.Mascotas, Is.Empty);
            Assert.That(mascota.Dueno, Is.Null);
        });
    }

    [Test]
    public void Constructor_RechazaDatosObligatoriosVacios()
    {
        Assert.Throws<ArgumentException>(() => new Cliente("  ", "Javier", "Combita", "3001234567"));
    }
}

public class MascotaTests
{
    [Test]
    public void EdadEnMeses_SeCalculaContraLaFechaActual()
    {
        var nacimiento = DateOnly.FromDateTime(DateTime.Today).AddMonths(-30);

        var mascota = new Mascota("Simba", Especie.Gato, "Persa", nacimiento);

        Assert.That(mascota.EdadEnMeses, Is.EqualTo(30));
    }

    [Test]
    public void EdadDescriptiva_SeExpresaEnAniosYMeses()
    {
        var nacimiento = DateOnly.FromDateTime(DateTime.Today).AddMonths(-14);

        var mascota = new Mascota("Coco", Especie.Perro, "Chihuahua", nacimiento);

        Assert.That(mascota.EdadDescriptiva, Is.EqualTo("1 año y 2 meses"));
    }

    [Test]
    public void Constructor_RechazaFechaDeNacimientoFutura()
    {
        var manana = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Mascota("Pelusa", Especie.Conejo, "Angora", manana));
    }

    [Test]
    public void Constructor_ConservaElNombreTalComoSeEscribio()
    {
        var mascota = new Mascota("  Firulais  ", Especie.Perro, "Criollo", DateOnly.FromDateTime(DateTime.Today));

        Assert.That(mascota.Nombre, Is.EqualTo("Firulais"));
    }

    [Test]
    public void RegistrarPeso_RechazaValoresNoPositivos()
    {
        var mascota = new Mascota("Zeus", Especie.Perro, "Gran Danes", DateOnly.FromDateTime(DateTime.Today));

        Assert.Throws<ArgumentOutOfRangeException>(() => mascota.RegistrarPeso(0));
    }
}
