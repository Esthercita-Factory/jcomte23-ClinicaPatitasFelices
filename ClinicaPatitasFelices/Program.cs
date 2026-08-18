using ClinicaPatitasFelices.Data;
using ClinicaPatitasFelices.Repositories;
using ClinicaPatitasFelices.Services;
using ClinicaPatitasFelices.UI;

// Punto de composicion: las dependencias se arman una sola vez y se pasan hacia
// abajo. Cuando entre un contenedor de inyeccion de dependencias, estas lineas
// se reemplazan por los registros del contenedor:
//   services.AddSingleton<AlmacenEnMemoria>();
//   services.AddSingleton<IMascotaRepository, MascotaRepository>();
//   services.AddScoped<IMascotaService, MascotaService>();

var almacen = new AlmacenEnMemoria();

IMascotaRepository mascotaRepository = new MascotaRepository(almacen);
IClienteRepository clienteRepository = new ClienteRepository(almacen);

IMascotaService mascotaService = new MascotaService(mascotaRepository);
IClienteService clienteService = new ClienteService(clienteRepository, mascotaRepository);

DatosDeEjemplo.Sembrar(clienteService, mascotaService);

var managerMascota = new ManagerMascota(mascotaService);

string opcion;
do
{
    ManagerUser.MostraMenu();

    opcion = Console.ReadLine() ?? string.Empty;

    switch (opcion)
    {
        case "1":
            managerMascota.CrearUnaMascota();
            break;
        case "2":
            managerMascota.MostrarTodasLasMascotas();
            break;
        case "0":
            Console.WriteLine("adios");
            break;
        default:
            Console.WriteLine("te equivocaste de opcion");
            break;
    }
} while (opcion != "0");
