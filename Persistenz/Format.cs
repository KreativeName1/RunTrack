namespace Klimalauf
{

   public enum SchriftTyp
   {
      Normal,
      Fett,
      Kursiv,
      FettKursiv
   }
   public enum Orientierung
   {
      Hochformat,
      Querformat
   }

   public class Format
   {
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;

      // Seitenränder
      public float SeitenRandOben { get; set; } = 20;
      public float SeitenRandUnten { get; set; } = 20;
      public float SeitenRandLinks { get; set; } = 20;
      public float SeitenRandRechts { get; set; } = 20;

      // Zellen
      public int ZellenAbstandHorizontal { get; set; } = 20;
      public int ZellenAbstandVertikal { get; set; } = 0;
      public int ZellenBreite { get; set; } = 150;
      public int ZellenHoehe { get; set; } = 50;

      // Schrift
      public float SchriftGroesse { get; set; } = 12;
      public SchriftTyp SchriftTyp { get; set; } = SchriftTyp.Fett;


      // Blattformat
      public BlattGroesse BlattGroesse { get; set; }

      public int BlattGroesseId { get; set; }

      public Orientierung Orientierung { get; set; } = Orientierung.Hochformat;

      // Weiteres
      public bool KopfAnzeigen { get; set; } = true;
      public bool Zentriert { get; set; } = true;
      public int SpaltenAnzahl { get; set; } = 3;


      public Format()
      {
      }

      public override string ToString()
      {
         return Name;
      }
   }
}
