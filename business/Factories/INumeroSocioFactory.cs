namespace business.Factories
{
    // Patron Factory - Interfaz para la fabrica de numeros de socio
    // Este patron encapsula la logica de creacion de objetos complejos
    // o la generacion de valores con reglas especificas
    public interface INumeroSocioFactory
    {
        // Genera un numero de socio a partir del ID
        string GenerarNumeroSocio(int idSocio);

        // Valida el formato de un numero de socio
        bool ValidarFormato(string numeroSocio);

        // Extrae el ID desde un numero de socio
        int? ExtraerIdDesdeNumero(string numeroSocio);
    }
}
