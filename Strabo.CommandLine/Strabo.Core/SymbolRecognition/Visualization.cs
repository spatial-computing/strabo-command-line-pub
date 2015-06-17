using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using System.Collections;

namespace Strabo.Core.SymbolRecognition
{
    public static class Visualization
    {
        public static void DrawingResults(HashSet<float[]> hash, Image<Gray, Byte> gElement, Image<Bgr, Byte> test, string inputPath)
        {
            TextWriter coordinatesOnMapBlue = File.AppendText(inputPath + "/coordinatesOnMapBlue.txt");
            foreach (float[] i in hash)
            {
                test.Draw(new Rectangle(new Point((int)i[0], (int)i[1]), gElement.Size), new Bgr(Color.Blue), 5);
                coordinatesOnMapBlue.WriteLine("x= " + (int)i[0] + ", y=" + (int)i[1] + "");
            }
            coordinatesOnMapBlue.Close();
            test.Save(string.Format("{0}{1}/out.jpg", inputPath, ""));
        }

        public static void DrawingResults(ArrayList hash, Image<Gray, Byte> gElement, Image<Bgr, Byte> test, string inputPath)
        {
            TextWriter coordinatesOnMapBlue = File.AppendText(inputPath + "/coordinatesOnMapBlue.txt");
            foreach (float[] i in hash)
            {
                test.Draw(new Rectangle(new Point((int)i[0], (int)i[1]), gElement.Size), new Bgr(Color.Blue), 5);
                coordinatesOnMapBlue.WriteLine("x= " + (int)i[0] + ", y=" + (int)i[1] + "");
            }
            coordinatesOnMapBlue.Close();
            test.Save(string.Format("{0}{1}/out.jpg", inputPath, ""));
        }
        public static void DrawResults(List<Point> points, Size size, Image<Bgr, Byte> test, string inputpath)
        {
            foreach (Point i in points)
                test.Draw(new Rectangle(i, size), new Bgr(Color.Blue), 5);
            test.Save(string.Format("{0}{1}/out.jpg", inputpath, ""));
        }
    }
}
