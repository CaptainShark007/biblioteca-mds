namespace business.Observer
{
    // Patron Observer - Observador concreto para estadisticas
    // NOTA: Esta es una implementacion de EJEMPLO
    // Mantiene contadores de eventos para generar reportes y estadisticas
    public class StatisticsObserver : ILibraryObserver
    {
        public string Name => "StatisticsCollector";

        // Contadores de eventos
        private readonly Dictionary<EventType, int> _eventCounts = new Dictionary<EventType, int>();
        private readonly object _lock = new object();

        public void Update(LibraryEvent evento)
        {
            IncrementarContador(evento.Type);
            
            // Acciones especificas segun el tipo de evento
            switch (evento.Type)
            {
                case EventType.PrestamoSolicitado:
                    RegistrarPrestamo(evento);
                    break;
                    
                case EventType.MultaCreada:
                    RegistrarMulta(evento);
                    break;
                    
                case EventType.LibroAgotado:
                    RegistrarLibroPopular(evento);
                    break;
            }
        }

        private void IncrementarContador(EventType tipo)
        {
            lock (_lock)
            {
                if (!_eventCounts.ContainsKey(tipo))
                {
                    _eventCounts[tipo] = 0;
                }
                
                _eventCounts[tipo]++;
                
                Console.WriteLine($"[Statistics] Total eventos de tipo '{tipo}': {_eventCounts[tipo]}");
            }
        }

        private void RegistrarPrestamo(LibraryEvent evento)
        {
            var libro = evento.Data?["Libro"]?.ToString() ?? "N/A";
            Console.WriteLine($"[Statistics] Nuevo prestamo registrado para: {libro}");
            // En un sistema real, actualizar estadisticas de libros mas prestados
        }

        private void RegistrarMulta(LibraryEvent evento)
        {
            var diasRestringido = evento.Data?["DiasRestringido"];
            Console.WriteLine($"[Statistics] Nueva multa registrada ({diasRestringido} dias)");
            // En un sistema real, calcular metricas de cumplimiento
        }

        private void RegistrarLibroPopular(LibraryEvent evento)
        {
            var libro = evento.Data?["Libro"]?.ToString() ?? "N/A";
            Console.WriteLine($"[Statistics] Libro popular detectado (agotado): {libro}");
            // En un sistema real, agregar a lista de libros para comprar mas copias
        }

        // Metodo para obtener estadisticas
        public Dictionary<EventType, int> ObtenerEstadisticas()
        {
            lock (_lock)
            {
                return new Dictionary<EventType, int>(_eventCounts);
            }
        }

        public void MostrarResumen()
        {
            Console.WriteLine("\n===== RESUMEN DE ESTADISTICAS =====");
            lock (_lock)
            {
                foreach (var kvp in _eventCounts.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value} eventos");
                }
            }
            Console.WriteLine("====================================\n");
        }
    }
}
