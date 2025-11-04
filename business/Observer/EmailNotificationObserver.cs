namespace business.Observer
{
    // Patron Observer - Observador concreto para notificaciones por email
    // NOTA: Esta es una implementacion de EJEMPLO
    // En un sistema real, integraria con un servicio de email como SendGrid, SMTP, etc.
    public class EmailNotificationObserver : ILibraryObserver
    {
        public string Name => "EmailNotifier";

        public void Update(LibraryEvent evento)
        {
            // Simula el envio de emails segun el tipo de evento
            switch (evento.Type)
            {
                case EventType.SocioRegistrado:
                    EnviarEmailBienvenida(evento);
                    break;
                    
                case EventType.PrestamoSolicitado:
                    EnviarEmailConfirmacionPrestamo(evento);
                    break;
                    
                case EventType.LibroDevuelto:
                    EnviarEmailConfirmacionDevolucion(evento);
                    break;
                    
                case EventType.MultaCreada:
                    EnviarEmailNotificacionMulta(evento);
                    break;
                    
                case EventType.PrestamoVencido:
                    EnviarEmailRecordatorioDevolucion(evento);
                    break;
                    
                default:
                    // Otros eventos no requieren notificacion por email
                    break;
            }
        }

        private void EnviarEmailBienvenida(LibraryEvent evento)
        {
            var numeroSocio = evento.Data?["NumeroSocio"]?.ToString() ?? "N/A";
            var nombre = evento.Data?["Nombre"]?.ToString() ?? "N/A";
            
            Console.WriteLine($"[EmailNotifier] Enviando email de bienvenida a {nombre}");
            Console.WriteLine($"  To: socio_{numeroSocio}@biblioteca.com");
            Console.WriteLine($"  Subject: Bienvenido a la Biblioteca");
            Console.WriteLine($"  Body: Hola {nombre}, tu numero de socio es: {numeroSocio}");
        }

        private void EnviarEmailConfirmacionPrestamo(LibraryEvent evento)
        {
            var numeroSocio = evento.Data?["NumeroSocio"]?.ToString() ?? "N/A";
            var libro = evento.Data?["Libro"]?.ToString() ?? "N/A";
            var fechaDevolucion = evento.Data?["FechaDevolucion"];
            
            Console.WriteLine($"[EmailNotifier] Enviando confirmacion de prestamo a {numeroSocio}");
            Console.WriteLine($"  Subject: Prestamo confirmado - {libro}");
            Console.WriteLine($"  Body: Tu prestamo fue registrado. Fecha de devolucion: {fechaDevolucion}");
        }

        private void EnviarEmailConfirmacionDevolucion(LibraryEvent evento)
        {
            var numeroSocio = evento.Data?["NumeroSocio"]?.ToString() ?? "N/A";
            var libro = evento.Data?["Libro"]?.ToString() ?? "N/A";
            
            Console.WriteLine($"[EmailNotifier] Enviando confirmacion de devolucion a {numeroSocio}");
            Console.WriteLine($"  Subject: Devolucion registrada - {libro}");
            Console.WriteLine($"  Body: Gracias por devolver el libro a tiempo");
        }

        private void EnviarEmailNotificacionMulta(LibraryEvent evento)
        {
            var numeroSocio = evento.Data?["NumeroSocio"]?.ToString() ?? "N/A";
            var diasRestringido = evento.Data?["DiasRestringido"];
            var motivo = evento.Data?["Motivo"]?.ToString() ?? "N/A";
            
            Console.WriteLine($"[EmailNotifier] Enviando notificacion de multa a {numeroSocio}");
            Console.WriteLine($"  Subject: Multa aplicada - {diasRestringido} dias");
            Console.WriteLine($"  Body: Se ha aplicado una multa por: {motivo}");
        }

        private void EnviarEmailRecordatorioDevolucion(LibraryEvent evento)
        {
            var numeroSocio = evento.Data?["NumeroSocio"]?.ToString() ?? "N/A";
            
            Console.WriteLine($"[EmailNotifier] Enviando recordatorio de devolucion a {numeroSocio}");
            Console.WriteLine($"  Subject: Recordatorio - Devolucion pendiente");
            Console.WriteLine($"  Body: Tienes un libro con fecha de devolucion vencida");
        }
    }
}
