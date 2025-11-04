using trabajoMetodologiaDSistemas.Models;

namespace org.mds.parcial3.business.services
{
    public class LibraryFacade : ILibraryFacade
    {
        // Simulación de "almacenamiento" en memoria
        private readonly List<Socio> _socios = new();
        private readonly List<Libro> _libros = new();
        private readonly List<Prestamo> _prestamos = new();
        private readonly List<Multa> _multas = new();

        private int _nextSocioId = 1;
        private int _nextLibroId = 1;
        private int _nextPrestamoId = 1;
        private int _nextMultaId = 1;

        public LibraryFacade()
        {
            // Datos iniciales de ejemplo
            _libros.Add(new Libro { Id = _nextLibroId++, Titulo = "El Quijote", Autor = "Miguel de Cervantes", ISBN = 123456, CantidadDisponible = 3 });
            _libros.Add(new Libro { Id = _nextLibroId++, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", ISBN = 789012, CantidadDisponible = 2 });
        }

        /// <summary>
        /// Registra un socio asegurando DNI único y genera un NumeroSocio.
        /// </summary>
        public Task<(bool Success, string? Error, string? NumeroSocio)> RegisterMemberAsync(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.Dni))
                return Task.FromResult((false, "DNI inválido", (string?)null));

            if (_socios.Any(s => s.Dni == socio.Dni))
                return Task.FromResult((false, "DNI ya registrado", (string?)null));

            socio.IdSocio = _nextSocioId++;
            socio.NumeroSocio = $"NS{socio.IdSocio:D6}";
            _socios.Add(socio);

            return Task.FromResult((true, (string?)null, socio.NumeroSocio));
        }

        /// <summary>
        /// Solicita un préstamo: verifica socio, multas activas, disponibilidad del libro.
        /// </summary>
        public Task<(bool Success, string? Error, DateTime? DueDate)> RequestLoanAsync(string numeroSocio, string tituloLibro)
        {
            var socio = _socios.FirstOrDefault(s => s.NumeroSocio == numeroSocio);
            if (socio == null) return Task.FromResult((false, "Número de socio no válido", (DateTime?)null));

            // Limpiar multas vencidas
            var vencidas = _multas.Where(m => m.IdSocio == socio.IdSocio && m.Fecha.AddDays(m.DiasRestringido) <= DateTime.Now).ToList();
            foreach (var v in vencidas) _multas.Remove(v);

            var multaActiva = _multas.FirstOrDefault(m => m.IdSocio == socio.IdSocio && m.Fecha.AddDays(m.DiasRestringido) > DateTime.Now);
            if (multaActiva != null)
            {
                var fin = multaActiva.Fecha.AddDays(multaActiva.DiasRestringido);
                return Task.FromResult((false, $"Socio con multa activa hasta {fin:dd/MM/yyyy}", (DateTime?)null));
            }

            var libro = _libros.FirstOrDefault(l => string.Equals(l.Titulo, tituloLibro, StringComparison.OrdinalIgnoreCase));
            if (libro == null) return Task.FromResult((false, "Libro no encontrado", (DateTime?)null));
            if (libro.CantidadDisponible < 1) return Task.FromResult((false, "No hay ejemplares disponibles", (DateTime?)null));

            var ahora = DateTime.Now;
            var prestamo = new Prestamo
            {
                IdPrestamo = _nextPrestamoId++,
                IdSocio = socio.IdSocio,
                IdLibro = libro.Id,
                FechaPrestamo = ahora,
                FechaCorrespondienteDevolucion = ahora.AddDays(7)
            };
            _prestamos.Add(prestamo);
            libro.CantidadDisponible--;

            return Task.FromResult((true, (string?)null, (DateTime?)prestamo.FechaCorrespondienteDevolucion));
        }

        /// <summary>
        /// Devuelve los préstamos del socio.
        /// </summary>
        public Task<IEnumerable<Prestamo>> GetMemberLoansAsync(string numeroSocio)
        {
            var socio = _socios.FirstOrDefault(s => s.NumeroSocio == numeroSocio);
            if (socio == null) return Task.FromResult(Enumerable.Empty<Prestamo>());

            var loans = _prestamos.Where(p => p.IdSocio == socio.IdSocio).OrderByDescending(p => p.FechaPrestamo);
            return Task.FromResult((IEnumerable<Prestamo>)loans.ToList());
        }

        /// <summary>
        /// Marca la devolución y actualiza stock.
        /// </summary>
        public Task<(bool Success, string? Error)> ReturnBookAsync(int idPrestamo)
        {
            var prestamo = _prestamos.FirstOrDefault(p => p.IdPrestamo == idPrestamo);
            if (prestamo == null) return Task.FromResult((false, "Préstamo no encontrado"));

            if (prestamo.FechaDevolucion != null) return Task.FromResult((false, "Libro ya devuelto"));

            prestamo.FechaDevolucion = DateTime.Now;

            // Si hay retraso, crear multa
            if (prestamo.FechaDevolucion > prestamo.FechaCorrespondienteDevolucion)
            {
                var diasTarde = (prestamo.FechaDevolucion.Value.Date - prestamo.FechaCorrespondienteDevolucion.Date).Days;
                var multa = new Multa
                {
                    IdMulta = _nextMultaId++,
                    IdSocio = prestamo.IdSocio,
                    IdLibro = prestamo.IdLibro,
                    Fecha = DateTime.Now,
                    DiasRestringido = Math.Max(1, diasTarde),
                    Descripcion = $"Retraso de {diasTarde} días"
                };
                _multas.Add(multa);
            }

            var libro = _libros.FirstOrDefault(l => l.Id == prestamo.IdLibro);
            if (libro != null) libro.CantidadDisponible++;

            return Task.FromResult((true, (string?)null));
        }

        /// <summary>
        /// Devuelve multas activas para un socio.
        /// </summary>
        public Task<IEnumerable<Multa>> GetActiveFinesAsync(int idSocio)
        {
            var fines = _multas.Where(m => m.IdSocio == idSocio && m.Fecha.AddDays(m.DiasRestringido) > DateTime.Now);
            return Task.FromResult((IEnumerable<Multa>)fines.ToList());
        }
    }
}