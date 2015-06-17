using Strabo.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AForge;
using AForge.Imaging;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Strabo.Core.ImageProcessing;

namespace Strabo.Test
{
    public class TestHoughCircle
    {
        public TestHoughCircle() { }
        public void Apply()
        {
            string output_dir = @"C:\Users\yaoyichi\Desktop\";
            string inputPath = output_dir + "1.png";
            Log.SetLogDir(output_dir);
            Log.SetOutputDir(output_dir);

            HoughCircleTransformation circleTransform = new HoughCircleTransformation(35);
            // apply Hough circle transform
            Bitmap srcimg = new Bitmap(inputPath);
            AForge.Imaging.Filters.BradleyLocalThresholding bradley = new AForge.Imaging.Filters.BradleyLocalThresholding();
            Image<Gray, Byte> img = new Image<Gray, Byte>(inputPath);
            Bitmap dstimg = bradley.Apply(img.Bitmap);

            circleTransform.ProcessImage(dstimg);
            Bitmap houghCirlceImage = circleTransform.ToBitmap();
            // get circles using relative intensity
            HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(0.2);
            Graphics g = Graphics.FromImage(ImageUtils.AnyToFormat24bppRgb(srcimg));
            foreach (HoughCircle circle in circles)
            {
                //circle.RelativeIntensity
                g.DrawEllipse(new Pen(Color.Red), circle.X, circle.Y, circle.Radius, circle.Radius);
            }
            g.Dispose();
            srcimg.Save(output_dir + "1_.png");
        }
        public void ApplyEmguCV()
        {
            //
            // Summary:
            //     First apply Canny Edge Detector on the current image, then apply Hough transform
            //     to find circles
            //
            // Parameters:
            //   cannyThreshold:
            //     The higher threshold of the two passed to Canny edge detector (the lower
            //     one will be twice smaller).
            //
            //   accumulatorThreshold:
            //     Accumulator threshold at the center detection stage. The smaller it is, the
            //     more false circles may be detected. Circles, corresponding to the larger
            //     accumulator values, will be returned first
            //
            //   dp:
            //     Resolution of the accumulator used to detect centers of the circles. For
            //     example, if it is 1, the accumulator will have the same resolution as the
            //     input image, if it is 2 - accumulator will have twice smaller width and height,
            //     etc
            //
            //   minRadius:
            //     Minimal radius of the circles to search for
            //
            //   maxRadius:
            //     Maximal radius of the circles to search for
            //
            //   minDist:
            //     Minimum distance between centers of the detected circles. If the parameter
            //     is too small, multiple neighbor circles may be falsely detected in addition
            //     to a true one. If it is too large, some circles may be missed
            //
            // Returns:
            //     The circle detected for each of the channels
            string output_dir = @"C:\Users\yaoyichi\Desktop\";
            string inputPath = output_dir + "5.png";
            Image<Gray, Byte> Img_Source_Gray = new Image<Gray, Byte>(inputPath);
            Image<Bgr, byte> Img_Result_Bgr = new Image<Bgr, byte>(Img_Source_Gray.Width, Img_Source_Gray.Height);
            CvInvoke.cvCvtColor(Img_Source_Gray.Ptr, Img_Result_Bgr.Ptr, Emgu.CV.CvEnum.COLOR_CONVERSION.GRAY2BGR);
            
            Gray cannyThreshold = new Gray(12);
            Gray circleAccumulatorThreshold = new Gray(26);
            double Resolution = 1;
            double MinDistance = 10.0;
            int MinRadius = 10;
            int MaxRadius = 50;

            CircleF[] HoughCircles = Img_Source_Gray.Clone().HoughCircles(
                                    cannyThreshold,
                                    circleAccumulatorThreshold,
                                    Resolution, //Resolution of the accumulator used to detect centers of the circles
                                    MinDistance, //min distance 
                                    MinRadius, //min radius
                                    MaxRadius //max radius
                                    )[0]; //Get the circles from the first channel
            #region draw circles
            foreach (CircleF circle in HoughCircles)
                Img_Result_Bgr.Draw(circle, new Bgr(Color.Red), 2);
            #endregion

            Img_Result_Bgr.Save(output_dir + "1_.png");
        }
    }
}
