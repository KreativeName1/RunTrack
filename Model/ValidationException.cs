namespace RunTrack
{
    // Definiert eine benutzerdefinierte Ausnahme (Exception) namens ValidationException
    public class ValidationException : Exception
    {
        // Konstruktor, der eine Nachricht als Parameter akzeptiert und diese an die Basisklasse Exception weitergibt
        public ValidationException(string message) : base(message)
        {
        }
    }
}
