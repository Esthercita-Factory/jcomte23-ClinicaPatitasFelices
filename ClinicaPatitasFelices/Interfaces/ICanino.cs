namespace ClinicaPatitasFelices.Interfaces;

public interface ICanino : IAnimal
{
    bool EstaAdiestrado { get; set; }
    int NivelDeOlfato { get; set; }

    void Ladrar();
    void MoverLaCola();
    void Rastrear(string olor);
}
