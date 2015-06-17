using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Strabo.Test
{
    class RemoveiNoise
    {
        public void remove()
        {
            StreamReader file = new StreamReader(@"C:\Users\nhonarva\Documents\AllCLSLDocumentations\boxfile.txt");
             string line;
             System.IO.StreamWriter file1 = new System.IO.StreamWriter(@"C:\Users\nhonarva\Documents\AllCLSLDocumentations\Editedboxfile.txt");
          // trainedData.Add()
            while ((line = file.ReadLine()) != null)
            {
                string[] split = line.Split(' ');
                if (split[0].Equals("i"))
                {
                 //   split[2]=(Convert.ToInt32(split[2])+ 9).ToString();
                   split[4] = (Convert.ToInt32(split[4]) - 7).ToString();
                    string newline = "";
                    for (int i = 0; i < split.Length;i++ )
                    {
                        if (i < split.Length - 1)
                            newline += split[i] + " ";
                        else
                            newline += split[i];
                    }
                        file1.WriteLine(newline);
                }
                else if (split[0].Equals("j"))
                {
                    //   split[2]=(Convert.ToInt32(split[2])+ 9).ToString();
                    split[4] = (Convert.ToInt32(split[4]) - 7).ToString();
                    string newline = "";
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (i < split.Length - 1)
                            newline += split[i] + " ";
                        else
                            newline += split[i];
                    }
                    file1.WriteLine(newline);
                }
              
                //else if (split[0].Equals("n"))
                //{
                // //   if ((Convert.ToInt32(split[3]) - Convert.ToInt32(split[1])) != 20)
                //    split[1] = ((Convert.ToInt32(split[1])) + 2).ToString();
                //    string newline = "";
                //    for (int i = 0; i < split.Length; i++)
                //    {
                //        if (i < split.Length - 1)
                //            newline += split[i] + " ";
                //        else
                //            newline += split[i];
                //    }
                //    file1.WriteLine(newline);
                //}
                //else if (split[0].Equals("m"))
                //{
                //    //   if ((Convert.ToInt32(split[3]) - Convert.ToInt32(split[1])) != 20)
                //    split[1] = ((Convert.ToInt32(split[1])) + 2).ToString();
                //    string newline = "";
                //    for (int i = 0; i < split.Length; i++)
                //    {
                //        if (i < split.Length - 1)
                //            newline += split[i] + " ";
                //        else
                //            newline += split[i];
                //    }
                //    file1.WriteLine(newline);
                //}
                //else if (split[0].Equals("r"))
                //{
                //    //   if ((Convert.ToInt32(split[3]) - Convert.ToInt32(split[1])) != 20)
                //    split[1] = ((Convert.ToInt32(split[1])) + 2).ToString();
                //    string newline = "";
                //    for (int i = 0; i < split.Length; i++)
                //    {
                //        if (i < split.Length - 1)
                //            newline += split[i] + " ";
                //        else
                //            newline += split[i];
                //    }
                //    file1.WriteLine(newline);
                //}
                //else if (split[0].Equals("u"))
                //{
                //    //   if ((Convert.ToInt32(split[3]) - Convert.ToInt32(split[1])) != 20)
                //    split[1] = ((Convert.ToInt32(split[1])) + 2).ToString();
                //    string newline = "";
                //    for (int i = 0; i < split.Length; i++)
                //    {
                //        if (i < split.Length - 1)
                //            newline += split[i] + " ";
                //        else
                //            newline += split[i];
                //    }
                //    file1.WriteLine(newline);
                //}
                else
                {
                    file1.WriteLine(line);
                }
            }

        }

    }
}
