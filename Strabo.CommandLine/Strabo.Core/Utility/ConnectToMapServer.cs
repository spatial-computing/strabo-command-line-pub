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
using System.Net;

namespace Strabo.Core.Utility
{
    static public class ConnectToMapServer
    {
        public static Bitmap ConnectToWMS(string URL)
        {
            Log.WriteLine("GetMapFromWMS Starts");
            Log.WriteLine(URL);
            WebClient webClient = new WebClient();

            try
            {
                byte[] imageBytes = webClient.DownloadData(URL);
                Bitmap data = BytesToBitmap(imageBytes);
                webClient.Dispose();
                Log.WriteLine("WMS Connection Built");
                imageBytes = null;
                return data;
            }
            catch (Exception e)
            {
                Log.WriteLine("Error at GetMapFromWMS");
                Log.WriteLine(e.Message);
                Log.WriteLine(e.StackTrace);
                Log.WriteLine(e.Source);
                throw;
            }
        }
        public static Bitmap ConnectToWMTS(string URL, string username, string password)
        {

            Log.WriteLine("GetMapFromWMS Starts");
            Log.WriteLine(URL);
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(username, password);

            try
            {
                byte[] imageBytes = webClient.DownloadData(URL);
                Bitmap data = BytesToBitmap(imageBytes);
                webClient.Dispose();
                Log.WriteLine("WMTS Connection Built and Map Received");
                imageBytes = null;
                return data;
            }
            catch (Exception e)
            {
                Log.WriteLine("Error at GetMapFromWMTS");
                Log.WriteLine(e.Message);
                Log.WriteLine(e.StackTrace);
                Log.WriteLine(e.Source);
                throw;
            }
        }
        private static Bitmap BytesToBitmap(byte[] byteArray)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray))
            {
                return (Bitmap)Image.FromStream(ms);
            }
        }
    }
}
