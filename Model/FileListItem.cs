namespace RunTrack.Model
{
    // Klasse, die ein Listenelement für Dateien repräsentiert
    public class FileListItem
    {
        // Pfad der Datei
        public string Pfad { get; set; }

        // Name der Datei
        public string Name { get; set; }

        // Gibt an, ob die Info sichtbar ist
        public bool InfoVisible { get; set; }

        // Tooltip-Text für die Datei
        public string Tooltip { get; set; }
    }

}
