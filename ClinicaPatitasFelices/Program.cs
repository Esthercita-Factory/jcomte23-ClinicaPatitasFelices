using ClinicaPatitasFelices.UI;


//

string opcion;
do
{
    ManagerUser.MostraMenu();

    opcion = Console.ReadLine() ?? string.Empty;

    switch (opcion)
    {
        case "1":
            ManagerMascota.CrearUnaMascota();
            break;
        case "2":
            ManagerMascota.MostrarTodasLasMascotas();
            break;
        case "0":
            Console.WriteLine("adios");
            break;
        default:
            Console.WriteLine("te equivocaste de opcion");
            break;
    }
}while(opcion != "0");





