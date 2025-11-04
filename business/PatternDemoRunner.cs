namespace business
{
    /// <summary>
    /// PROGRAMA DE DEMOSTRACION DE PATRONES DE DISEÑO
    /// 
    /// Este programa demuestra todos los patrones implementados en la carpeta business:
    /// 1. Repository Pattern
    /// 2. Service Layer Pattern
    /// 3. Factory Pattern
    /// 4. Strategy Pattern
    /// 5. Unit of Work Pattern
    /// 6. Singleton Pattern
    /// 7. Facade Pattern
    /// 8. Observer Pattern (NUEVO)
    /// 
    /// NOTA: Este codigo es SOLO PARA DEMOSTRACION EDUCATIVA
    /// No esta integrado con el sistema principal
    /// </summary>
    public class PatternDemoRunner
    {
        public static async Task Main(string[] args)
        {
            Console.Clear();
            MostrarBanner();

            bool continuar = true;
            
            while (continuar)
            {
                MostrarMenu();
                var opcion = Console.ReadLine();

                Console.Clear();

                switch (opcion)
                {
                    case "1":
                        MostrarResumenPatrones();
                        break;

                    case "2":
                        MostrarBeneficiosPatrones();
                        break;

                    case "3":
                        MostrarEjemploObserver();
                        break;

                    case "0":
                        continuar = false;
                        Console.WriteLine("\n¡Hasta luego!\n");
                        break;

                    default:
                        Console.WriteLine("\nOpcion no valida. Intente nuevamente.\n");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                    MostrarBanner();
                }
            }
        }

        private static void MostrarBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("?????????????????????????????????????????????????????????????????");
            Console.WriteLine("?                                                               ?");
            Console.WriteLine("?        SISTEMA DE BIBLIOTECA - PATRONES DE DISEÑO            ?");
            Console.WriteLine("?               Ejemplos Educativos de Patrones                ?");
            Console.WriteLine("?                                                               ?");
            Console.WriteLine("?????????????????????????????????????????????????????????????????");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void MostrarMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("??? MENU DE DEMOSTRACIONES ???\n");
            Console.ResetColor();

            Console.WriteLine("  [1] Ver Resumen de Todos los Patrones");
            Console.WriteLine();

            Console.WriteLine("  [2] Ver Beneficios de los Patrones");
            Console.WriteLine();

            Console.WriteLine("  [3] Ver Ejemplo del Patron Observer");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  [0] Salir");
            Console.ResetColor();

            Console.WriteLine("\n??????????????????????????????????\n");
            Console.Write("Seleccione una opcion: ");
        }

        private static void MostrarResumenPatrones()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("         RESUMEN DE PATRONES IMPLEMENTADOS            ");
            Console.WriteLine("???????????????????????????????????????????????????????\n");
            Console.ResetColor();

            var patrones = new[]
            {
                ("1. REPOSITORY", "Abstraccion del acceso a datos", "Estructural"),
                ("2. SERVICE LAYER", "Encapsulacion de logica de negocio", "Estructural"),
                ("3. FACTORY", "Creacion de objetos complejos", "Creacional"),
                ("4. STRATEGY", "Intercambio de algoritmos en runtime", "Comportamiento"),
                ("5. UNIT OF WORK", "Coordinacion de transacciones", "Estructural"),
                ("6. SINGLETON", "Instancia unica global", "Creacional"),
                ("7. FACADE", "Simplificacion de interfaces complejas", "Estructural"),
                ("8. OBSERVER", "Notificacion de cambios de estado", "Comportamiento")
            };

            foreach (var (nombre, descripcion, tipo) in patrones)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  {nombre}");
                Console.ResetColor();
                Console.WriteLine($"    • Proposito: {descripcion}");
                Console.WriteLine($"    • Tipo: {tipo}");
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Todos estos patrones trabajan juntos para crear");
            Console.WriteLine("una arquitectura robusta, mantenible y extensible.");
            Console.ResetColor();
        }

        private static void MostrarBeneficiosPatrones()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("         BENEFICIOS DE USAR PATRONES DE DISEÑO        ");
            Console.WriteLine("???????????????????????????????????????????????????????\n");
            Console.ResetColor();

            var beneficios = new[]
            {
                ("? Separation of Concerns", "Cada clase tiene una responsabilidad unica"),
                ("? Testability", "Facil hacer pruebas unitarias con mocks"),
                ("? Maintainability", "Codigo organizado y facil de mantener"),
                ("? Scalability", "Facil agregar nuevas funcionalidades"),
                ("? Flexibility", "Facil cambiar implementaciones sin afectar el resto"),
                ("? Reusability", "Componentes reutilizables en diferentes contextos"),
                ("? Open/Closed Principle", "Abierto a extension, cerrado a modificacion"),
                ("? Dependency Inversion", "Dependencias hacia abstracciones, no implementaciones"),
                ("? Single Responsibility", "Una clase, una razon para cambiar"),
                ("? Code Quality", "Codigo mas limpio y profesional")
            };

            foreach (var (titulo, descripcion) in beneficios)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"  {titulo}");
                Console.ResetColor();
                Console.WriteLine($"\n    {descripcion}\n");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("Los patrones de diseño son soluciones probadas a");
            Console.WriteLine("problemas comunes en el desarrollo de software.");
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.ResetColor();
        }

        private static void MostrarEjemploObserver()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("???????????????????????????????????????????????????????");
            Console.WriteLine("         PATRON OBSERVER - EJEMPLO DE USO             ");
            Console.WriteLine("???????????????????????????????????????????????????????\n");
            Console.ResetColor();

            Console.WriteLine("El patron Observer permite que un objeto (Subject) notifique");
            Console.WriteLine("automaticamente a otros objetos (Observers) cuando cambia su estado.\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("EJEMPLO DE CODIGO:\n");
            Console.ResetColor();

            Console.WriteLine("// 1. Crear el gestor de eventos");
            Console.WriteLine("var eventManager = new LibraryEventManager();");
            Console.WriteLine();

            Console.WriteLine("// 2. Registrar observadores");
            Console.WriteLine("eventManager.Attach(new EmailNotificationObserver());");
            Console.WriteLine("eventManager.Attach(new LoggingObserver());");
            Console.WriteLine("eventManager.Attach(new StatisticsObserver());");
            Console.WriteLine();

            Console.WriteLine("// 3. Notificar eventos");
            Console.WriteLine("eventManager.NotificarSocioRegistrado(\"NS000123\", \"Juan Perez\");");
            Console.WriteLine("eventManager.NotificarPrestamoSolicitado(\"NS000123\", \"El Quijote\", fecha);");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("VENTAJAS:");
            Console.ResetColor();
            Console.WriteLine("? Desacopla emisores de eventos de receptores");
            Console.WriteLine("? Facil agregar nuevos observadores sin modificar codigo existente");
            Console.WriteLine("? Permite notificaciones simultaneas a multiples sistemas");
            Console.WriteLine("? Ideal para logging, notificaciones, estadisticas, etc.");
        }
    }
}
