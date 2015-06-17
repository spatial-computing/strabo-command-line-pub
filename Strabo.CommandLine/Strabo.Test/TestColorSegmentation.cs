using Emgu.CV;
using Emgu.CV.Structure;
//using Emgu.CV.GPU;

using System;
using System.Drawing;
using AForge;
using AForge.Imaging;


using Strabo.Core.ColorSegmentation;
using Strabo.Core.Worker;

namespace Strabo.Test
{
    public class TestColorSegmentation
    {
        public static void test(String directory)
        {


            ColorSegmentationWorker cs = new ColorSegmentationWorker();
            //cs.Apply(@"C:\Users\yaoyichi\Desktop\", @"C:\Users\yaoyichi\Desktop\", "WMSImage_ms.png");

//            AForge.Imaging.Filters.OtsuThreshold otsu = new AForge.Imaging.Filters.OtsuThreshold();
            AForge.Imaging.Filters.BradleyLocalThresholding bradley = new AForge.Imaging.Filters.BradleyLocalThresholding();
            Image<Gray, Byte> img = new Image<Gray, Byte>(@"C:\Users\yaoyichi\Desktop\Images-22-10\576800_mc1024_k64.png");
            Bitmap dstimg =bradley.Apply(img.Bitmap);

            dstimg.Save(@"C:\Users\yaoyichi\Desktop\Images-22-10\576800_mc1024_k64-2.png");
          //  Image<Gray, Byte> img = new Image<Gray, Byte>(@"C:\Users\yaoyichi\Desktop\Images-22-10\576800_mc1024_k64.png");

           
            img = img.ThresholdAdaptive(new Gray(255),
              Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY,15,new Gray(0));

            img.Save(@"C:\Users\yaoyichi\Desktop\Images-22-10\576800_mc1024_k64-1.png");

        }
    }
}