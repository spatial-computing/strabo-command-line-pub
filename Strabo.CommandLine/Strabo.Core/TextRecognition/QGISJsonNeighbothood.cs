using System;

namespace Strabo.Core.TextRecognition
{
    class QGISJsonNeighbothood
    {
        private static string _streamPixel;
        public static string path;
        public static double Ex;
        public static double Ny;
        public static double scale;
        public static string filename;
        private static string _streamRef;

        public static void Start()
        {
            _streamRef = _streamPixel = "{\"type\":\"FeatureCollection\",\"features\":[";
        }

        public static void AddFeature(double x1, double y1,double x2, double y2, double x3 , double y3, double x4 , double y4, string imageID, string PixelToSizeRatio, int geo_Ref)
        {
            AddCordinationFeature(x1, y1, x3-x1, y3-y1, imageID);      //// correct this part

            _streamPixel += "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[" + x1.ToString() + "," + (geo_Ref * y1).ToString() + "],[" +
                (x2).ToString() + "," + (geo_Ref * y2).ToString() + "],[" + (x3).ToString() + "," + (geo_Ref * (y3)).ToString() + "],[" + x4.ToString() + "," + (geo_Ref * (y4)).ToString() + "],[" +
                x1.ToString() + "," + (geo_Ref * y1).ToString() + "]]]},\"ImageId\":\"" + imageID + "\",\"PixelToSizeRatio\":\"" + PixelToSizeRatio + "\",},";

        }
       
        private static void WriteGeojsonFile()
        {
            Console.WriteLine("Preparing GeojsonFile for QGIS");
            _streamPixel += "]}";
            _streamRef += "]}";

        }

        public static void WriteGeojsonFileByPixels()
        {
            WriteGeojsonFile();
            System.IO.File.WriteAllText(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\InputImages" + "\\" + filename + "ByPixelsNeighborhood.txt", _streamPixel);
        }
       
        private static void AddCordinationFeature(double x, double y, double h, double w, string imagID)
        {
            double refx, refy, refh, refw;
            refx = Ex + x * scale;
            refy = Ny - y * scale;
            refh = h * scale;
            refw = w * scale;

            _streamRef += "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[" + refx.ToString() + "," + (refy).ToString() + "],[" +
            (refx + refw).ToString() + "," + (refy).ToString() + "],[" + (refx + refw).ToString() + "," + ((refy - refh)).ToString() + "],[" + refx.ToString() + "," + ((refy - refh)).ToString() + "],[" +
            refx.ToString() + "," + (refy).ToString() + "]]]},\"ImageId\":\"" + imagID + "\",},";
        }
    }
}
