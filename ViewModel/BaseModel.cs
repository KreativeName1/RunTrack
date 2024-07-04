using System;
using System.ComponentModel;

namespace Klimalauf
{
   public class BaseModel : INotifyPropertyChanged
   {
      // Declare the PropertyChanged event
      public event PropertyChangedEventHandler PropertyChanged;

      // OnPropertyChanged will raise the PropertyChanged event passing the
      // source property that is being updated.
      protected void OnPropertyChanged(String propertyName)
      {
         if (this.PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
