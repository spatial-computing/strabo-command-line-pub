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
using System.Linq;
using System.Text;

namespace Strabo.Core.Utility
{

    public static class WMSParameters
    {


        public static string URLBuilder()
        {
            _version = (ReadConfigFile.ReadModelConfiguration("Version") != "") ? ReadConfigFile.ReadModelConfiguration("Version") : "";
            _service = (ReadConfigFile.ReadModelConfiguration("SERVICE") != "") ? ReadConfigFile.ReadModelConfiguration("SERVICE") : "";
            _request = (ReadConfigFile.ReadModelConfiguration("Request") != "") ? ReadConfigFile.ReadModelConfiguration("Request") : "";
            _srs = (ReadConfigFile.ReadModelConfiguration("SRS") != "") ? ReadConfigFile.ReadModelConfiguration("SRS") : "";
            _format = (ReadConfigFile.ReadModelConfiguration("FORMAT") != "") ? ReadConfigFile.ReadModelConfiguration("FORMAT") : "";
            _dpi = (ReadConfigFile.ReadModelConfiguration("DPI") != "") ? ReadConfigFile.ReadModelConfiguration("DPI") : "";
            _map_resolution = (ReadConfigFile.ReadModelConfiguration("MAP_RESOLUTION") != "") ? ReadConfigFile.ReadModelConfiguration("MAP_RESOLUTION") : "";
            _format_option = (ReadConfigFile.ReadModelConfiguration("FORMAT_OPTIONS") != "") ? ReadConfigFile.ReadModelConfiguration("FORMAT_OPTIONS") : "";
            _transparent = (ReadConfigFile.ReadModelConfiguration("TRANSPARENT") != "") ? ReadConfigFile.ReadModelConfiguration("TRANSPARENT") : "";
            _styles = (ReadConfigFile.ReadModelConfiguration("STYLES") != "") ? ReadConfigFile.ReadModelConfiguration("STYLES") : "";



            if ((ReadConfigFile.ReadModelConfiguration(_layers + ":URL")) != null)
            {
                _url = (ReadConfigFile.ReadModelConfiguration(_layers + ":URL") != "") ? ReadConfigFile.ReadModelConfiguration(_layers + ":URL") : "";
                _width = (ReadConfigFile.ReadModelConfiguration(_layers + ":Width") != "") ? ReadConfigFile.ReadModelConfiguration(_layers + ":Width") : "";
                _height = (ReadConfigFile.ReadModelConfiguration(_layers + ":Height") != "") ? ReadConfigFile.ReadModelConfiguration(_layers + ":Height") : "";
                _meterWidth = (ReadConfigFile.ReadModelConfiguration(_layers + ":WIDTHMeter") != "") ? int.Parse(ReadConfigFile.ReadModelConfiguration(_layers + ":WIDTHMeter")) : 0;
                _meterHeight = (ReadConfigFile.ReadModelConfiguration(_layers + ":HEIGHTMeter") != "") ? int.Parse(ReadConfigFile.ReadModelConfiguration(_layers + ":HEIGHTMeter")) : 0;
                _srid = (ReadConfigFile.ReadModelConfiguration(_layers + ":srid") != "") ? (ReadConfigFile.ReadModelConfiguration(_layers + ":srid")) : "";
            }
            else
            {
                _url = (ReadConfigFile.ReadModelConfiguration("URL") != "") ? ReadConfigFile.ReadModelConfiguration("URL") : "";
                _width = (ReadConfigFile.ReadModelConfiguration("WIDTH") != "") ? ReadConfigFile.ReadModelConfiguration("WIDTH") : "";
                _height = (ReadConfigFile.ReadModelConfiguration("HEIGHT") != "") ? ReadConfigFile.ReadModelConfiguration("HEIGHT") : "";
                _meterWidth = (ReadConfigFile.ReadModelConfiguration("WIDTHMeter") != "") ? int.Parse(ReadConfigFile.ReadModelConfiguration("WIDTHMeter")) : 0;
                _meterHeight = (ReadConfigFile.ReadModelConfiguration("HEIGHTMeter") != "") ? int.Parse(ReadConfigFile.ReadModelConfiguration("HEIGHTMeter")) : 0;
                _srid = (ReadConfigFile.ReadModelConfiguration("srid") != "") ? (ReadConfigFile.ReadModelConfiguration("srid")) : "";

            }


            string e, n, s, w;
            e = n = w = s = "";
            _bBOXE = (Int32.Parse(_bBOXW) + _meterWidth).ToString();
            _bBOXS = (Int32.Parse(_bBOXN) - _meterHeight).ToString();

            if (_bBOXE != null && _bBOXN != null && _bBOXS != null && _bBOXW != null)
            { s = _bBOXS; n = _bBOXN; w = _bBOXW; e = _bBOXE; }


            return _targetURL = _url.Trim() + "&&SERVICE=" + _service.Trim() + "&VERSION=" + _version.Trim() + "&REQUEST=" + _request.Trim() + "&BBOX=" +  w.Trim()+ "," +
                     s.Trim() + "," + e.Trim() + "," + n.Trim() + "&SRS=" + _srs.Trim() + "&WIDTH=" + _width.Trim() + "&HEIGHT=" + _height.Trim() + "&LAYERS=" + _layers.Trim() +
                     "&STYLES=" + _styles.Trim() + "&FORMAT=" + _format.Trim() + "&DPI=" + _dpi.Trim() + "&MAP_RESOLUTION=" + _map_resolution.Trim() + "&FORMAT_OPTIONS=" + _format_option.Trim() +
                     "&TRANSPARENT=" + _transparent.Trim();
        }
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
        private static string _layers;
        private static string _format;
        private static string _dpi;
        private static string _map_resolution;
        private static string _format_option;
        private static string _transparent;
        private static string _styles;
        private static double _scale;
        private static string _srid;

        public static string srid
        {
            get { return WMSParameters._srid; }
            set { WMSParameters._srid = value; }
        }

        private static int _meterWidth;
        private static int _meterHeight;

        public static int MeterScale
        {
            get { return _meterWidth; }
        }
        public static double scale
        { get { return _scale = ((double)(_meterHeight) / int.Parse(_height)); } }
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
            get { return _bBOXW  ; }
            set { _bBOXW = value; }
        }
        public static string BBOXE
        {
            get { return _bBOXE; }
            set
            {
                _bBOXE =(Int32.Parse(_bBOXW) + _meterWidth).ToString();;
                
            }
        }
        public static string BBOXN
        {
            get { return _bBOXN; }
            set
            {
                _bBOXN = value;
                _bBOXS = (Int32.Parse(_bBOXN) - _meterHeight).ToString();
            }
        }
        public static string BBOXS
        {
            get { return (Int32.Parse(_bBOXN) - _meterHeight).ToString(); }
            set { _bBOXS = value; }
        }
        public static string srs
        { get { return _srs; } }
        public static string width
        { get { return _width; } }
        public static string height
        { get { return _height; } }
        public static string layers
        {
            get { return _layers; }
            set { _layers = value; }
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
