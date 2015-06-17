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

using AForge.Imaging.Filters;
using Strabo.Core.ImageProcessing;
using Strabo.Core.TextDetection;
using Strabo.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Strabo.Core.Worker
{
    /// <summary>
    /// Sima 
    /// Starting class for text recognition
    /// </summary>
    public class TextDetectionWorker
    {
        public double angle_ratio = 0.3;//0.19

        public TextDetectionWorker() { }
        public void Apply(string intermediatePath, double size_ratio, bool preprocessing, int tnum)
        {
            string input_dir = intermediatePath;
            string output_dir = intermediatePath;
            string fn = StraboParameters.textLayerOutputFileName;
            int char_size = StraboParameters.char_size;
            Apply(input_dir, output_dir, fn, char_size, size_ratio, preprocessing, tnum, StraboParameters.language);
        }
        public void Apply(string input_dir, string output_dir, string fn, int char_size, double size_ratio, bool preprocessing, int tnum, string lang)
        {
            MergeTextStrings mts = new MergeTextStrings();

            ImageSlicer imgslicer = new ImageSlicer();
            List<TextString> text_string_list = new List<TextString>();
            List<string> imgpath_list = imgslicer.Apply(1, 1, 100, input_dir + fn, output_dir);

            Log.WriteLine("Grouping text strings...");
            for (int s = 0; s < imgpath_list.Count; s++)
            {
                Bitmap srcimg = null;

                if (lang == "eng")
                {
                    using (Bitmap tileimg = new Bitmap(imgpath_list[s]))
                    {
                        RemoveBoarderAndNoiseCC removeBoarderAndNoiseCC = new RemoveBoarderAndNoiseCC();
                        srcimg = removeBoarderAndNoiseCC.Apply(tileimg, char_size, 0.18);
                        Log.WriteBitmap2FolderExactFileName(output_dir, srcimg, "CDAInput.png");
                    }
                }
                else
                {
                    using (Bitmap tileimg = new Bitmap(imgpath_list[s]))
                    {
                        RemoveBoarderCC removeBoarderCC = new RemoveBoarderCC();
                        srcimg = removeBoarderCC.Apply(tileimg);
                        Log.WriteBitmap2FolderExactFileName(output_dir, srcimg, "CDAInput.png");
                    }
                }
                ConditionalDilationAutomatic cda = new ConditionalDilationAutomatic();
                cda.ang_threshold = angle_ratio;
                string outputImagePath = output_dir + s + ".png";
                cda.Apply(tnum, srcimg, size_ratio, angle_ratio, preprocessing, outputImagePath);

                using (Bitmap dilatedimg = new Bitmap(outputImagePath))
                {
                    DetectTextStrings detectTS = new DetectTextStrings();
                    List<TextString> string_list = detectTS.Apply(srcimg, dilatedimg);
                    List<Bitmap> string_img_list = new List<Bitmap>();
                    for (int i = 0; i < string_list.Count; i++)
                        string_img_list.Add(string_list[i].srcimg);

                    int[] offset = imgslicer.xy_offset_list[s];

                    for (int i = 0; i < string_list.Count; i++)
                    {
                        string_list[i].x_offset = offset[0];
                        string_list[i].y_offset = offset[1];
                        mts.AddTextString(string_list[i]);
                    }
                }
                using (Bitmap CDAInputwithLabel=new Bitmap (outputImagePath))
                {
                    Graphics g = Graphics.FromImage(CDAInputwithLabel);
                    for (int i = 0; i < mts.text_string_list.Count; i++)
                    {
                        Font font = new Font("Arial", 20);
                        g.DrawString(i.ToString(), font, Brushes.Red, mts.text_string_list[i].bbx.X, mts.text_string_list[i].bbx.Y);

                        g.DrawRectangle(new Pen(Color.Green, 4), mts.text_string_list[i].bbx);

                    }
                    Log.WriteBitmap2FolderExactFileName(output_dir, CDAInputwithLabel, "CDAInputwithLabel.png");
                    g.Dispose();
                    g=null;
                }
                srcimg.Dispose();
                srcimg = null;
            }

            Log.WriteLine("Detecting long string orientation...");
            DetectTextOrientation detectTextOrientation = new DetectTextOrientation();
            detectTextOrientation.Apply(mts, tnum);

            Log.WriteLine("Detecting short string orientation...");
            for (int i = 0; i < mts.text_string_list.Count; i++)
            {
                if (mts.text_string_list[i].char_list.Count <= 3)
                {
                    List<int> nearest_string_list = detectTextOrientation.findNearestSrings(i, mts.text_string_list);
                    int initial_orientation_count = mts.text_string_list[i].orientation_list.Count;
                    for (int j = 0; j < nearest_string_list.Count; j++)
                        mts.text_string_list[i].orientation_list.AddRange(mts.text_string_list[nearest_string_list[j]].orientation_list);
                    for (int j = initial_orientation_count; j < mts.text_string_list[i].orientation_list.Count; j++)
                    {
                        RotateBilinear filter = new RotateBilinear(mts.text_string_list[i].orientation_list[j]);
                        Bitmap rotateimg = ImageUtils.InvertColors(filter.Apply(ImageUtils.InvertColors(mts.text_string_list[i].srcimg)));
                        mts.text_string_list[i].rotated_img_list.Add(rotateimg);
                    }
                }
            }

            Log.WriteLine("Writing string results...");
            mts.WriteBMP(output_dir);
        }
        #region debuging_code
        //public Bitmap PrintSubStringBBXonMap(Bitmap srcimg)
        //{
        //    //changed here - ASHISH

        //    srcimg = ImageUtils.InvertColors(srcimg);

        //    /*
        //    Invert ivt = new Invert();
        //    srcimg = ivt.Apply(srcimg);
        //    */

        //    srcimg = ImageUtils.toRGB(srcimg);
        //    Graphics g = Graphics.FromImage(srcimg);
        //    for (int i = 1; i < initial_string_list.Count; i++)
        //    {
        //        g.DrawRectangle(new Pen(Color.Red, 6), initial_string_list[i].bbx);
        //        //Font font = new Font("Arial", 20);
        //        //g.DrawString(i.ToString(), font, Brushes.Blue, initial_string_list[i].bbx.X, initial_string_list[i].bbx.Y);
        //        for (int j = 0; j < initial_string_list[i].final_string_list.Count; j++)
        //            g.DrawRectangle(new Pen(Color.Green, 4), initial_string_list[i].final_string_list[j].bbx);

        //        //for (int j = 0; j < initial_string_list[i].char_list.Count; j++)
        //        //{
        //        //    Font font2 = new Font("Arial", 20);
        //        //    g.DrawString(j.ToString(), font2, Brushes.Red, initial_string_list[i].char_list[j].bbx.X, initial_string_list[i].char_list[j].bbx.Y);
        //        //   // g.DrawRectangle(new Pen(Color.Yellow, 2), initial_string_list[i].char_list[j].bbx);
        //        //}

        //    }
        //    g.Dispose();
        //    return srcimg;
        //}
        //public Bitmap PrintSubStringNumonMap(Bitmap srcimg)
        //{
        //    //ASHISH
        //    srcimg = ImageUtils.InvertColors(srcimg);
        //    /*
        //    Invert ivt = new Invert();
        //    srcimg = ivt.Apply(srcimg);
        //    */
        //    srcimg = ImageUtils.toRGB(srcimg);
        //    Graphics g = Graphics.FromImage(srcimg);
        //    for (int i = 1; i < initial_string_list.Count; i++)
        //    {
        //        for (int j = 0; j < initial_string_list[i].char_list.Count; j++)
        //        {
        //            Font font2 = new Font("Arial", 20);
        //            g.DrawString(j.ToString(), font2, Brushes.Red, initial_string_list[i].char_list[j].bbx.X, initial_string_list[i].char_list[j].bbx.Y);
        //            // g.DrawRectangle(new Pen(Color.Yellow, 2), initial_string_list[i].char_list[j].bbx);
        //        }
        //    }
        //    g.Dispose();
        //    return srcimg;
        //}
        //public Bitmap PrintSubStringsSmall(TextString ts)
        //{
        //    bool[,] stringimg = new bool[ts.bbx.Height + 100, ts.bbx.Width + 100];
        //    for (int i = 0; i < ts.char_list.Count; i++)
        //    {
        //        for (int xx = ts.bbx.X; xx < ts.bbx.X + ts.bbx.Width; xx++)
        //            for (int yy = ts.bbx.Y; yy < ts.bbx.Y + ts.bbx.Height; yy++)
        //            {
        //                if (char_labels[yy * mw + xx] == ts.char_list[i].pixel_id)
        //                    stringimg[yy - ts.bbx.Y + 50, xx - ts.bbx.X + 50] = true;
        //                //else
        //                //   stringimg[yy - ts.bbx.Y, xx - ts.bbx.X] = false;
        //            }

        //    }
        //    ts.srcimg = ImageUtils.ArrayBool2DToBitmap(stringimg); ;
        //    return ts.srcimg;
        //}
        //public Bitmap PrintSubStringsSmall(TextString ts, int margin)
        //{
        //    bool[,] stringimg = new bool[ts.bbx.Height + margin, ts.bbx.Width + margin];
        //    for (int i = 0; i < ts.char_list.Count; i++)
        //    {
        //        for (int xx = ts.bbx.X; xx < ts.bbx.X + ts.bbx.Width; xx++)
        //            for (int yy = ts.bbx.Y; yy < ts.bbx.Y + ts.bbx.Height; yy++)
        //            {
        //                if (char_labels[yy * mw + xx] == ts.char_list[i].pixel_id)
        //                    stringimg[yy - ts.bbx.Y + margin / 2, xx - ts.bbx.X + margin / 2] = true;
        //                //else
        //                //   stringimg[yy - ts.bbx.Y, xx - ts.bbx.X] = false;
        //            }

        //    }
        //    ts.srcimg = ImageUtils.ArrayBool2DToBitmap(stringimg); ;
        //    return ts.srcimg;
        //}
        #endregion debuging_code
    }
}
