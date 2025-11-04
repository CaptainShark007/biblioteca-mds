using System;
using System.Collections.Concurrent;

namespace trabajoMetodologiaDSistemas.Utilities
{
    public sealed class LibraryConfigSingleton
    {
        private static readonly Lazy<LibraryConfigSingleton> _lazy =
            new(() => new LibraryConfigSingleton(), isThreadSafe: true);

        // Configuración
        private readonly ConcurrentDictionary<string, string> _settings = new();

        // Constructor privado evita instanciación fuera de la clase.
        private LibraryConfigSingleton() { }

        // Instancia global de acceso: LibraryConfigSingleton.Instance
        public static LibraryConfigSingleton Instance => _lazy.Value;

        // Operaciones expuestas por el Singleton
        public string? Get(string key) => _settings.TryGetValue(key, out var v) ? v : null;
        public void Set(string key, string value) => _settings[key] = value;
        public bool TryRemove(string key) => _settings.TryRemove(key, out _);
        public void Clear() => _settings.Clear();
    }
}