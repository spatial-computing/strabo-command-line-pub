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

using Emgu.CV;
using Emgu.CV.Structure;
using Strabo.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Strabo.Core.ImageProcessing
{
    public static class ImageUtils
    {
        public static Bitmap ImageResize(string intermediatePath, string output_filename)
        {
            string path = intermediatePath + output_filename;
            int resize = MapServerParameters.Resize;

            Bitmap img = new Bitmap(path);
            Size size = new Size(img.Width * resize, img.Height * resize);
            Bitmap newImage = new Bitmap(img, size);
            img.Dispose();

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    newImage.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return newImage;
        }

        public static Bitmap InvertColors(Bitmap srcimg)
        {
            using (Image<Gray, byte> img = new Image<Gray, byte>(srcimg))
            { return img.Not().ToBitmap(); }
        }

        public static Bitmap CreateBlankGrayScaleBmp(int width, int height)
        {
            return new Image<Gray, Byte>(width, height).ToBitmap();
        }

        public static Bitmap ConvertToGrayScale(Bitmap srcimg)
        {
            using (Image<Gray, byte> img = new Image<Gray, byte>(srcimg))
            { return img.Convert<Gray, Byte>().ToBitmap(); }
        }

        public static Bitmap ConvertGrayScaleToBinary(Bitmap srcimg, int threshold)
        {
            using (Image<Gray, byte> img = new Image<Gray, byte>(srcimg))
            { return img.Convert<Gray, Byte>().ThresholdBinary(new Gray(threshold), new Gray(255)).ToBitmap(); }
        }

        public static Bitmap GetIntersection(Bitmap img1, Bitmap img2)
        {
            using (Image<Gray, byte> ip1 = new Image<Gray, byte>(img1))
            using (Image<Gray, byte> ip2 = new Image<Gray, byte>(img2))
            { return ip1.And(ip2).ToBitmap(); }
        }

        public static Bitmap AnyToFormat24bppRgb(Bitmap srcimg)
        {
            if (srcimg.PixelFormat != PixelFormat.Format24bppRgb)
            {
                Bitmap tmp = new Bitmap(srcimg.Width, srcimg.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (Graphics gr = Graphics.FromImage(tmp))
                {
                    gr.DrawImage(srcimg, new Rectangle(0, 0, srcimg.Width, srcimg.Height));
                }
                srcimg = (Bitmap)(tmp.Clone());
                tmp.Dispose();
                tmp = null;
            }
            return srcimg;
        }
        public static int GetNumberOfUniqueColors(Bitmap srcimg)
        {
            srcimg = AnyToFormat24bppRgb(srcimg);

            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - width * 3;

            HashSet<int> cnt = new HashSet<int>();
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += 3)
                        cnt.Add(src[RGB.R] * 256 * 256 + src[RGB.G] * 256 + src[RGB.B]);
                    src += srcOffset;
                }
            }
            srcimg.UnlockBits(srcData);
            return cnt.Count;
        }
        public static int[] BitmapToArray1DIntRGB(Bitmap srcimg)
        {
            srcimg = AnyToFormat24bppRgb(srcimg);

            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - width * 3;

            int[] image = new int[height * width];
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                // RGB binarization
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += 3)
                        image[y * width + x] = src[RGB.R] * 256 * 256 + src[RGB.G] * 256 + src[RGB.B];
                    src += srcOffset;
                }
            }
            srcimg.UnlockBits(srcData);
            return image;
        }
        public static Bitmap Array1DToBitmapRGB(int[] input, int width, int height)
        {
            // create new grayscale image
            Bitmap dstImg = new Bitmap(width, height);

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int dstOffset = dstData.Stride - width * 3;

            // do the job
            unsafe
            {
                byte* dst = (byte*)dstData.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst += 3)
                    {
                        dst[RGB.R] = (byte)((input[y * width + x] & 0xFF0000) >> 16);
                        dst[RGB.G] = (byte)((input[y * width + x] & 0xFF00) >> 8);
                        dst[RGB.B] = (byte)((input[y * width + x] & 0xFF));
                    }
                    dst += dstOffset;
                }
            }
            dstImg.UnlockBits(dstData);
            return dstImg;
        }
        public static bool[,] BitmapToArray2D(Bitmap srcimg, int threshold, int margin)
        {
            srcimg = AnyToFormat24bppRgb(srcimg);
            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - (width * 3);

            bool[,] image = new bool[height, width];
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                if (srcFmt == PixelFormat.Format8bppIndexed)
                {
                    // graysclae binarization
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src++)
                        {
                            if (y > margin && y < height - margin && x > margin && x < width - margin)
                                image[y, x] = ((*src > threshold) ? false : true); //bg : fg
                            else
                                image[y, x] = false;
                        }
                        src += srcOffset;
                    }
                }
                else
                {
                    byte v;

                    // RGB binarization
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += 3)
                        {
                            // grayscale value using BT709
                            if (y > margin && y < height - margin && x > margin && x < width - margin)
                            {
                                v = (byte)(0.2125f * src[RGB.R] + 0.7154f * src[RGB.G] + 0.0721f * src[RGB.B]);
                                image[y, x] = ((v > threshold) ? false : true);
                            }
                            else
                                image[y, x] = false;
                        }
                        src += srcOffset;
                    }
                }
            }
            srcimg.UnlockBits(srcData);
            return image;
        }
        /// <summary>
        /// Bitmaps to bool array2 d.
        /// </summary>
        /// <param name="srcimg">The srcimg.</param>
        /// <param name="margin">The margin.</param>
        /// <returns></returns>
        public static bool[,] BitmapToBoolArray2D(Bitmap srcimg, int margin)
        {
            srcimg = AnyToFormat24bppRgb(srcimg);

            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - (width * 3);

            bool[,] image = new bool[height, width];
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                if (srcFmt == PixelFormat.Format8bppIndexed)
                {
                    // graysclae binarization
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src++)
                        {
                            byte s = *src;
                            if (y > margin && y < height - margin && x > margin && x < width - margin)
                                image[y, x] = (s > 128) ? false : true;
                            else
                                image[y, x] = false;
                        }
                        src += srcOffset;
                    }
                }
                else
                {
                    byte v;

                    // RGB binarization
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, src += 3)
                        {
                            // grayscale value using BT709
                            if (y > margin && y < height - margin && x > margin && x < width - margin)
                            {
                                v = (byte)(0.2125f * src[RGB.R] + 0.7154f * src[RGB.G] + 0.0721f * src[RGB.B]);
                                image[y, x] = ((v >= 128) ? false : true);
                            }
                            else
                                image[y, x] = false;
                        }
                        src += srcOffset;
                    }
                }
            }
            srcimg.UnlockBits(srcData);
            return image;
        }
        public static bool[,] BitmapToBoolArray2D(Bitmap srcimg, int margin, int th)
        {
            srcimg = AnyToFormat24bppRgb(srcimg);

            // get source image size
            int width = srcimg.Width;
            int height = srcimg.Height;

            PixelFormat srcFmt = PixelFormat.Format24bppRgb;

            // lock source bitmap data
            BitmapData srcData = srcimg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, srcFmt);

            int srcOffset = srcData.Stride - (width * 3);

            bool[,] image = new bool[height, width];
            // do the job
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();

                byte v;

                // RGB binarization
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += 3)
                    {
                        // grayscale value using BT709
                        if (y > margin && y < height - margin && x > margin && x < width - margin)
                        {
                            v = (byte)(0.2125f * src[RGB.R] + 0.7154f * src[RGB.G] + 0.0721f * src[RGB.B]);
                            image[y, x] = ((v >= th) ? false : true);
                        }
                        else
                            image[y, x] = false;
                    }
                    src += srcOffset;
                }

            }
            srcimg.UnlockBits(srcData);
            return image;
        }
        public static Bitmap Array2DToBitmap(bool[,] input)
        {
            return Array2DToBitmap(input, 0);
        }
        public static Bitmap Array2DToBitmap(bool[,] input, int margin)
        {
            // get source image size
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            Bitmap dstImg = ImageUtils.CreateBlankGrayScaleBmp(width, height);

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int dstOffset = dstData.Stride - width;

            // do the job
            unsafe
            {
                byte* dst = (byte*)dstData.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        if (y > margin && y < height - margin && x > margin && x < width - margin)
                        {
                            if (input[y, x] == true) *dst = (byte)0;
                            else if (input[y, x] == false) *dst = (byte)255;
                            else *dst = (byte)128;
                        }
                        else *dst = (byte)255;
                    }
                    dst += dstOffset;
                }
            }
            dstImg.UnlockBits(dstData);
            return dstImg;
        }
        public static Bitmap ArrayBool2DToBitmap(bool[,] input)
        {
            // get source image size
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            // create new grayscale image
            Bitmap dstImg = new Bitmap(width, height, PixelFormat.Format8bppIndexed);// AForge.Imaging.Image.CreateGrayscaleImage(width, height);

            // lock destination bitmap data
            BitmapData dstData = dstImg.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int dstOffset = dstData.Stride - width;

            // do the job
            unsafe
            {
                byte* dst = (byte*)dstData.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, dst++)
                    {
                        if (input[y, x]) *dst = (byte)0;
                        else *dst = (byte)255;

                    }
                    dst += dstOffset;
                }
            }
            dstImg.UnlockBits(dstData);
            return dstImg;
        }
        public static Bitmap toGray(Bitmap srcimg)
        {
            if (srcimg.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                //ASHISH
                return ImageUtils.ConvertToGrayScale(srcimg);
            }
            else
                return srcimg;
        }
        public static Bitmap toRGB(Bitmap srcimg)
        {
            if (srcimg.PixelFormat == PixelFormat.Format8bppIndexed)
            {

                // get source image size
                int width = srcimg.Width;
                int height = srcimg.Height;

                // create new grayscale image
                Bitmap dstImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);// AForge.Imaging.Image.CreateGrayscaleImage(width, height);

                // lock destination bitmap data
                BitmapData srcData = srcimg.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

                // lock destination bitmap data
                BitmapData dstData = dstImg.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int srcOffset = srcData.Stride - width;
                int dstOffset = dstData.Stride - width * 3;
                // do the job
                unsafe
                {
                    byte* src = (byte*)srcData.Scan0.ToPointer();
                    byte* dst = (byte*)dstData.Scan0.ToPointer();
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++, dst += 3, src++)
                        {
                            //*dst = *src;
                            dst[0] = src[0]; dst[1] = src[0]; dst[2] = src[0];

                        }
                        dst += dstOffset;
                    }
                }
                srcimg.UnlockBits(srcData);
                dstImg.UnlockBits(dstData);
                return dstImg;
            }
            else
                return srcimg;
        }

        public static float[] RGB2YIQ(int rgb)
        {
            float Rc = ((rgb & 0xFF0000) >> 16);
            float Gc = ((rgb & 0xFF00) >> 8);
            float Bc = (rgb & 0xFF);
            float[] yiq = new float[3];
            yiq[0] = 0.299f * Rc + 0.587f * Gc + 0.114f * Bc; // Y
            yiq[1] = 0.5957f * Rc - 0.2744f * Gc - 0.3212f * Bc; // I
            yiq[2] = 0.2114f * Rc - 0.5226f * Gc + 0.3111f * Bc; // Q
            return yiq;
        }
    }
}
