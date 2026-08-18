using ClinicaPatitasFelices.Data;
using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;
using ClinicaPatitasFelices.Services;

namespace ClinicaPatitasFelices.Tests;

public class ClienteServiceTests
{
    private IMascotaRepository _mascotaRepository = null!;
    private IClienteService _clienteService = null!;
    private IMascotaService _mascotaService = null!;

    [SetUp]
    public void Setup()
    {
        _mascotaRepository = new MascotaRepository();
        IClienteRepository clienteRepository = new ClienteRepository();

        _mascotaService = new MascotaService(_mascotaRepository);
        _clienteService = new ClienteService(clienteRepository, _mascotaRepository);
    }

    private Cliente CrearCliente(string documento = "9999")
    {
        return _clienteService.CrearCliente(documento, "Prueba", "Apellido", "3000000000", null, null)!;
    }

    [Test]
    public void CrearCliente_DevuelveElClienteCreado()
    {
        var cliente = _clienteService.CrearCliente(
            "1122334455", "Javier", "Combita", "3001112233", "javier@correo.com", null);

        Assert.That(cliente, Is.Not.Null);
        Assert.That(cliente!.NombreCompleto, Is.EqualTo("Javier Combita"));
    }

    [Test]
    public void CrearCliente_DevuelveNullSiElDocumentoYaExiste()
    {
        CrearCliente("1122334455");

        var duplicado = _clienteService.CrearCliente("1122334455", "Otro", "Cliente", "3009999999", null, null);

        Assert.Multiple(() =>
        {
            Assert.That(duplicado, Is.Null);
            Assert.That(_clienteService.ConsultarClientes().Count(c => c.Documento == "1122334455"), Is.EqualTo(1));
        });
    }

    [Test]
    public void CrearCliente_PropagaLaValidacionDelModelo()
    {
        Assert.Throws<ArgumentException>(
            () => _clienteService.CrearCliente("   ", "Javier", "Combita", "3001112233", null, null));
    }

    [Test]
    public void AsignarMascota_VinculaLasDosEntidades()
    {
        var cliente = CrearCliente();
        var mascota = _mascotaService.CrearMascota(
            "Tomas", Especie.Gato, "Criollo", DateOnly.FromDateTime(DateTime.Today).AddMonths(-10), Sexo.Macho);

        var seAsigno = _clienteService.AsignarMascota(cliente.Id, mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seAsigno, Is.True);
            Assert.That(mascota.Dueno, Is.SameAs(cliente));
            Assert.That(_clienteService.ConsultarMascotasDe(cliente.Id), Has.Count.EqualTo(1));
            Assert.That(_clienteService.ConsultarDuenoDe(mascota.Id), Is.SameAs(cliente));
        });
    }

    [Test]
    public void AsignarMascota_TransfiereLaMascotaEntreClientes()
    {
        var duenoAnterior = CrearCliente("111");
        var duenoNuevo = CrearCliente("222");
        var mascota = _mascotaService.ConsultarMascotas().First();
        _clienteService.AsignarMascota(duenoAnterior.Id, mascota.Id);

        var seTransfirio = _clienteService.AsignarMascota(duenoNuevo.Id, mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seTransfirio, Is.True);
            Assert.That(_clienteService.ConsultarMascotasDe(duenoAnterior.Id), Is.Empty);
            Assert.That(_clienteService.ConsultarMascotasDe(duenoNuevo.Id), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void AsignarMascota_RechazaLaAsignacionRepetida()
    {
        var cliente = CrearCliente();
        var mascota = _mascotaService.ConsultarMascotas().First();
        _clienteService.AsignarMascota(cliente.Id, mascota.Id);

        Assert.That(_clienteService.AsignarMascota(cliente.Id, mascota.Id), Is.False);
    }

    [Test]
    public void AsignarMascota_DevuelveFalseSiFaltaElClienteOLaMascota()
    {
        var cliente = CrearCliente();
        var mascota = _mascotaService.ConsultarMascotas().First();

        Assert.Multiple(() =>
        {
            Assert.That(_clienteService.AsignarMascota(Guid.NewGuid(), mascota.Id), Is.False);
            Assert.That(_clienteService.AsignarMascota(cliente.Id, Guid.NewGuid()), Is.False);
        });
    }

    [Test]
    public void DesasignarMascota_DejaLaMascotaSinDueno()
    {
        var cliente = CrearCliente();
        var mascota = _mascotaService.ConsultarMascotas().First();
        _clienteService.AsignarMascota(cliente.Id, mascota.Id);

        var seDesasigno = _clienteService.DesasignarMascota(cliente.Id, mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seDesasigno, Is.True);
            Assert.That(mascota.Dueno, Is.Null);
        });
    }

    [Test]
    public void CrearMascotaParaCliente_RegistraYVinculaEnUnSoloPaso()
    {
        var cliente = CrearCliente();
        var mascotaNueva = new Mascota(
            "Huesos", Especie.Perro, "Criollo", DateOnly.FromDateTime(DateTime.Today).AddMonths(-8));

        var seCreo = _clienteService.CrearMascotaParaCliente(cliente.Id, mascotaNueva);

        Assert.Multiple(() =>
        {
            Assert.That(seCreo, Is.True);
            Assert.That(_mascotaRepository.ObtenerPorId(mascotaNueva.Id), Is.Not.Null);
            Assert.That(mascotaNueva.Dueno, Is.SameAs(cliente));
        });
    }

    [Test]
    public void CrearMascotaParaCliente_DevuelveFalseSiElClienteNoExiste()
    {
        var mascotaNueva = new Mascota(
            "Huesos", Especie.Perro, "Criollo", DateOnly.FromDateTime(DateTime.Today).AddMonths(-8));

        Assert.That(_clienteService.CrearMascotaParaCliente(Guid.NewGuid(), mascotaNueva), Is.False);
    }

    [Test]
    public void RetirarCliente_DejaSusMascotasSinDuenoPeroRegistradas()
    {
        var cliente = CrearCliente();
        var mascotas = _mascotaService.ConsultarMascotas().Take(2).ToList();
        foreach (var mascota in mascotas)
        {
            _clienteService.AsignarMascota(cliente.Id, mascota.Id);
        }

        var seRetiro = _clienteService.RetirarCliente(cliente.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seRetiro, Is.True);
            Assert.That(_clienteService.ConsultarCliente(cliente.Id), Is.Null);
            Assert.That(mascotas.All(mascota => mascota.Dueno is null), Is.True);
            Assert.That(mascotas.All(mascota => _mascotaRepository.ObtenerPorId(mascota.Id) is not null), Is.True);
        });
    }

    [Test]
    public void RetirarCliente_DevuelveFalseSiNoExiste()
    {
        Assert.That(_clienteService.RetirarCliente(Guid.NewGuid()), Is.False);
    }

    [Test]
    public void DatosDeEjemplo_VinculaMascotasConSusDuenos()
    {
        DatosDeEjemplo.Sembrar(_clienteService, _mascotaService);

        var cliente = _clienteService.ConsultarClientePorDocumento("52987654");

        Assert.That(cliente, Is.Not.Null);
        Assert.That(cliente!.CantidadDeMascotas, Is.EqualTo(3));
    }
}

public class MascotaServiceTests
{
    private IMascotaService _mascotaService = null!;

    [SetUp]
    public void Setup()
    {
        _mascotaService = new MascotaService(new MascotaRepository());
    }

    [Test]
    public void CrearMascota_DevuelveLaMascotaRegistrada()
    {
        var nacimiento = DateOnly.FromDateTime(DateTime.Today).AddMonths(-10);

        var mascota = _mascotaService.CrearMascota("Tomas", Especie.Gato, "Criollo", nacimiento, Sexo.Macho);

        Assert.Multiple(() =>
        {
            Assert.That(mascota.Nombre, Is.EqualTo("Tomas"));
            Assert.That(_mascotaService.ConsultarMascota(mascota.Id), Is.SameAs(mascota));
            Assert.That(_mascotaService.ContarMascotas(), Is.EqualTo(21));
        });
    }

    [Test]
    public void CrearMascota_PropagaLaValidacionDeFechaFutura()
    {
        var manana = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => _mascotaService.CrearMascota("Pelusa", Especie.Conejo, "Angora", manana, Sexo.Hembra));
    }

    [Test]
    public void BuscarMascotasPorRangoDeEdad_RechazaUnRangoInvertido()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _mascotaService.BuscarMascotasPorRangoDeEdad(60, 12));
    }

    [Test]
    public void BuscarMascotasPorRangoDeEdad_RechazaEdadesNegativas()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _mascotaService.BuscarMascotasPorRangoDeEdad(-1, 12));
    }

    [Test]
    public void BuscarCachorros_SoloDevuelveMenoresDeUnAnio()
    {
        var cachorros = _mascotaService.BuscarCachorros();

        Assert.Multiple(() =>
        {
            Assert.That(cachorros, Is.Not.Empty);
            Assert.That(cachorros.All(mascota => mascota.EdadEnMeses <= 12), Is.True);
        });
    }

    [Test]
    public void MarcarComoEsterilizada_NoPermiteMarcarDosVeces()
    {
        var mascota = _mascotaService.ConsultarMascotas().First();

        var primera = _mascotaService.MarcarComoEsterilizada(mascota.Id);
        var segunda = _mascotaService.MarcarComoEsterilizada(mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(primera, Is.True);
            Assert.That(segunda, Is.False);
            Assert.That(mascota.EstaEsterilizada, Is.True);
        });
    }

    [Test]
    public void AnotarPeso_GuardaElValorCuandoEsValido()
    {
        var mascota = _mascotaService.ConsultarMascotas().First();

        var seAnoto = _mascotaService.AnotarPeso(mascota.Id, 12.5m);

        Assert.Multiple(() =>
        {
            Assert.That(seAnoto, Is.True);
            Assert.That(mascota.PesoEnKg, Is.EqualTo(12.5m));
        });
    }

    [Test]
    public void AnotarPeso_PropagaLaValidacionDelModelo()
    {
        var mascota = _mascotaService.ConsultarMascotas().First();

        Assert.Throws<ArgumentOutOfRangeException>(() => _mascotaService.AnotarPeso(mascota.Id, 0));
    }

    [Test]
    public void AnotarPeso_DevuelveFalseSiLaMascotaNoExiste()
    {
        Assert.That(_mascotaService.AnotarPeso(Guid.NewGuid(), 12.5m), Is.False);
    }

    [Test]
    public void RetirarMascota_DevuelveFalseCuandoElIdNoExiste()
    {
        var seRetiro = _mascotaService.RetirarMascota(Guid.NewGuid());

        Assert.Multiple(() =>
        {
            Assert.That(seRetiro, Is.False);
            Assert.That(_mascotaService.ContarMascotas(), Is.EqualTo(20));
        });
    }
}
