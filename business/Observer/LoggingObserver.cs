using System.Text.Json;

namespace business.Observer
{
    // Patron Observer - Observador concreto para logging/auditoria
    // NOTA: Esta es una implementacion de EJEMPLO
    // En un sistema real, escribiria a archivos de log, base de datos, o servicios como Serilog
    public class LoggingObserver : ILibraryObserver
    {
        public string Name => "AuditLogger";

        private readonly string _logDirectory;

        public LoggingObserver(string logDirectory = "logs")
        {
            _logDirectory = logDirectory;
            // En un sistema real, crear el directorio si no existe
        }

        public void Update(LibraryEvent evento)
        {
            // Registra todos los eventos en el log
            var logEntry = CrearEntradaLog(evento);
            EscribirEnLog(logEntry);
        }

        private string CrearEntradaLog(LibraryEvent evento)
        {
            // Crea un objeto estructurado para el log
            var logData = new
            {
                Timestamp = evento.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                EventType = evento.Type.ToString(),
                Message = evento.Message,
                Data = evento.Data
            };

            // Serializa a JSON para tener logs estructurados
            return JsonSerializer.Serialize(logData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        private void EscribirEnLog(string logEntry)
        {
            // Simula la escritura en un archivo de log
            Console.WriteLine($"[AuditLogger] Registrando evento:");
            Console.WriteLine(logEntry);
            
            // En un sistema real:
            // var filename = Path.Combine(_logDirectory, $"audit_{DateTime.Now:yyyyMMdd}.log");
            // File.AppendAllText(filename, logEntry + Environment.NewLine);
        }
    }
}
