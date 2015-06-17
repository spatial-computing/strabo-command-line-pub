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

using Strabo.Core.Utility;
using System.Collections.Generic;

namespace Strabo.Core.TextRecognition
{
    public static class QGISJson
    {
        private static string _streamPixel;
        public static string path;
        public static double Wx;
        public static double Ny;
        public static double yscale;
        public static double xscale;
        public static string srid="EPSG:27700";
        public static string filename;
        private static string _streamRef;

        public static void Start()
        {
             _streamPixel = "{\"type\":\"FeatureCollection\",\"features\":[";
            _streamRef ="{\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\""+srid+"\"}},\"type\":\"FeatureCollection\",\"features\":[";

        }
        public static void AddFeature(int x, int y, int h, int w, int geo_Ref, List<KeyValuePair<string,string>> items)
        {
            AddCordinationFeature(x, y, h, w, items);

            _streamPixel += "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[" + x.ToString() + "," + (geo_Ref * y).ToString() + "],[" +
                (x + w).ToString() + "," + (geo_Ref * y).ToString() + "],[" + (x + w).ToString() + "," + (geo_Ref * (y + h)).ToString() + "],[" + x.ToString() + "," + (geo_Ref * (y + h)).ToString() + "],[" +
                x.ToString() + "," + (geo_Ref * y).ToString() + "]]]}";
            for (int i = 0; i < items.Count; i++)
                _streamPixel += ",\"" + items[i].Key + "\":\"" + items[i].Value + "\"";
            _streamPixel +=",},";

        }

        public static void WriteGeojsonFiles()
        {
            Log.WriteLine("Preparing GeojsonFile for QGIS");
            _streamPixel += "]}";
            _streamRef += "]}";
            System.IO.File.WriteAllText(path + "\\" + filename + "ByRef.txt", _streamRef);
            System.IO.File.WriteAllText(path + "\\" + filename + "ByPixels.txt", _streamPixel);

        }
        private static void AddCordinationFeature(int x, int y, int h, int w, List<KeyValuePair<string,string>> items)
        {
            double refx, refy, refh, refw;
            refx = Wx + (x * xscale);///(double)(MapServerParameters.Resize);
            refy = Ny - (y * yscale);// / (double)(MapServerParameters.Resize);
            refh = h * yscale;// / (double)(MapServerParameters.Resize);
            refw = w * xscale;// / (double)(MapServerParameters.Resize);

            _streamRef += "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[" + refx.ToString() + "," + (refy).ToString() + "],[" +
            (refx + refw).ToString() + "," + (refy).ToString() + "],[" + (refx + refw).ToString() + "," + ((refy - refh)).ToString() + "],[" + refx.ToString() + "," + ((refy - refh)).ToString() + "],[" +
            refx.ToString() + "," + (refy).ToString() + "]]]}";
            for (int i = 0; i < items.Count; i++)
            {
                _streamRef += ",\"" + items[i].Key + "\":\"" + items[i].Value + "\"";
            }
             _streamRef += ",},";
            
        }
    }
}
