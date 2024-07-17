namespace Klimalauf
{
   public class CSVReader
   {

      // example file:
      // Vorname;Nachname;Klasse
      // Sascha;Dierl;10A
      // Paul;Fischer;10B
      public static List<object> ReadToList(string path)
      {
         // return a list of objects. the first row of the csv file is used to determine the name of the properties

         // the following rows are used to create objects with the propertiy names of the first row
         // the objects are then added to the list

         // the list is then returned

         List<object> list = new List<object>();

         List<string> lines = System.IO.File.ReadAllLines(path).ToList();

         string[] first = lines[0].Split(';');
         lines.RemoveAt(0);

         foreach (string line in lines)
         {
            string[] parts = line.Split(';');
            object obj = new object();
            for (int i = 0; i < first.Length; i++)
            {
               obj.GetType().GetProperty(first[i]).SetValue(obj, parts[i]);
            }
            list.Add(obj);
         }

         return list;
      }
   }
}
