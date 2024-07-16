using System.Windows;

namespace Klimalauf.View
{
   /// <summary>
   /// Interaktionslogik für AdminEinstellungen.xaml
   /// </summary>
   public partial class AdminEinstellungen : Window
   {
      private string firstName;
      private string lastName;
      private string pwd;

      public AdminEinstellungen()
      {
         InitializeComponent();
      }

      public AdminEinstellungen(string firstName, string lastName, string pwd)
      {
         this.firstName = firstName;
         this.lastName = lastName;
         this.pwd = pwd;
      }
   }
}
