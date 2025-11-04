namespace business.Observer
{
    // Clase que encapsula la informacion de un evento de la biblioteca
    public class LibraryEvent
    {
        // Tipo de evento que ocurrio
        public EventType Type { get; set; }
        
        // Mensaje descriptivo del evento
        public string Message { get; set; } = string.Empty;
        
        // Fecha y hora en que ocurrio el evento
        public DateTime Timestamp { get; set; }
        
        // Datos adicionales del evento (opcional)
        public Dictionary<string, object>? Data { get; set; }

        public LibraryEvent(EventType type, string message)
        {
            Type = type;
            Message = message;
            Timestamp = DateTime.Now;
        }
    }

    // Tipos de eventos que pueden ocurrir en la biblioteca
    public enum EventType
    {
        // Eventos de socios
        SocioRegistrado,
        SocioMultado,
        
        // Eventos de prestamos
        PrestamoSolicitado,
        LibroDevuelto,
        PrestamoVencido,
        
        // Eventos de libros
        LibroAgotado,
        LibroDisponibleNuevamente,
        
        // Eventos de multas
        MultaCreada,
        MultaPagada
    }
}
