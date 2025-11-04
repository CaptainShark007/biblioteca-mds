namespace business.Factories
{
    // Patron Factory - Implementacion de la fabrica de numeros de socio
    // Centraliza la logica de generacion y validacion de numeros de socio
    public class NumeroSocioFactory : INumeroSocioFactory
    {
        // Constantes que definen el formato del numero de socio
        private const string PREFIJO = "NS";
        private const int LONGITUD_NUMERO = 6;

        public string GenerarNumeroSocio(int idSocio)
        {
            // Genera el numero con formato: NS000001, NS000002, etc.
            // Usa PadLeft para completar con ceros a la izquierda
            return $"{PREFIJO}{idSocio.ToString().PadLeft(LONGITUD_NUMERO, '0')}";
        }

        public bool ValidarFormato(string numeroSocio)
        {
            // Validacion 1: No puede ser nulo o vacio
            if (string.IsNullOrWhiteSpace(numeroSocio))
                return false;

            // Validacion 2: Debe comenzar con el prefijo correcto
            if (!numeroSocio.StartsWith(PREFIJO))
                return false;

            // Validacion 3: Debe tener la longitud correcta
            if (numeroSocio.Length != PREFIJO.Length + LONGITUD_NUMERO)
                return false;

            // Validacion 4: La parte numerica debe ser un numero valido
            var numeroParte = numeroSocio.Substring(PREFIJO.Length);
            return int.TryParse(numeroParte, out _);
        }

        public int? ExtraerIdDesdeNumero(string numeroSocio)
        {
            // Verifica que el formato sea valido antes de extraer
            if (!ValidarFormato(numeroSocio))
                return null;

            // Extrae la parte numerica y la convierte a int
            var numeroParte = numeroSocio.Substring(PREFIJO.Length);
            if (int.TryParse(numeroParte, out int id))
                return id;

            return null;
        }
    }
}
