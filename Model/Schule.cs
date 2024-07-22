namespace Klimalauf
{
   public class Schule
   {
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;

        public virtual List<Klasse> Klassen { get; set; } = new List<Klasse>();


        public Schule()
      {
      }

      public override string ToString()
      {
         return Name;
      }
   }
}
