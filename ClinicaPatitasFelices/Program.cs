using ClinicaPatitasFelices.Repositories;
using ClinicaPatitasFelices.UI;

// Punto de composicion: aqui se arman las dependencias una sola vez y se pasan
// hacia abajo. Cuando entre un contenedor de inyeccion de dependencias, estas
// lineas se reemplazan por los registros del contenedor.
IMascotaRepository mascotaRepository = new MascotaRepository();
IClienteRepository clienteRepository = new ClienteRepository(mascotaRepository);

var managerMascota = new ManagerMascota(mascotaRepository);

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
