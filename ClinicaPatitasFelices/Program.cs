using ClinicaPatitasFelices.Models;
using ClinicaPatitasFelices.Repositories;

Console.WriteLine("Programa de mascostas");

Console.WriteLine("Menu");
Console.WriteLine("1. Registrar una nueva mascota");

string nombre=  Console.ReadLine();
string raza =  Console.ReadLine();
int edadEnMeses = int.Parse(Console.ReadLine());



var nuevaMascota2 = new Mascota("pablo","criollo",24);
var nuevaMascota3 = new Mascota("lupe", "criolla", 12);

var bodega = new MascotaRepository();

bodega.Mascotas.Add(nuevaMascota2);



