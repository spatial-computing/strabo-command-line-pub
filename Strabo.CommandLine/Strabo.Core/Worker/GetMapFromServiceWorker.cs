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

using Strabo.Core.ImageProcessing;
using Strabo.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strabo.Core.Worker
{
    public class GetMapFromServiceWorker
    {
        public GetMapFromServiceWorker() { }
        public void Apply(InputArgs inputArgs)
        {
            Apply(inputArgs.bbx.BBW, inputArgs.bbx.BBN, inputArgs.mapLayerName, inputArgs.intermediatePath, StraboParameters.sourceMapFileName);
        }
        public void Apply(string BBW, string BBN, string mapLayerName, string output_path, string output_filename)
        {
            try
            {
                MapServerParameters.BBOXW = BBW;
                MapServerParameters.BBOXN = BBN;
                MapServerParameters.layer = mapLayerName;
                MapServerParameters.URLBuilder();

                if (MapServerParameters.WMTS)
                {
                    GetMapFromWMTS wmtsWorker = new GetMapFromWMTS(MapServerParameters.targetURL, MapServerParameters.Username,
                       MapServerParameters.Password, MapServerParameters.WidthMeterScale, MapServerParameters.HeightMeterScale, MapServerParameters.ZoomLevel);
                    Log.WriteLine("Connecting WMTS...");
                    Tile tile = wmtsWorker.Apply(output_path,output_filename);
                    MapServerParameters.BBOXW = tile.westCorner.ToString();
                    MapServerParameters.BBOXN = tile.northCorner.ToString();
                    Log.WriteLine("WMTSWorker finished");
                }
                else
                {
                    GetMapFromWMS wmsWorker = new GetMapFromWMS();
                    Log.WriteLine("Connecting WMS...");
                    wmsWorker.Apply(output_path);
                    Log.WriteLine("WMSWorker finished");
                }
                if (MapServerParameters.Resize > 1)
                    ImageUtils.ImageResize(output_path, output_filename);
            }
            catch (Exception e)
            {
                Log.WriteLine("GetMapFromServiceWorker:" + e.Message);
                Log.WriteLine(e.Source);
                Log.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}
