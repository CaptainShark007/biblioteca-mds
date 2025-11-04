# Carpeta Business - Patrones de Diseno Implementados

Esta carpeta contiene implementaciones de EJEMPLO de patrones de diseno comunes.
NOTA: Estas implementaciones NO SON FUNCIONALES, son solo ejemplos educativos.

## Estructura de Carpetas

```
business/
??? Repositories/          Patron Repository
??? Services/              Patron Service Layer
??? Factories/             Patron Factory
??? Validations/           Patron Strategy (validaciones)
??? UnitOfWork/            Patron Unit of Work
??? Singleton/             Patron Singleton
??? Facade/                Patron Facade
??? Observer/              Patron Observer
```

## Patrones Implementados

### 1. Repository Pattern
// ...existing code...

### 8. Observer Pattern
Archivos: Observer/ILibraryObserver.cs, ILibrarySubject.cs, LibraryEventManager.cs, LibraryEvent.cs, EmailNotificationObserver.cs, LoggingObserver.cs, StatisticsObserver.cs

Proposito:
- Define una dependencia uno-a-muchos entre objetos
- Cuando un objeto cambia de estado, todos sus dependientes son notificados
- Permite desacoplar emisores de eventos de sus receptores
- Ideal para sistemas de notificaciones y auditoria

Componentes:
- **Subject (LibraryEventManager)**: Mantiene la lista de observadores y los notifica
- **Observer (ILibraryObserver)**: Interfaz que define el metodo Update
- **Concrete Observers**: EmailNotificationObserver, LoggingObserver, StatisticsObserver

Uso:
```csharp
// Configurar el gestor de eventos (singleton)
var eventManager = new LibraryEventManager();

// Registrar observadores
eventManager.Attach(new EmailNotificationObserver());
eventManager.Attach(new LoggingObserver());
eventManager.Attach(new StatisticsObserver());

// Notificar eventos
eventManager.NotificarSocioRegistrado("NS000123", "Juan Perez");
eventManager.NotificarPrestamoSolicitado("NS000123", "El Quijote", fechaDevolucion);
eventManager.NotificarMultaCreada("NS000123", 3, "Devolucion tardia");
```

## Beneficios de estos Patrones

1. Separation of Concerns - Cada clase tiene una responsabilidad unica
2. Testability - Facil hacer pruebas unitarias con mocks
3. Maintainability - Codigo organizado y facil de mantener
4. Scalability - Facil agregar nuevas funcionalidades
5. Flexibility - Facil cambiar implementaciones sin afectar el resto
6. Extensibility - Nuevas funcionalidades sin modificar codigo existente (Open/Closed Principle)

## Como Usar en un Proyecto Real

Para usar estos patrones en un proyecto funcional:

1. Implementar repositorios con Entity Framework Core
2. Inyectar dependencias en Program.cs con builder.Services.AddScoped
3. Usar interfaces para facilitar testing
4. Implementar transacciones reales con DbContext
5. Agregar manejo de errores apropiado

Ejemplo de registro de servicios:
```csharp
// Patrones existentes
builder.Services.AddScoped<ISocioRepository, SocioRepository>();
builder.Services.AddScoped<ISocioService, SocioService>();
builder.Services.AddSingleton<INumeroSocioFactory, NumeroSocioFactory>();
builder.Services.AddScoped<IValidationStrategy<Socio>, SocioValidationStrategy>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILibraryFacade, LibraryFacade>();

// Patron Observer (Singleton para mantener observadores entre requests)
builder.Services.AddSingleton<LibraryEventManager>();
```

## Relacion entre Patrones

Los patrones trabajan en conjunto para crear una arquitectura robusta:

```
???????????????????????????????????????????????????????????
?                    FACADE                               ?
?  (Simplifica operaciones complejas)                     ?
?                                                         ?
?  ??????????????  ????????????????  ????????????       ?
?  ? SERVICES   ?  ? UNIT OF WORK ?  ? OBSERVER ?       ?
?  ? (Business) ?  ? (Transactions)?  ? (Events) ?       ?
?  ??????????????  ????????????????  ????????????       ?
?        ?                ?                 ?             ?
?  ??????????????                     ?????????????      ?
?  ?REPOSITORIES?                     ? OBSERVERS ?      ?
?  ?  (Data)    ?                     ?  (Email,  ?      ?
?  ??????????????                     ?   Log,    ?      ?
?        ?                            ?   Stats)  ?      ?
?  ??????????????                    ?????????????      ?
?  ?  ENTITIES  ?                                        ?
?  ? + FACTORY  ?                                        ?
?  ? + STRATEGY ?                                        ?
?  ??????????????                                        ?
???????????????????????????????????????????????????????????
```

### Ejemplo de Flujo Completo

```
1. Socio solicita prestamo (Controller)
         ?
2. Facade coordina la operacion
         ?
3. Service valida con Strategy
         ?
4. Repository + UnitOfWork guardan datos
         ?
5. Observer notifica evento "PrestamoSolicitado"
         ?
6. Observers procesan el evento (Email, Log, Stats)
```
