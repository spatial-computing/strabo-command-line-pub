using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing.Drawing2D;
using System.Drawing;
using Emgu.CV.Structure;

namespace Strabo.Test
{
    class SampleImageSVM
    {
        public void cropImage()
        {
            Bitmap SourceImage = new Bitmap(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\BinaryOutput.png");
            Bitmap Cropped;
            int counter=0;
            for(int i=0;i<54;i++)
            {
                for (int j=0;j<54;j++)
                {
                   
                    Point rectangleLocation=new Point(i*28,j*28);
                    Size rectangleSize=new Size(28,28);
                    Rectangle boundingBox=new Rectangle(rectangleLocation,rectangleSize);
                    System.Drawing.Imaging.PixelFormat format = new System.Drawing.Imaging.PixelFormat();
                    Cropped= SourceImage.Clone(boundingBox, format);
                    counter++;
                    Cropped.Save(@"C:\Users\nhonarva\Documents\MachineLearning\Test&TrainedData\"+counter+".png");
                }
            }
        }
    }
}
