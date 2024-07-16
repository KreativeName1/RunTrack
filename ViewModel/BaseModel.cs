using System.ComponentModel;

namespace Klimalauf
{
   public class BaseModel : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;
      protected void OnPropertyChanged(String propertyName)
      {
         if (this.PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
