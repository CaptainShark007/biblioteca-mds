namespace business.Observer
{
    // Patron Observer - Interfaz del observador
    // Define el contrato que deben cumplir todos los observadores
    // que quieran recibir notificaciones de eventos de la biblioteca
    public interface ILibraryObserver
    {
        // Metodo llamado cuando ocurre un evento en la biblioteca
        // El parametro evento contiene la informacion del evento
        void Update(LibraryEvent evento);
        
        // Nombre identificador del observador (para debugging)
        string Name { get; }
    }
}
