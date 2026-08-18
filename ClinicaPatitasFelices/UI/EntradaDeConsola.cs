namespace ClinicaPatitasFelices.UI;

/// <summary>
/// Lectura de consola tolerante a errores: en vez de reventar la aplicacion con una
/// excepcion, vuelve a preguntar hasta que el dato sea valido.
/// </summary>
public static class EntradaDeConsola
{
    public static string LeerTextoObligatorio(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            var valor = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }

            Console.WriteLine("  !! Este dato es obligatorio, intente de nuevo.");
        }
    }

    public static string? LeerTextoOpcional(string mensaje)
    {
        Console.Write(mensaje);
        var valor = Console.ReadLine();

        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    public static DateOnly LeerFechaDeNacimiento(string mensaje)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);

        while (true)
        {
            Console.Write(mensaje);

            if (!DateOnly.TryParse(Console.ReadLine(), out var fecha))
            {
                Console.WriteLine("  !! Fecha invalida. Use el formato dd/mm/aaaa.");
                continue;
            }

            if (fecha > hoy)
            {
                Console.WriteLine("  !! La fecha de nacimiento no puede estar en el futuro.");
                continue;
            }

            return fecha;
        }
    }

    public static TEnum LeerOpcionDeLista<TEnum>(string titulo) where TEnum : struct, Enum
    {
        var opciones = Enum.GetValues<TEnum>();

        Console.WriteLine(titulo);

        for (var indice = 0; indice < opciones.Length; indice++)
        {
            Console.WriteLine($"    [{indice + 1}] {opciones[indice]}");
        }

        while (true)
        {
            Console.Write("  >> Seleccione: ");

            if (int.TryParse(Console.ReadLine(), out var seleccion) &&
                seleccion >= 1 &&
                seleccion <= opciones.Length)
            {
                return opciones[seleccion - 1];
            }

            Console.WriteLine($"  !! Debe ingresar un numero entre 1 y {opciones.Length}.");
        }
    }
}
