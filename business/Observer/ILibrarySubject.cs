namespace business.Observer
{
    // Patron Observer - Interfaz del sujeto (Subject)
    // Define el contrato para objetos que pueden ser observados
    // Mantiene una lista de observadores y los notifica cuando hay cambios
    public interface ILibrarySubject
    {
        // Registra un nuevo observador
        void Attach(ILibraryObserver observer);
        
        // Elimina un observador
        void Detach(ILibraryObserver observer);
        
        // Notifica a todos los observadores sobre un evento
        void Notify(LibraryEvent evento);
    }
}
