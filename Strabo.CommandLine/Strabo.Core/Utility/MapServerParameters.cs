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
using System.Configuration;

namespace Strabo.Core.Utility
{

    public static class MapServerParameters
    {
        private static string _targetURL;
        private static string _url;
        private static string _service;
        private static string _version;
        private static string _request;
        private static string _bBOXW;
        private static string _bBOXE;
        private static string _bBOXN;
        private static string _bBOXS;
        private static string _srs;
        private static string _width;
        private static string _height;
        private static string _layer;
        private static string _format;
        private static string _dpi;
        private static string _map_resolution;
        private static string _format_option;
        private static string _transparent;
        private static string _styles;
        private static double _yscale;
        private static double _xscale;
        private static int _resize=1;
        private static string _srid;
        private static Boolean _wmts;
        private static string _username;
        private static string _password;
        private static int _zoomLevel;
        private static double _meterWidth;
        private static double _meterHeight;

        private static string ReadString(string key, string default_value)
        {
            return ConfigurationManager.AppSettings[key] != null ? ConfigurationManager.AppSettings[key].ToString() : default_value;
        }
        private static int ReadInt(string key, int default_value)
        {
            return ConfigurationManager.AppSettings[key] != null ? Convert.ToInt16(ConfigurationManager.AppSettings[key].ToString()) : default_value;
        }
        private static double ReadDouble(string key, double default_value)
        {
            return ConfigurationManager.AppSettings[key] != null ? Convert.ToDouble(ConfigurationManager.AppSettings[key].ToString()) : default_value;
        }
        private static bool ReadBool(string key, bool default_value)
        {
            return ConfigurationManager.AppSettings[key] != null ? Boolean.Parse(ConfigurationManager.AppSettings[key].ToString()) : default_value;
        }

        public static string URLBuilder()
        {
            Log.WriteLine("Building map service URL...");
            try
            {
                if (_layer != "")
                    _layer += ":";   //setting found
                else
                    _layer = "";     //use the default setting

                _version = ReadString("WMSVersion", "");
                _service = ReadString("SERVICE", "");
                _request = ReadString("Request", "");
                _srs = ReadString("SRS", "");
                _format = ReadString("FORMAT", "");
                _dpi = ReadString("DPI", "");
                _map_resolution = ReadString("MAP_RESOLUTION", "");
                _format_option = ReadString("FORMAT_OPTIONS", "");
                _transparent = ReadString("TRANSPARENT", "");
                _styles = ReadString("STYLES", "");

                _wmts = ReadBool(_layer + "WMTS", false);

                if (_wmts)
                {
                    _username = ReadString(_layer + "username", "");
                    _password = ReadString(_layer + "password", "");
                    _zoomLevel = ReadInt(_layer + "zoom-level", 0);
                }

                //if (ReadString("URL", "") != "")
                {
                    _url = ReadString(_layer + "URL", "");
                    _width = ReadString(_layer + "Width", "");
                    _height = ReadString(_layer + "Height", "");
                    _meterWidth = ReadDouble(_layer + "WIDTHSpatialUnit", 0);
                    _meterHeight = ReadDouble(_layer + "HEIGHTSpatialUnit", 0);
                    _srid = ReadString(_layer + "srid", "");
                    _resize = ReadInt(_layer + "Resize", 1);
                }

                if (_wmts)
                {
                    if (_layer.Contains(":")) _layer = _layer.Substring(0, layer.Length - 1);
                    return _targetURL = _url + _layer + "-png-" + _zoomLevel.ToString(); 
                }
                else
                {
                    string e, n, s, w;
                    e = n = w = s = "";
                    _bBOXE = (double.Parse(_bBOXW) + _meterWidth).ToString();
                    _bBOXS = (double.Parse(_bBOXN) - _meterHeight).ToString();

                    if (_bBOXE != null && _bBOXN != null && _bBOXS != null && _bBOXW != null)
                    { s = _bBOXS; n = _bBOXN; w = _bBOXW; e = _bBOXE; }

                    if (_layer.Contains(":")) _layer = _layer.Substring(0, layer.Length - 1);
                    
                    return _targetURL = _url.Trim() + "&&SERVICE=" + _service.Trim() + "&VERSION=" + _version.Trim() + "&REQUEST=" + _request.Trim() + "&BBOX=" + w.Trim() + "," +
                             s.Trim() + "," + e.Trim() + "," + n.Trim() + "&SRS=" + _srs.Trim() + "&WIDTH=" + _width.Trim() + "&HEIGHT=" + _height.Trim() + "&LAYERS=" + _layer.Trim() +
                             "&STYLES=" + _styles.Trim() + "&FORMAT=" + _format.Trim() + "&DPI=" + _dpi.Trim() + "&MAP_RESOLUTION=" + _map_resolution.Trim() + "&FORMAT_OPTIONS=" + _format_option.Trim() +
                             "&TRANSPARENT=" + _transparent.Trim();
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                Log.WriteLine(e.Source);
                Log.WriteLine(e.StackTrace);
                throw;
            }
        }

        public static string srid
        {
            get { return MapServerParameters._srid; }
            set { MapServerParameters._srid = value; }
        }
        public static int Resize
        { get { return _resize; } }
        public static double HeightMeterScale
        {
            get { return _meterHeight; }
        }
        public static double WidthMeterScale
        {
            get { return _meterWidth; }
        }
        public static string Username
        { get { return _username; } }
        public static string Password
        { get { return _password; } }
        public static int ZoomLevel
        {
            get { return _zoomLevel; }
            set { _zoomLevel = value; }
        }
        public static Boolean WMTS
        { get { return _wmts; } }
        public static double yscale
        {
            get
            {
                if (_wmts)
                    return _yscale;// = (double)((double)(_meterHeight*2) / (Math.Pow(2, _zoomLevel)) / 256);
                return _yscale = ((double)(_meterHeight) / int.Parse(_height));
            }
            set { _yscale = value; }
        }
        public static double xscale
        {
            get
            {
                if (_wmts)
                    return _xscale;// = (double)((double)(_meterHeight*2) / (Math.Pow(2, _zoomLevel)) / 256);
                return _xscale = ((double)(_meterWidth) / int.Parse(_width));
            }
            set { _xscale = value; }
        }
        public static string targetURL
        { get { return _targetURL; } }
        public static string styles
        { get { return _styles; } }
        public static string URL
        { get { return _url; } }
        public static string service
        { get { return _service; } }
        public static string version
        { get { return _version; } }
        public static string request
        { get { return _request; } }
        public static string BBOXW
        {
            get { return _bBOXW; }
            set
            {
                _bBOXW = value;
                _bBOXE = (double.Parse(_bBOXW) + _meterWidth).ToString();
            }
        }
        public static string BBOXE
        {
            get { return _bBOXE; }
            set
            {
                _bBOXE = (double.Parse(_bBOXW) + _meterWidth).ToString();

            }
        }
        public static string BBOXN
        {
            get { return _bBOXN; }
            set
            {
                _bBOXN = value;
                _bBOXS = (double.Parse(_bBOXN) - _meterHeight).ToString();
            }
        }
        public static string BBOXS
        {
            get { return (double.Parse(_bBOXN) - _meterHeight).ToString(); }
            set { _bBOXS = value; }
        }
        public static string srs
        { get { return _srs; } }
        public static string width
        { get { return _width; } }
        public static string height
        { get { return _height; } }
        public static string layer
        {
            get { return _layer; }
            set { _layer = value; }
        }
        public static string format
        { get { return _format; } }
        public static string dpi
        { get { return _dpi; } }
        public static string map_Resolution
        { get { return _map_resolution; } }
        public static string format_option
        { get { return _format_option; } }
        public static string transparent
        { get { return _transparent; } }
    }
}
