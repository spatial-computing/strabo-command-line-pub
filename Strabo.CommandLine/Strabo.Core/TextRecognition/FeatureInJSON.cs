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

using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Narges
/// </summary>
namespace Strabo.Core.TextRecognition
{

    public struct FieldAliases
    {
        private string oBJECTID;
        private string text;
        private string char_count;
        private string orientation;
        private string filename;
        private string susp_text;
        private string susp_char_count;
        private string mass_centerX;
        private string mass_centerY;
        private string detectionCost;

        public string DetectionCost
        {
            get { return detectionCost; }
            set { detectionCost = value; }
        }

        public string OBJECTID
        {
            get
            {
                return oBJECTID;
            }
            set
            {
                oBJECTID = value;
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        public string Char_count
        {
            get
            {
                return char_count;
            }
            set
            {
                char_count = value;
            }
        }
        public string Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }
        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }
        public string Susp_text
        {
            get
            {
                return susp_text;
            }
            set
            {
                susp_text = value;
            }
        }
        public string Susp_char_count
        {
            get
            {
                return susp_char_count;
            }
            set
            {
                susp_char_count = value;
            }
        }
        public string Mass_centerX
        {
            get
            {
                return mass_centerX;
            }
            set
            {
                mass_centerX = value;
            }
        }
        public string Mass_centerY
        {
            get
            {
                return mass_centerY;
            }
            set
            {
                mass_centerY = value;
            }
        }

    }

    public struct Spatial_Reference
    {
        private int wk_id;
        private int latestWk_id;


        public int wkid
        {
            get
            {
                return wk_id;
            }
            set
            {
                wk_id = value;
            }
        }
        public int latestWkid
        {
            get
            {
                return latestWk_id;
            }
            set
            {
                latestWk_id = value;
            }
        }
    }
    public struct Field_info
    {
        private string vname;
        private string vtype;
        private string valias;
        private int vlength;


        public string name
        {
            get
            {
                return vname;
            }
            set
            {
                vname = value;
            }
        }
        public string type
        {
            get
            {
                return vtype;
            }
            set
            {
                vtype = value;
            }
        }
        public string alias
        {
            get
            {
                return valias;
            }
            set
            {
                valias = value;
            }
        }
        public int length
        {
            get
            {
                return vlength;
            }
            set
            {
                vlength = value;
            }
        }
    }

    public class Geometry
    {

        private double[, ,] vrings = new double[1, 5, 2];

        public double[, ,] rings
        {
            get { return vrings; }
            set { vrings = value; }
        }
    }
    public class Attributes
    {
        private int oBJECTID;

        public int OBJECTID
        {
            get { return oBJECTID; }
            set { oBJECTID = value; }
        }
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        private int char_count;

        public int Char_count
        {
            get { return char_count; }
            set { char_count = value; }
        }
        private double orientation;

        public double Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }
        private string susp_text;

        public string Susp_text
        {
            get { return susp_text; }
            set { susp_text = value; }
        }
        private int susp_char_count;

        public int Susp_char_count
        {
            get { return susp_char_count; }
            set { susp_char_count = value; }
        }
        private double mass_centerX;

        public double Mass_centerX
        {
            get { return mass_centerX; }
            set { mass_centerX = value; }
        }
        private double mass_centerY;

        public double Mass_centerY
        {
            get { return mass_centerY; }
            set { mass_centerY = value; }
        }
        private float detectionCost;

        public float DetectionCost
        {
            get { return detectionCost; }
            set { detectionCost = value; }
        }
    }
    public class Features
    {

        private Geometry vgeometry = new Geometry();

        public Geometry geometry
        {
            get { return vgeometry; }
            set { vgeometry = value; }
        }
        private Attributes vattributes = new Attributes();

        public Attributes attributes
        {
            get { return vattributes; }
            set { vattributes = value; }
        }


    }
    public class FeatureInJSON
    {
        private string vdisplayFieldName;

        public string displayFieldName
        {
            get { return vdisplayFieldName; }
            set { vdisplayFieldName = value; }
        }
        private string vgeometryType;

        public string geometryType
        {
            get { return vgeometryType; }
            set { vgeometryType = value; }
        }
        public FieldAliases fieldAliases = new FieldAliases();
        public Spatial_Reference spatialReference = new Spatial_Reference();
        public Field_info[] fields = new Field_info[10];
        public List<Features> features = new List<Features>();


    }

    public class Geojson
    {
        private FeatureInJSON _featureInJson;

        public FeatureInJSON featureInJson { get { return _featureInJson; } set { _featureInJson = value; } }
        public void writeJsonFile(string path)
        {
            string json = JsonConvert.SerializeObject(featureInJson);
            System.IO.File.WriteAllText(path, json);
        }
        public FeatureInJSON readGeoJsonFile(string path)
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<FeatureInJSON>(json);

        }
    }
}
