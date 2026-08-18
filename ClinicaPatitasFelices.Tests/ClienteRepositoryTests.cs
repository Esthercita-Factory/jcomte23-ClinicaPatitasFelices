using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.Tests;

public class ClienteRepositoryTests
{
    private IMascotaRepository _mascotaRepository = null!;
    private IClienteRepository _clienteRepository = null!;

    // Cada prueba arranca con repositorios nuevos: no hay estado compartido
    // entre pruebas ni necesidad de limpiar despues.
    [SetUp]
    public void Setup()
    {
        _mascotaRepository = new MascotaRepository();
        _clienteRepository = new ClienteRepository(_mascotaRepository);
    }

    private Cliente RegistrarCliente(string documento = "9999")
    {
        var cliente = new Cliente(documento, "Prueba", "Apellido", "3000000000");

        _clienteRepository.RegistrarCliente(cliente);

        return cliente;
    }

    private Mascota RegistrarMascota(string nombre = "Tomas")
    {
        var mascota = new Mascota(
            nombre, Especie.Gato, "Criollo", DateOnly.FromDateTime(DateTime.Today).AddMonths(-10));

        _mascotaRepository.RegistrarMascota(mascota);

        return mascota;
    }

    [Test]
    public void RegistrarCliente_AgregaYPermiteBuscarPorDocumento()
    {
        var cliente = RegistrarCliente("1122334455");

        Assert.That(_clienteRepository.BuscarPorDocumento("1122334455"), Is.SameAs(cliente));
    }

    [Test]
    public void RegistrarCliente_RechazaDocumentoDuplicado()
    {
        RegistrarCliente("1122334455");
        var duplicado = new Cliente("1122334455", "Otro", "Cliente", "3009999999");

        var seRegistro = _clienteRepository.RegistrarCliente(duplicado);

        Assert.Multiple(() =>
        {
            Assert.That(seRegistro, Is.False);
            Assert.That(_clienteRepository.ListClientes().Count(c => c.Documento == "1122334455"), Is.EqualTo(1));
        });
    }

    [Test]
    public void ListClientes_DevuelveUnaCopiaQueNoAfectaAlRepositorio()
    {
        var conteoOriginal = _clienteRepository.ContarClientes();

        _clienteRepository.ListClientes().Clear();

        Assert.That(_clienteRepository.ContarClientes(), Is.EqualTo(conteoOriginal));
    }

    [Test]
    public void AsignarMascota_VinculaClienteYMascotaExistentes()
    {
        var cliente = RegistrarCliente();
        var mascota = RegistrarMascota();

        var seAsigno = _clienteRepository.AsignarMascota(cliente.Id, mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seAsigno, Is.True);
            Assert.That(mascota.Dueno, Is.SameAs(cliente));
            Assert.That(_clienteRepository.ListarMascotasDe(cliente.Id), Has.Count.EqualTo(1));
            Assert.That(_clienteRepository.BuscarDuenoDeMascota(mascota.Id), Is.SameAs(cliente));
        });
    }

    [Test]
    public void AsignarMascota_TransfiereLaMascotaEntreClientes()
    {
        var duenoAnterior = RegistrarCliente("111");
        var duenoNuevo = RegistrarCliente("222");
        var mascota = RegistrarMascota();
        _clienteRepository.AsignarMascota(duenoAnterior.Id, mascota.Id);

        _clienteRepository.AsignarMascota(duenoNuevo.Id, mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(_clienteRepository.ListarMascotasDe(duenoAnterior.Id), Is.Empty);
            Assert.That(_clienteRepository.ListarMascotasDe(duenoNuevo.Id), Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void AsignarMascota_DevuelveFalseSiElClienteNoExiste()
    {
        var mascota = RegistrarMascota();

        Assert.That(_clienteRepository.AsignarMascota(Guid.NewGuid(), mascota.Id), Is.False);
    }

    [Test]
    public void DesasignarMascota_DejaLaMascotaSinDueno()
    {
        var cliente = RegistrarCliente();
        var mascota = RegistrarMascota();
        _clienteRepository.AsignarMascota(cliente.Id, mascota.Id);

        var seDesasigno = _clienteRepository.DesasignarMascota(cliente.Id, mascota.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seDesasigno, Is.True);
            Assert.That(mascota.Dueno, Is.Null);
            Assert.That(_clienteRepository.ListarMascotasDe(cliente.Id), Is.Empty);
        });
    }

    [Test]
    public void EliminarCliente_DejaSusMascotasSinDuenoPeroRegistradas()
    {
        var cliente = RegistrarCliente();
        var mascota = RegistrarMascota();
        _clienteRepository.AsignarMascota(cliente.Id, mascota.Id);

        var seElimino = _clienteRepository.EliminarCliente(cliente.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seElimino, Is.True);
            Assert.That(_clienteRepository.BuscarPorId(cliente.Id), Is.Null);
            Assert.That(_mascotaRepository.BuscarPorId(mascota.Id), Is.Not.Null);
            Assert.That(mascota.Dueno, Is.Null);
        });
    }

    [Test]
    public void ActualizarCliente_CambiaDatosPersonalesYDeContacto()
    {
        var cliente = RegistrarCliente();

        var seActualizo = _clienteRepository.ActualizarCliente(
            cliente.Id, "Javier", "Combita", "3151234567", "nuevo@correo.com", null);

        Assert.Multiple(() =>
        {
            Assert.That(seActualizo, Is.True);
            Assert.That(cliente.NombreCompleto, Is.EqualTo("Javier Combita"));
            Assert.That(cliente.Telefono, Is.EqualTo("3151234567"));
            Assert.That(cliente.Direccion, Is.Null);
        });
    }

    [Test]
    public void ActualizarCliente_DevuelveFalseSiNoExiste()
    {
        Assert.That(
            _clienteRepository.ActualizarCliente(Guid.NewGuid(), "A", "B", "300", null, null),
            Is.False);
    }

    [Test]
    public void BuscarPorNombre_EncuentraPorCoincidenciaParcialSinImportarMayusculas()
    {
        var encontrados = _clienteRepository.BuscarPorNombre("marcela");

        Assert.That(encontrados.Select(cliente => cliente.Documento), Does.Contain("52987654"));
    }

    [Test]
    public void BuscarPorNombre_DevuelveVacioSiElTextoEstaEnBlanco()
    {
        Assert.That(_clienteRepository.BuscarPorNombre("   "), Is.Empty);
    }

    [Test]
    public void SeedInicial_DejaMascotasVinculadasASusDuenos()
    {
        var cliente = _clienteRepository.BuscarPorDocumento("52987654");

        Assert.That(cliente, Is.Not.Null);
        Assert.That(cliente!.CantidadDeMascotas, Is.EqualTo(3));
    }

    [Test]
    public void DosInstanciasDelRepositorioNoCompartenEstado()
    {
        var otroMascotaRepository = new MascotaRepository();
        var otroClienteRepository = new ClienteRepository(otroMascotaRepository);

        _clienteRepository.RegistrarCliente(new Cliente("SOLO-EN-UNO", "Ana", "Diaz", "3001234567"));

        Assert.That(otroClienteRepository.BuscarPorDocumento("SOLO-EN-UNO"), Is.Null);
    }
}
