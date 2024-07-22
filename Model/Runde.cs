namespace Klimalauf
{
   public class Runde
   {
      public int Id { get; set; }
      public DateTime Zeitstempel { get; set; }
      public Schueler? Schueler { get; set; }
      public int? SchuelerId { get; set; }
      public string? BenutzerName { get; set; }

      public Runde()
      {
      }
   }
}
