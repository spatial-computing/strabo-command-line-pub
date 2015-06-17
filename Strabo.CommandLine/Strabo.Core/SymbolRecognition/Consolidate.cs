using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Strabo.Core.SymbolRecognition
{
 public static    class Consolidation
    {
     public static HashSet<float[]> Consolidate(ArrayList al, int w, int h, TextWriter log)
     {
         ArrayList al2 = new ArrayList();

         foreach (float[] i in al)
         {
             al2.Add(i);
         }

         foreach (float[] i in al)
         {
             foreach (float[] j in al)
             {
                 if (!((Math.Abs(i[0] - j[0]) > w) ||
                     (Math.Abs(i[1] - j[1]) > h) ||
                     (Math.Sqrt(Math.Pow(i[0] - j[0], 2) + Math.Pow(i[1] - j[1], 2)) > Math.Sqrt(Math.Pow(w, 2) + Math.Pow(h, 2)))))
                 {

                     if (i[2] > j[2])
                     {
                         al2[al.IndexOf(j)] = i;
                     }
                     else if (i[2] < j[2])
                     {
                         al2[al.IndexOf(i)] = j;
                     }
                 }
             }
         }

         HashSet<float[]> hash = new HashSet<float[]>();

         foreach (float[] i in al2)
         {
             hash.Add(i);
         }

         log.WriteLine("The count of al2: " + al2.Count);
         log.WriteLine("The count of hash: " + hash.Count);

         al.Clear();

         foreach (float[] i in hash)
         {
             al.Add(i);
         }

         log.WriteLine("The count after hash: " + al.Count);

         return hash;
     }
    }
}
