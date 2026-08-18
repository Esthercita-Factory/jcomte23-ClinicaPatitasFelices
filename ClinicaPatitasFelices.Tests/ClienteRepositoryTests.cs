using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

namespace ClinicaPatitasFelices.Tests;

public class ClienteRepositoryTests
{
    private IClienteRepository _clienteRepository = null!;

    // Cada prueba arranca con un repositorio nuevo: no hay estado compartido.
    [SetUp]
    public void Setup()
    {
        _clienteRepository = new ClienteRepository();
    }

    private Cliente RegistrarCliente(string documento = "9999")
    {
        var cliente = new Cliente(documento, "Prueba", "Apellido", "3000000000");

        _clienteRepository.Registrar(cliente);

        return cliente;
    }

    [Test]
    public void RegistrarCliente_AgregaYPermiteBuscarPorDocumento()
    {
        var cliente = RegistrarCliente("1122334455");

        Assert.That(_clienteRepository.ObtenerPorDocumento("1122334455"), Is.SameAs(cliente));
    }

    [Test]
    public void RegistrarCliente_RechazaDocumentoDuplicado()
    {
        RegistrarCliente("1122334455");

        var seRegistro = _clienteRepository.Registrar(
            new Cliente("1122334455", "Otro", "Cliente", "3009999999"));

        Assert.Multiple(() =>
        {
            Assert.That(seRegistro, Is.False);
            Assert.That(_clienteRepository.ObtenerTodos().Count(c => c.Documento == "1122334455"), Is.EqualTo(1));
        });
    }

    [Test]
    public void ListClientes_DevuelveUnaCopiaQueNoAfectaAlRepositorio()
    {
        var conteoOriginal = _clienteRepository.Contar();

        _clienteRepository.ObtenerTodos().Clear();

        Assert.That(_clienteRepository.Contar(), Is.EqualTo(conteoOriginal));
    }

    [Test]
    public void EliminarCliente_DesvinculaSusMascotas()
    {
        var cliente = RegistrarCliente();
        var mascota = new Mascota("Tomas", Especie.Gato, "Criollo", DateOnly.FromDateTime(DateTime.Today).AddMonths(-10));
        cliente.AgregarMascota(mascota);

        var seElimino = _clienteRepository.Eliminar(cliente.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seElimino, Is.True);
            Assert.That(_clienteRepository.ObtenerPorId(cliente.Id), Is.Null);
            Assert.That(mascota.Dueno, Is.Null);
        });
    }

    [Test]
    public void ActualizarCliente_CambiaDatosPersonalesYDeContacto()
    {
        var cliente = RegistrarCliente();

        var seActualizo = _clienteRepository.Actualizar(
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
            _clienteRepository.Actualizar(Guid.NewGuid(), "A", "B", "300", null, null),
            Is.False);
    }

    [Test]
    public void BuscarPorNombre_EncuentraPorCoincidenciaParcialSinImportarMayusculas()
    {
        var encontrados = _clienteRepository.FiltrarPorNombre("marcela");

        Assert.That(encontrados.Select(cliente => cliente.Documento), Does.Contain("52987654"));
    }

    [Test]
    public void BuscarPorNombre_DevuelveVacioSiElTextoEstaEnBlanco()
    {
        Assert.That(_clienteRepository.FiltrarPorNombre("   "), Is.Empty);
    }

    [Test]
    public void DosInstanciasDelRepositorioNoCompartenEstado()
    {
        var otroRepositorio = new ClienteRepository();

        _clienteRepository.Registrar(new Cliente("SOLO-EN-UNO", "Ana", "Diaz", "3001234567"));

        Assert.That(otroRepositorio.ObtenerPorDocumento("SOLO-EN-UNO"), Is.Null);
    }
}
