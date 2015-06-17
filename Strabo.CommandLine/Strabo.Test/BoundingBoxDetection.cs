using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Strabo.Test
{
    class BoundingBoxDetection
    {
        public void Rotation()
        {
            //Bitmap src = new Bitmap(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\136_p_4_559_1292_s_0_525_1280_72_24.png");
            //Rectangle rect = new Rectangle(0, 0, 100, 50);
            //Bitmap cropped = CropRotatedRect(src, rect, -30.5f, true);
            //cropped.Save(@"C:\Users\nhonarva\Documents\sample.png");


            List<PointF> points=new List<PointF>();

            PointF[] pts;
            //PointF[] pts=new PointF[4];
            //pts[0] = new PointF(40.5f, 60.5f);
            //pts[1] = new PointF(34f, 44f);
            //pts[2] = new PointF(39f, 70f);
            //pts[3] = new PointF(14f, 45f);


            Bitmap sourceImage = new Bitmap(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\138_p_6_1482_1009_s_0_1469_965_28_84.png");  /// this image belongs to map 1920-4 intermediate result 
            Image<Bgr, byte> source = new Image<Bgr, byte>(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\138_p_6_1482_1009_s_0_1469_965_28_84.png");
            Image<Bgr, byte> source1 = new Image<Bgr, byte>(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\SourceMapImage_ms.png");
            Bitmap sourceImage1 = new Bitmap(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\BinaryOutput.png");
            Image<Bgr, byte> img = new Image<Bgr, byte>(400, 400, new Bgr(Color.White));

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color color = sourceImage.GetPixel(i, j);
                    if (!sourceImage.GetPixel(i, j).Name.Equals("ffffffff"))
                    {
                        PointF point = new PointF(i , j );
                        points.Add(point);
                    }
                }
            }

            string filename = Path.GetFileNameWithoutExtension(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\138_p_6_1482_1009_s_0_1469_965_28_84.png");
            // string filename = Path.GetFileNameWithoutExtension(filePaths[i]);
            String[] splitTokens = filename.Split('_');
            int numberOfLetters = 5;
            //if (splitTokens[6].Equals("0"))
            //{
                int x = Convert.ToInt32(splitTokens[7]);
                int y = Convert.ToInt32(splitTokens[8]);
            //    int width=Convert.ToInt32(splitTokens[9]);
            //    int height=Convert.ToInt32(splitTokens[10]);
            //    int estimatedSizeOfOneLetter = width/numberOfLetters;
            //    double pixelToSizeRatio = (double)points.Count / (double)(width * height);

            //    Point left = new Point();
            //    int leftPixelCount = 0;
            //    double leftPixelToSizeRatio = 0;
            //    left.X = x - estimatedSizeOfOneLetter;
            //    left.Y = y;

            //    Point right = new Point();
            //    int rightPixelCount = 0;
            //    double rightPixelToSizeRatio = 0;
            //    right.X = x + width;
            //    right.Y = y;

            //    do
            //    {
                    
            //        for (int i = left.X; i < left.X+estimatedSizeOfOneLetter; i++)
            //        {
            //            for (int j = left.Y; j < left.Y+height; j++)
            //            {
            //                Color color = sourceImage1.GetPixel(i, j);
            //                if (!sourceImage1.GetPixel(i, j).Name.Equals("ffffffff"))
            //                {
            //                    leftPixelCount++;
            //                 //   sourceImage1.SetPixel(i, j, Color.Green);
            //                }
            //                sourceImage1.SetPixel(i, j, Color.Green);
            //            }
            //        }

            //        leftPixelToSizeRatio = (double)leftPixelCount / (double)(estimatedSizeOfOneLetter * height);
            //        left.X -= estimatedSizeOfOneLetter;
            //        leftPixelCount = 0;

            //    } while (leftPixelToSizeRatio > 0.1);


            //    do
            //    {
            //        for (int i = right.X; i < right.X + estimatedSizeOfOneLetter; i++)
            //        {
            //            for (int j = right.Y; j < right.Y + height; j++)
            //            {
            //                Color color = sourceImage1.GetPixel(i, j);
            //                if (!sourceImage1.GetPixel(i, j).Name.Equals("ffffffff"))
            //                {
            //                    rightPixelCount++;
            //                 //   sourceImage1.SetPixel(i, j, Color.Green);
            //                }
            //                sourceImage1.SetPixel(i, j, Color.Green);
            //            }
            //        }

            //        rightPixelToSizeRatio = (double)(rightPixelCount) / (double)(estimatedSizeOfOneLetter * height);
            //        right.X += estimatedSizeOfOneLetter;
            //        rightPixelCount = 0;

            //    } while (rightPixelToSizeRatio > 0.2);
      
            //}

          //  else
          //  {





                pts = points.ToArray();
                Emgu.CV.Structure.MCvBox2D box = PointCollection.MinAreaRect(pts);
                Rectangle boundingbox = PointCollection.BoundingRectangle(pts);


                img.Draw(box, new Bgr(Color.Red), 1);
                img.Draw(boundingbox, new Bgr(Color.Green), 2);
                source.Draw(box, new Bgr(Color.Red), 1);
                source.Draw(boundingbox, new Bgr(Color.Green), 2);

                Emgu.CV.Structure.MCvBox2D boxinoriginalImage = box;
                boxinoriginalImage.center.X += x - boundingbox.X;   /// 63=originalboundingbox.x(1069)(I used result of intermediate image)-boundingbox.x(21)
                boxinoriginalImage.center.Y += y - boundingbox.Y;      //// 175=originalboundingbox.x(1211)(I used result of intermediate image)-boundingbox.x(21)

                double angleInRadius = (((-1) * box.angle) * Math.PI) / 180;

              //  double height = (box.size.Height) / Math.Sin(angleInRadius);
              //  double width = (box.size.Width) / Math.Sin(angleInRadius);

                double addToX = (((box.size.Width) * 3) / 4) * Math.Cos(angleInRadius);  // I have multiplied height to 3/4 because we have four letter in recognized image and I want to find the center of added part for boundingbox
                double addToY = (((box.size.Width) * 3) / 4) * Math.Sin(angleInRadius);

                double OriginalpixelToSizeRatio = points.Count() / (box.size.Height * box.size.Width);

                Emgu.CV.Structure.MCvBox2D addedArea = box;

                addedArea.center.X += x - boundingbox.X + (float)addToX;
                addedArea.center.Y += y - boundingbox.Y - (float)addToY;

                addedArea.size.Width = (float)((box.size.Width) / 2);

                source1.Draw(boxinoriginalImage, new Bgr(Color.Green), 2);
                source1.Draw(addedArea, new Bgr(Color.Red), 2);





                //  PointF[] vertices = boxinoriginalImage.GetVertices();
                PointF[] AddedAreaVertices = addedArea.GetVertices();
                int AddedAreaPixelNumber = 0;

                double[] slope = new double[4];
                double[] b = new double[4];
                for (int i = 0; i < AddedAreaVertices.Length; i++)
                {
                    if (i < AddedAreaVertices.Length - 1)
                        slope[i] = (AddedAreaVertices[i].Y - AddedAreaVertices[i + 1].Y) / (AddedAreaVertices[i].X - AddedAreaVertices[i + 1].X);

                    else
                        slope[i] = (AddedAreaVertices[i].Y - AddedAreaVertices[0].Y) / (AddedAreaVertices[i].X - AddedAreaVertices[0].X);

                    b[i] = -(AddedAreaVertices[i].Y - slope[i] * AddedAreaVertices[i].X);
                }

                for (int i = 0; i < sourceImage1.Width; i++)
                {
                    for (int j = 0; j < sourceImage1.Height; j++)
                    {
                        Color color = sourceImage1.GetPixel(i, j);
                        double[] result = new double[4];
                        for (int k = 0; k < result.Length; k++)
                            result[k] = j - slope[k] * i + b[k];

                        if (result[0] < 0 && result[1] > 0 && result[2] > 0 && result[3] < 0)   /// explore if pixel is in bounding box of Min Area Rectangle
                        {
                            //  sourceImage1.SetPixel(i, j, Color.Green);
                            if (!sourceImage1.GetPixel(i, j).Name.Equals("ffffffff"))
                            {
                                AddedAreaPixelNumber++;
                            }
                              sourceImage1.SetPixel(i, j, Color.Green);
                        }
                    }
                }

                double addedAreaSize = addedArea.size.Height * addedArea.size.Width;
                double AddedAreaPixelToSizeRatio = (double)(AddedAreaPixelNumber) / (addedArea.size.Height * addedArea.size.Width);



                //   PointF[] vertices = boxinoriginalImage.GetVertices();
                ////   PointF[] otherVertices = addedArea.GetVertices();


                //   double[] slope = new double[4];
                //   double[] b=new double[4];
                //   for (int i = 0; i < vertices.Length;i++)
                //   {
                //       if (i < vertices.Length - 1)
                //           slope[i] = (vertices[i].Y - vertices[i + 1].Y) / (vertices[i].X - vertices[i + 1].X);

                //       else
                //           slope[i] = (vertices[i].Y - vertices[0].Y) / (vertices[i].X - vertices[0].X);

                //       b[i] = -(vertices[i].Y - slope[i] * vertices[i].X);
                //   }

                //       for (int i = 0; i < sourceImage.Width; i++)
                //       {
                //           for (int j = 0; j < sourceImage.Height; j++)
                //           {
                //               Color color = sourceImage.GetPixel(i, j);
                //               double[] result = new double[4];
                //               for (int k = 0; k < result.Length;k++)
                //                   result[k] = j - slope[k] * i + b[k];

                //                   if (result[0] < 0  && result[1] > 0 && result[2] > 0 && result[3] < 0)   /// explore if pixel is in bounding box of Min Area Rectangle
                //                   {
                //                       sourceImage.SetPixel(i, j, Color.Green);
                //                   }
                //           }
                //       }

                foreach (PointF p in pts)
                    img.Draw(new CircleF(p, 2), new Bgr(Color.Green), 1);
         //   }
            sourceImage.Save(@"C:\Users\nhonarva\Documents\separationResult.png");
            img.Save(@"C:\Users\nhonarva\Documents\sample.png");
            source.Save(@"C:\Users\nhonarva\Documents\sample1.png");
            source1.Save(@"C:\Users\nhonarva\Documents\sample2.png");
            sourceImage1.Save(@"C:\Users\nhonarva\Documents\sample3.png");

        }

        public static Bitmap CropRotatedRect(Bitmap source, Rectangle rect, float angle, bool HighQuality)
        {
            Bitmap result = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = HighQuality ? InterpolationMode.HighQualityBicubic : InterpolationMode.Default;
                using (Matrix mat = new Matrix())
                {
                 //   mat = result.GetHbitmap;
                    mat.Translate(-rect.Location.X, -rect.Location.Y);
                    mat.RotateAt(angle, rect.Location);
             //       mat.Translate(source.Width, source.Height);
                    g.Transform = mat;
                    g.DrawImage(source, new Point(0, 0));
                }
            }
            return result;
        }


        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

    }
}
