using ClinicaPatitasFelices.Data;
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
        _clienteRepository = new ClienteRepository(new AlmacenEnMemoria());
    }

    private Cliente RegistrarCliente(string documento = "9999")
    {
        var cliente = new Cliente(documento, "Prueba", "Apellido", "3000000000", "", "");

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
            new Cliente("1122334455", "Otro", "Cliente", "3009999999", "", ""));

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
    public void Eliminar_SacaAlClienteDelAlmacen()
    {
        var cliente = RegistrarCliente();

        var seElimino = _clienteRepository.Eliminar(cliente.Id);

        Assert.Multiple(() =>
        {
            Assert.That(seElimino, Is.True);
            Assert.That(_clienteRepository.ObtenerPorId(cliente.Id), Is.Null);
        });
    }

    [Test]
    public void Actualizar_GuardaLaEntidadModificada()
    {
        var cliente = RegistrarCliente();
        cliente.Nombre = "Javier";
        cliente.Apellido = "Combita";

        var seActualizo = _clienteRepository.Actualizar(cliente);

        Assert.Multiple(() =>
        {
            Assert.That(seActualizo, Is.True);
            Assert.That(_clienteRepository.ObtenerPorId(cliente.Id)!.NombreCompleto, Is.EqualTo("Javier Combita"));
        });
    }

    [Test]
    public void Actualizar_DevuelveFalseSiElClienteNoEstaRegistrado()
    {
        var ajeno = new Cliente("NO-REGISTRADO", "A", "B", "300", "", "");

        Assert.That(_clienteRepository.Actualizar(ajeno), Is.False);
    }

    [Test]
    public void FiltrarPorNombre_EncuentraPorCoincidenciaParcialSinImportarMayusculas()
    {
        _clienteRepository.Registrar(new Cliente("52987654", "Marcela", "Rojas", "3104445566", "", ""));

        var encontrados = _clienteRepository.FiltrarPorNombre("marcela");

        Assert.That(encontrados.Select(cliente => cliente.Documento), Does.Contain("52987654"));
    }

    [Test]
    public void FiltrarPorNombre_DevuelveVacioSiElTextoEstaEnBlanco()
    {
        Assert.That(_clienteRepository.FiltrarPorNombre("   "), Is.Empty);
    }

    [Test]
    public void DosInstanciasDelRepositorioNoCompartenEstado()
    {
        var otroRepositorio = new ClienteRepository(new AlmacenEnMemoria());

        _clienteRepository.Registrar(new Cliente("SOLO-EN-UNO", "Ana", "Diaz", "3001234567", "", ""));

        Assert.That(otroRepositorio.ObtenerPorDocumento("SOLO-EN-UNO"), Is.Null);
    }
}
