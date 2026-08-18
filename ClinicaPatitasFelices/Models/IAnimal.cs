namespace ClinicaPatitasFelices.Interfaces;

public interface IAnimal
{
    Guid Id { get; set; }
    string Nombre { get; set; }
    string TipoDeAlimentacion { get; set; }
    string Especie { get; set; }
    string Raza { get; set; }

    void Reproducirse();
    void Comer();
    void Dormir();
}