using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Util;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Strabo.Core.Utility;
using System.Drawing;

namespace Strabo.Core.SymbolRecognition
{
    public class Descriptor
    {

        public List<VectorOfKeyPoint> SURF_BruteForceMatcher(Image<Gray, byte> model, Image<Gray, byte> observed, int hessianThreshould, out SURFDetector surfCPU)
        {
             surfCPU = new SURFDetector(hessianThreshould, false);
            List<VectorOfKeyPoint> KeyPointsList = new List<VectorOfKeyPoint>();
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            try
            {

              
                modelKeyPoints = surfCPU.DetectKeyPointsRaw(model, null); // Extract features from the object image
                observedKeyPoints = surfCPU.DetectKeyPointsRaw(observed, null); // Extract features from the observed image
              
                if (modelKeyPoints.Size <= 0)
                    throw new System.ArgumentException("Can't find any keypoints in your model image!");

                KeyPointsList.Add(modelKeyPoints);
                KeyPointsList.Add(observedKeyPoints);

            }
            catch (Exception e)
            {
                Log.WriteLine("SURF_BruteForceMatcher: " + e.Message);
                Console.WriteLine(e.Message);
                throw e;
            }
            return KeyPointsList;
        }

        public double[] GetImageVector(Image<Bgr, Byte> img, int threshould)
        {
            
            double[] pixels = new double[img.Height*img.Width];

            int gray = 0;
            for (int i = 0; i < img.Width; i++)
            {
                
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.Bitmap.GetPixel(i, j);
                    gray = (pixel.R + pixel.G + pixel.B) / 3;
                    if (gray > threshould)
                        pixels[i * img.Height + j] = 1;
                    //pixels[i * img.Height + j] = gray;
                }
            }

            return pixels;

        }
    }
}
