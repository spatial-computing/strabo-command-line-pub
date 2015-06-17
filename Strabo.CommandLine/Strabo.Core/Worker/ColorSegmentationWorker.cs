﻿/*******************************************************************************
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

using nQuant;
using Strabo.Core.ColorSegmentation;
using Strabo.Core.Utility;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Strabo.Core.Worker
{
    /// <summary>
    /// Starting class for color segmentation
    /// </summary>
    public class ColorSegmentationWorker
    {
        public ColorSegmentationWorker() { }
        public string Apply(string intermediatePath, int tn)
        {
            string input_dir = intermediatePath;
            string output_dir = intermediatePath;
            string fn = StraboParameters.sourceMapFileName;
            int k = StraboParameters.numberOfSegmentationColor;
            return Apply(input_dir, output_dir, fn, k, tn);
        }
        public string Apply(string input_dir, string output_dir, string fn, int k, int tn)
        {
            try
            {
                string fn_only = Path.GetFileNameWithoutExtension(fn);

                //MeanShift
                Log.WriteLine("Meanshift in progress...");
                MeanShiftMultiThreads mt = new MeanShiftMultiThreads();
                string ms_path = output_dir + fn_only + "_ms.png";
                mt.ApplyYIQMT(input_dir + fn, tn, 3, 3, ms_path);
                mt = null;
                GC.Collect();
                Log.WriteLine("Meanshift finished");

                // Median Cut
                Log.WriteLine("Median Cut in progress...");
                int mc_colors = 256;
                string mc_path = output_dir + fn_only + "_mc" + mc_colors.ToString() + ".png";

                using (Bitmap msimg = new Bitmap(output_dir + fn_only + "_ms.png"))
                {
                    WuQuantizer wq = new WuQuantizer();
                    wq.QuantizeImage(msimg).Save(mc_path, ImageFormat.Png);
                }
                Log.WriteLine("Median Cut finished");

                // KMeans
                Log.WriteLine("KMeans in progress...");
                MyKMeans kmeans = new MyKMeans();
                string kmeans_path = output_dir + fn_only + "_mc" + mc_colors + "_k" + k + ".png";
                kmeans.Apply(k, mc_path, kmeans_path);
                kmeans = null;
                GC.Collect();
                Log.WriteLine("KMeans finished");
                return kmeans_path;
            }
            catch (Exception e)
            {
                Log.WriteLine("ColorSegmentationWorker: " + e.Message);
                Log.WriteLine(e.Source);
                Log.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}
