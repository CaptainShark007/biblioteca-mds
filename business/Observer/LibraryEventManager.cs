namespace business.Observer
{
    // Patron Observer - Implementacion del sujeto (Subject)
    // NOTA: Esta es una implementacion de EJEMPLO
    // Gestiona la lista de observadores y coordina las notificaciones
    public class LibraryEventManager : ILibrarySubject
    {
        // Lista de observadores suscritos
        private readonly List<ILibraryObserver> _observers = new List<ILibraryObserver>();
        
        // Lock para operaciones thread-safe
        private readonly object _lock = new object();

        public void Attach(ILibraryObserver observer)
        {
            lock (_lock)
            {
                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                    Console.WriteLine($"[EventManager] Observador '{observer.Name}' registrado");
                }
            }
        }

        public void Detach(ILibraryObserver observer)
        {
            lock (_lock)
            {
                if (_observers.Remove(observer))
                {
                    Console.WriteLine($"[EventManager] Observador '{observer.Name}' eliminado");
                }
            }
        }

        public void Notify(LibraryEvent evento)
        {
            List<ILibraryObserver> observersCopy;
            
            lock (_lock)
            {
                // Crea una copia para evitar problemas de concurrencia
                observersCopy = new List<ILibraryObserver>(_observers);
            }

            Console.WriteLine($"[EventManager] Notificando evento: {evento.Type} - {evento.Message}");
            
            // Notifica a todos los observadores
            foreach (var observer in observersCopy)
            {
                try
                {
                    observer.Update(evento);
                }
                catch (Exception ex)
                {
                    // En un sistema real, loguear el error
                    Console.WriteLine($"[EventManager] Error notificando a '{observer.Name}': {ex.Message}");
                }
            }
        }

        // Metodos auxiliares para crear y notificar eventos comunes
        
        public void NotificarSocioRegistrado(string numeroSocio, string nombre)
        {
            var evento = new LibraryEvent(
                EventType.SocioRegistrado,
                $"Nuevo socio registrado: {nombre} ({numeroSocio})"
            )
            {
                Data = new Dictionary<string, object>
                {
                    ["NumeroSocio"] = numeroSocio,
                    ["Nombre"] = nombre
                }
            };
            
            Notify(evento);
        }

        public void NotificarPrestamoSolicitado(string numeroSocio, string tituloLibro, DateTime fechaDevolucion)
        {
            var evento = new LibraryEvent(
                EventType.PrestamoSolicitado,
                $"Prestamo solicitado: {tituloLibro} por {numeroSocio}"
            )
            {
                Data = new Dictionary<string, object>
                {
                    ["NumeroSocio"] = numeroSocio,
                    ["Libro"] = tituloLibro,
                    ["FechaDevolucion"] = fechaDevolucion
                }
            };
            
            Notify(evento);
        }

        public void NotificarLibroDevuelto(string numeroSocio, string tituloLibro, bool conRetraso)
        {
            var evento = new LibraryEvent(
                EventType.LibroDevuelto,
                $"Libro devuelto: {tituloLibro} por {numeroSocio}" + (conRetraso ? " (CON RETRASO)" : "")
            )
            {
                Data = new Dictionary<string, object>
                {
                    ["NumeroSocio"] = numeroSocio,
                    ["Libro"] = tituloLibro,
                    ["ConRetraso"] = conRetraso
                }
            };
            
            Notify(evento);
        }

        public void NotificarMultaCreada(string numeroSocio, int diasRestringido, string motivo)
        {
            var evento = new LibraryEvent(
                EventType.MultaCreada,
                $"Multa creada para {numeroSocio}: {motivo} ({diasRestringido} dias)"
            )
            {
                Data = new Dictionary<string, object>
                {
                    ["NumeroSocio"] = numeroSocio,
                    ["DiasRestringido"] = diasRestringido,
                    ["Motivo"] = motivo
                }
            };
            
            Notify(evento);
        }

        public void NotificarLibroAgotado(string tituloLibro)
        {
            var evento = new LibraryEvent(
                EventType.LibroAgotado,
                $"Libro agotado: {tituloLibro}"
            )
            {
                Data = new Dictionary<string, object>
                {
                    ["Libro"] = tituloLibro
                }
            };
            
            Notify(evento);
        }
    }
}
