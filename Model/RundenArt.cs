namespace Klimalauf
{
   public class RundenArt
   {
      public int Id { get; set; }
      public String Name { get; set; }
      public int LaengeInMeter { get; set; }
      public int MaxScanIntervalInSekunden { get; set; } = 60;

      public override string ToString()
      {
         return Name;
      }
   }
}
