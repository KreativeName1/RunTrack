using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RunTrack
{
    // Basisklasse, die INotifyPropertyChanged implementiert
    public class BaseModel : INotifyPropertyChanged
    {
        // Ereignis, das ausgelöst wird, wenn sich eine Eigenschaft ändert
        public event PropertyChangedEventHandler? PropertyChanged;

        // Methode zum Auslösen des PropertyChanged-Ereignisses
        protected void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Methode zum Setzen einer Eigenschaft und Auslösen des PropertyChanged-Ereignisses
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            // Überprüfen, ob der neue Wert dem aktuellen Wert entspricht
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            // Setzen des neuen Wertes
            storage = value;

            // Auslösen des PropertyChanged-Ereignisses
            OnPropertyChanged(propertyName ?? "");
            return true;
        }
    }
}
