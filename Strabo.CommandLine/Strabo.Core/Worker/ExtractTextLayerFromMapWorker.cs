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

using Strabo.Core.ImageProcessing;
using Strabo.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Strabo.Core.Worker
{
    public class ExtractTextLayerFromMapWorker
    {
        public ExtractTextLayerFromMapWorker()
        { }
        public void Apply(string inputPath, string intermediatePath, int tnum)
        {
            RGBThreshold thd = StraboParameters.rgbThreshold;
            string outputPath = intermediatePath + StraboParameters.textLayerOutputFileName;
            Apply(inputPath, outputPath, intermediatePath, thd, tnum);
        }
        public void Apply(string inputPath, string outputPath, string intermediatePath, RGBThreshold thd, int tnum)
        {
            try
            {
                if (thd.upperBlueColorThd == thd.lowerBlueColorThd || thd.upperGreenColorThd == thd.lowerGreenColorThd || thd.upperRedColorThd == thd.lowerRedColorThd)
                    Binarization.ApplyBradleyLocalThresholding(inputPath,outputPath);
                else
                    Binarization.ApplyRGBColorThresholding(inputPath, outputPath, thd, tnum);
            }
            catch (Exception e)
            {
                Log.WriteLine("ExtractTextLayerFromMapWorker:" + e.Message);
                Log.WriteLine(e.Source);
                Log.WriteLine(e.StackTrace);
                throw;
            }
        }

    }
}
