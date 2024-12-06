namespace RunTrack
{
    // Definiert eine benutzerdefinierte Ausnahme namens ImportException
    internal class ImportException : Exception
    {
        // Standardkonstruktor für ImportException
        public ImportException() { }

        // Konstruktor, der eine Fehlermeldung akzeptiert und an die Basisklasse Exception weitergibt
        public ImportException(string message) : base(message) { }
    }
}
