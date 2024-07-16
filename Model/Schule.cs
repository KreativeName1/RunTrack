namespace Klimalauf
{
   public class Schule
   {
      public int Id { get; set; }
      public string Name { get; set; }

      public virtual List<Klasse> Klassen { get; set; }


      public Schule()
      {
      }

      public override string ToString()
      {
         return Name;
      }
   }
}
