namespace Klimalauf
{
   public class Benutzer
   {
      public int Id { get; set; }
      public string Passwort { get; set; }
      public string Vorname { get; set; }
      public string Nachname { get; set; }
      public bool IsAdmin { get; set; } = false;

      public Benutzer()
      {
      }

      public string Benutzername
      {
         get
         {
            return $"{Vorname}, {Nachname}";
         }
      }
   }
}
