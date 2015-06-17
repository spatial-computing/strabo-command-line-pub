using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing;

namespace Strabo.Core.SymbolRecognition
{
    class ScanImage
    {
        public void ScanByImageDimension(string inputPath, out Image<Bgr, Byte> element, out Image<Bgr, Byte> test, out Image<Gray, Byte> gElement, TextWriter log)
        {

            Stopwatch watch = Stopwatch.StartNew();

            Image<Gray, Byte> gTest;
            // Read images.
            //element = new Image<Bgr, Byte>(string.Format("{0}{1}/in/element.png", inputPath, ""));
            //test = new Image<Bgr, Byte>(string.Format("{0}{1}/in/test.png", inputPath, ""));
            SetImages(inputPath, out element, out test, out gElement, out gTest);

            // Get image dimensions.
            int wfactor = 2;
            // The size of the element image.
            int ex = element.Width;
            int ey = element.Height;
            // The size of the test image.
            int tx = test.Width;
            int ty = test.Height;
            // The distance that the sliding window shifts.
            int xshift = tx / ex / wfactor * 2 - 1;
            int yshift = ty / ey / wfactor * 2 - 1;
            Bitmap window = test.ToBitmap();

            try
            {
                log.WriteLine(string.Format("Element Image: ({0}*{1})\nTest Image:({2}*{3})\n", ex, ey, tx, ty));
                for (int j = 0; j < yshift; j++)
                {
                    for (int i = 0; i < xshift; i++)
                    {
                        int xstart = i * ex * wfactor / 2;
                        int ystart = j * ey * wfactor / 2;
                        int counter = i + j * xshift;
                        Rectangle r = new Rectangle(xstart, ystart, ex * wfactor, ey * wfactor);
                        Image<Bgr, Byte> pTest = new Image<Bgr, Byte>(window.Clone(r, window.PixelFormat));
                        pTest.Save(string.Format("{0}{1}/Intermediate/part-" + xstart.ToString() + "-" + ystart.ToString() + "-" + counter.ToString() + ".jpg", inputPath, ""));
                        log.WriteLine(string.Format("\n\nSub-image #{0}:\n\tLoop #({1}, {2})\n\tSW1 location: ({3}, {4})", counter, i, j, xstart, ystart));

                    }
                }
            }
            catch (Exception e)
            {
                log.WriteLine(e.Message);
                throw e;
            }

        }

        public void ScanByPixel(string inputPath, int everyOtherColoumn, int everyOtherRow, out Image<Bgr, Byte> element, out Image<Bgr, Byte> test, out Image<Gray, Byte> gElement, TextWriter log)
        {

            Stopwatch watch = Stopwatch.StartNew();

            Image<Gray, Byte> gTest;


            SetImages(inputPath, out element, out test, out gElement, out gTest);
            // The size of the element image.
            int ex = element.Width * 2;
            int ey = element.Height * 2;
            everyOtherColoumn = element.Width;//*3/4 ;
            everyOtherRow = element.Height; //* 3 / 4;
            // The size of the test image.
            int tx = test.Width;
            int ty = test.Height;
            Bitmap window = test.ToBitmap();


            // The distance that the sliding window shifts.
            try
            {

                log.WriteLine(string.Format("Element Image: ({0}*{1})\nTest Image:({2}*{3})\n", ex, ey, tx, ty));
                for (int j = 0; j < ty; j += everyOtherRow)
                {
                    if (ey + j > ty)
                        break;
                    for (int i = 0; i < tx; i += everyOtherColoumn)
                    {
                        if (i + ex > tx)
                            break;
                        int counter = i + j * everyOtherRow;
                        Rectangle r = new Rectangle(i, j, ex, ey);
                        Image<Bgr, Byte> pTest = new Image<Bgr, Byte>(window.Clone(r, window.PixelFormat));
                        pTest.Save(string.Format("{0}{1}/Intermediate/part-" + i.ToString() + "-" + j.ToString() + "-" + counter.ToString() + ".jpg", inputPath, ""));

                        //log.WriteLine(string.Format("\n\nSub-image #{0}:\n\tLoop #({1}, {2})\n\tSW1 location: ({3}, {4})", counter, i, j));

                    }
                    if (ey + j > ty)
                        break;
                }
            }
            catch (Exception e)
            {
                log.WriteLine("ScanImage, ScanByPixel:" + e.Message);
                throw e;
            }
        }


        public void ScanByPixel(string inputPath,Image<Bgr, Byte> test, Image<Bgr, Byte> element,int  everyOtherColoumn, int everyOtherRow)
        {
            // The size of the element image.
            int ex = element.Width;
            int ey = element.Height;
            everyOtherColoumn = (element.Width)/2;
            everyOtherRow = (element.Height) / 2;
            // The size of the test image.
            int tx = test.Width;
            int ty = test.Height;
            Bitmap window = test.ToBitmap();


            // The distance that the sliding window shifts.
            try
            {


                for (int j = 0; j < ty; j += everyOtherRow)
                {
                    if (ey + j > ty)
                        break;
                    for (int i = 0; i < tx; i += everyOtherColoumn)
                    {
                        if (i + ex > tx)
                            break;
                        int counter = i + j * everyOtherRow;
                        Rectangle r = new Rectangle(i, j, ex, ey);
                        Image<Bgr, Byte> pTest = new Image<Bgr, Byte>(window.Clone(r, window.PixelFormat));
                        pTest.Save(string.Format("{0}{1}/Intermediate/part-" + i.ToString() + "-" + j.ToString() + "-" + counter.ToString() + ".jpg", inputPath, ""));

                        //log.WriteLine(string.Format("\n\nSub-image #{0}:\n\tLoop #({1}, {2})\n\tSW1 location: ({3}, {4})", counter, i, j));

                    }
                    if (ey + j > ty)
                        break;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private void SetImages(string inputPath, out Image<Bgr, Byte> element, out Image<Bgr, Byte> test, out Image<Gray, Byte> gElement, out Image<Gray, Byte> gTest)
        {
            element = new Image<Bgr, Byte>(string.Format("{0}{1}/in/element.png", inputPath, ""));
            test = new Image<Bgr, Byte>(string.Format("{0}{1}/in/test.png", inputPath, ""));
            // Convert to gray-level images and save.
            gElement = element.Convert<Gray, Byte>();
            gTest = test.Convert<Gray, Byte>();
            gElement.Save(string.Format("{0}{1}/in/g-element.png", inputPath, ""));
            gTest.Save(string.Format("{0}{1}/in/g-test.png", inputPath, ""));


        }
        public void SetImages(string positivePath, string negativePath, string testPath,
            out Image<Bgr, Byte>[] Positives, out Image<Bgr, Byte>[] Negatives, out Image<Bgr, Byte> test)
        {
            test = new Image<Bgr, Byte>(string.Format("{0}{1}/in/test.png", testPath, ""));
            string[] filePaths = Directory.GetFiles(positivePath, "*.png");
            Positives = new Image<Bgr, byte>[filePaths.Length];
            int counter = 0;
            foreach (string path in filePaths)
            {
                Image<Bgr, Byte> temp = new Image<Bgr, byte>(path);
                Positives[counter++] = temp;

            }
            counter = 0;
            filePaths = Directory.GetFiles(negativePath, "*.png");
            Negatives = new Image<Bgr, byte>[filePaths.Length];
            foreach (string path in filePaths)
            {
                Image<Bgr, Byte> temp = new Image<Bgr, byte>(path);
                Negatives[counter++] = temp;

            }


        }


    }
}
