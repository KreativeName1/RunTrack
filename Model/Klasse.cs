namespace Klimalauf
{
   public class Klasse
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public Schule Schule { get; set; }
      public int SchuleId { get; set; }

      public virtual List<Schueler> Schueler { get; set; }

      public RundenArt RundenArt { get; set; }
      public int RundenArtId { get; set; }

      public Klasse()
      {
      }

      public override string ToString()
      {
         return Name;
      }



   }
}
