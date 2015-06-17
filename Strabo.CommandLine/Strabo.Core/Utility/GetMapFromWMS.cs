using Strabo.Core.ImageProcessing;
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
using System.Drawing;

namespace Strabo.Core.Utility
{
    /// <summary>
    /// Simas
    /// </summary>
    public class GetMapFromWMS
    {
        public void Apply(string intermediatePath)
        {
            //tergetURL = "http://secure.futureclimateinfo.com/api/wms?request=GetMap&BBOX=314100,543800,314600,544300&SRS=EPSG:27700&WIDTH=1056&HEIGHT=1056&LAYERS=nls-sixinch&STYLES=&FORMAT=image/png";
            //tergetURL = "http://secure.futureclimateinfo.com/api/wms?SERVICE=WMS&VERSION=1.1.1&request=GetMap&BBOX=314100,543800,314600,544300&SRS=EPSG:27700&WIDTH=1056&HEIGHT=1056&LAYERS=nls-sixinch&STYLES=&FORMAT=image/pnG";
            //tergetURL = "http://secure.futureclimateinfo.com/api/wms?request=GetMap&BBOX=589400,194000,589900,194500&SRS=EPSG:27700&WIDTH=1056&HEIGHT=1056&LAYERS=nls-sixinch&STYLES=&FORMAT=image/png";
            //tergetURL = "http://secure.futureclimateinfo.com/api/wms?request=GetMap&BBOX=589400,194000,590400,195000&SRS=EPSG:27700&WIDTH=947&HEIGHT=947&LAYERS=nls-sixinch&STYLES=&FORMAT=image/pnG";
            //MapServerParameters.targetURL = "http://secure.futureclimateinfo.com/api/wms?&&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&BBOX=597528,209822,598528,210822&SRS=EPSG:27700&WIDTH=1500&HEIGHT=1500&LAYERS=nls-sixinch:&STYLES=&FORMAT=image/png&DPI=96&MAP_RESOLUTION=96&FORMAT_OPTIONS=dpi:96&TRANSP...
            Bitmap SourceImage = ConnectToMapServer.ConnectToWMS(MapServerParameters.targetURL);

            SourceImage.Save(intermediatePath+StraboParameters.sourceMapFileName);
            ColorHistogram ch = new ColorHistogram();

            if (!ch.CheckImageColors(SourceImage,2))
            {
                Exception e = new Exception("The returned image is invalid, it has less than two colors.");
                Log.WriteLine("The returned image is invalid, it has less than two colors.");
                throw e;
            }         
        }
    }
}
