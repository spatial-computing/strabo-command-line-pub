/*******************************************************************************
 * Copyright 2010 University of Southern California
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * 	http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * This code was developed as part of the Strabo map processing project 
 * by the Spatial Sciences Institute and by the Information Integration Group 
 * at the Information Sciences Institute of the University of Southern 
 * California. For more information, publications, and related projects, 
 * please see: http://spatial-computing.github.io/
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Strabo.Core.ImageProcessing
{
    public class ColorHistogram
    {

        int[] colorArray = null;
        int[] countArray = null;

        public ColorHistogram() { }

        public class RGB
        {
            public const short B = 0;
            public const short G = 1;
            public const short R = 2;
        }
        ~ColorHistogram() 
        { 
            //Console.WriteLine("close histogram"); 
        }
        public bool CheckImageColors(Bitmap srcimg, int min_number_color)
        {
            srcimg = ImageUtils.AnyToFormat24bppRgb(srcimg);
            HashSet<int> color24bppRgb = new HashSet<int>();
            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = (srcimg.PixelFormat == PixelFormat.Format8bppIndexed) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - ((srcFmt == PixelFormat.Format8bppIndexed) ? width : width * 3);


            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                {
                    // RGB binarization
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += 3)
                        {
                            color24bppRgb.Add(src[RGB.R] * 256 * 256 + src[RGB.G] * 256 + src[RGB.B]);
                            if (color24bppRgb.Count > min_number_color)
                            {
                                srcimg.UnlockBits(srcData);
                                return true;
                            }
                        }
                        src += srcOffset;
                    }
                }
            }
            srcimg.UnlockBits(srcData);
            return false;
        }
        public HashSet<int> GetColorHashSet(Bitmap srcimg)
        {
            srcimg=ImageUtils.AnyToFormat24bppRgb(srcimg);
            HashSet<int> color24bppRgb = new HashSet<int>();
            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = (srcimg.PixelFormat == PixelFormat.Format8bppIndexed) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - ((srcFmt == PixelFormat.Format8bppIndexed) ? width : width * 3);

      
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                {
                    // RGB binarization
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += 3)
                            color24bppRgb.Add( src[RGB.R] * 256 * 256 + src[RGB.G] * 256 + src[RGB.B]);
                        src += srcOffset;
                    }
                }
            }
            srcimg.UnlockBits(srcData);
            return color24bppRgb;
        }
        public ColorHistogram(int[] pixelsOrig)
        {
            int N = pixelsOrig.Length;
            int[] pixelsCpy = (int[])pixelsOrig.Clone() ;// new int[N];
            Array.Sort(pixelsCpy);

            // count unique colors:
            int k = -1; // current color index
            int curColor = -1;
            for (int i = 0; i < pixelsCpy.Length; i++)
            {
                if (pixelsCpy[i] != curColor)
                {
                    k++;
                    curColor = pixelsCpy[i];
                }
            }
            int nColors = k + 1;

            // tabulate and count unique colors:
            colorArray = new int[nColors];
            countArray = new int[nColors];
            k = -1;	// current color index
            curColor = -1;
            for (int i = 0; i < pixelsCpy.Length; i++)
            {
                if (pixelsCpy[i] != curColor)
                {	// new color
                    k++;
                    curColor = pixelsCpy[i];
                    colorArray[k] = curColor;
                    countArray[k] = 1;
                }
                else
                {
                    countArray[k]++;
                }
            }
            pixelsCpy = null;
            GC.Collect();
        }
        public ColorHistogram(int[] pixelsOrig, bool modify)
        {
            int N = pixelsOrig.Length;
            //int[] pixelsOrig = (int[])pixelsOrig.Clone();// new int[N];
            Array.Sort(pixelsOrig);

            // count unique colors:
            int k = -1; // current color index
            int curColor = -1;
            for (int i = 0; i < pixelsOrig.Length; i++)
            {
                if (pixelsOrig[i] != curColor)
                {
                    k++;
                    curColor = pixelsOrig[i];
                }
            }
            int nColors = k + 1;

            // tabulate and count unique colors:
            colorArray = new int[nColors];
            countArray = new int[nColors];
            k = -1;	// current color index
            curColor = -1;
            for (int i = 0; i < pixelsOrig.Length; i++)
            {
                if (pixelsOrig[i] != curColor)
                {	// new color
                    k++;
                    curColor = pixelsOrig[i];
                    colorArray[k] = curColor;
                    countArray[k] = 1;
                }
                else
                {
                    countArray[k]++;
                }
            }
            pixelsOrig = null;
            GC.Collect();
        }
        public int[] getColorArray()
        {
            return colorArray;
        }

        public int[] getCountArray()
        {
            return countArray;
        }

        public int getNumberOfColors()
        {
            if (colorArray == null)
                return 0;
            else
                return colorArray.Length;
        }

        public int getColor(int index)
        {
            return this.colorArray[index];
        }

        public int getCount(int index)
        {
            return this.countArray[index];
        }
    }
}
