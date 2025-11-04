using System.Text;
using trabajoMetodologiaDSistemas.Models;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public Dictionary<string, string> Errors { get; } = new();

    public void AddError(string field, string message)
    {
        if (!Errors.ContainsKey(field))
            Errors[field] = message;
    }

    public override string ToString()
    {
        if (IsValid) return "Validación OK";
        var sb = new StringBuilder();
        foreach (var kv in Errors)
            sb.AppendLine($"{kv.Key}: {kv.Value}");
        return sb.ToString();
    }
}

public static class LibroValidator
{
    // Regla de validación centralizada para creación/edición de `Libro`.
    // Se puede extender con más métodos específicos (p.ej. ValidateForCreate).
    public static ValidationResult Validate(Libro libro)
    {
        var result = new ValidationResult();

        if (libro == null)
        {
            result.AddError(nameof(libro), "El objeto libro no puede ser nulo.");
            return result;
        }

        // Título: requerido, sin solo espacios, longitud máxima 200
        if (string.IsNullOrWhiteSpace(libro.Titulo))
        {
            result.AddError(nameof(libro.Titulo), "El título es obligatorio.");
        }
        else if (libro.Titulo.Length > 200)
        {
            result.AddError(nameof(libro.Titulo), "El título no puede exceder 200 caracteres.");
        }

        // CantidadDisponible: debe ser 0 o mayor
        if (libro.CantidadDisponible < 0)
        {
            result.AddError(nameof(libro.CantidadDisponible), "La cantidad disponible no puede ser negativa.");
        }

        // Ejemplo adicional: título no puede ser sólo números (política arbitraria)
        if (!string.IsNullOrWhiteSpace(libro.Titulo))
        {
            bool allDigits = true;
            foreach (var ch in libro.Titulo)
            {
                if (!char.IsDigit(ch) && !char.IsWhiteSpace(ch))
                {
                    allDigits = false;
                    break;
                }
            }
            if (allDigits)
            {
                result.AddError(nameof(libro.Titulo), "El título no puede contener sólo números.");
            }
        }

        return result;
    }
}

public static class ExampleUsage
{
    // Método de ejemplo que demuestra cómo usar el validador sin frameworks.
    // Puede llamarse desde una prueba unitaria, desde un PageModel o desde
    // cualquier parte de la aplicación.
    public static void Run()
    {
        var libroValido = new Libro { Id = 1, Titulo = "El Principito", CantidadDisponible = 5 };
        var libroInvalido = new Libro { Id = 2, Titulo = "123456", CantidadDisponible = -1 };

        var r1 = LibroValidator.Validate(libroValido);
        Console.WriteLine("Libro válido: " + r1.IsValid);
        if (!r1.IsValid) Console.WriteLine(r1);

        var r2 = LibroValidator.Validate(libroInvalido);
        Console.WriteLine("Libro inválido: " + r2.IsValid);
        if (!r2.IsValid) Console.WriteLine(r2);
    }
}
