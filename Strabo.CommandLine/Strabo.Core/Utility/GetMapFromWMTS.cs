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
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Strabo.Core.Utility
{
    public class Tile
    {
        public double westCorner, northCorner;
        public int column, row;
    }

    public class GetMapFromWMTS
    {
        private double _wmtsCoverageWestCorner, _wmtsCoverageNorthCorner;
        private int _zoomLayer;
        private string _username;
        private string _password;
        private int _tile_size=256;
        private Tile _tile;

        private string _url;

        public GetMapFromWMTS(string URL, string username, string password, double west, double north, int zoomLayer)
        {
            _tile = new Tile();
            _password = password;
            _username = username;
            _zoomLayer = zoomLayer;
            _wmtsCoverageWestCorner = west;
            _wmtsCoverageNorthCorner = north;
            _zoomLayer = zoomLayer;
            _url = URL;
        }
        public Tile Apply(string intermediatePath, string output_file_name )
        {
            double mapWestCorner = double.Parse(MapServerParameters.BBOXW);
            double mapNorthCorner = double.Parse(MapServerParameters.BBOXN);
            string wmsMapPath = intermediatePath + output_file_name;
            int layer = MapServerParameters.ZoomLevel;

            Bitmap SourceImage = GetImage(mapWestCorner, mapNorthCorner, intermediatePath);
            SourceImage.Save(wmsMapPath);
            ColorHistogram ch = new ColorHistogram();

            if (!ch.CheckImageColors(SourceImage, 2))
            {
                SourceImage.Dispose();
                Exception e = new Exception("The returned image is invalid, it has less than two colors.");
                Log.WriteLine("The returned image is invalid, it has less than two colors.");
                throw e;
            }
            SourceImage.Dispose();
            return _tile;
        }

        private Bitmap GetImage(double mapWestCorner, double mapNorthCorner, string path)
        {
            Tile tile = GetTile(mapWestCorner, mapNorthCorner);//GetTile(WestCorner, northCorner);
            MapServerParameters.BBOXN = tile.northCorner.ToString();
            MapServerParameters.BBOXW = tile.westCorner.ToString();
            string connectionString = _url + "-" + tile.column.ToString() + "-" + tile.row.ToString();
            Bitmap image = ConnectToMapServer.ConnectToWMTS(connectionString, _username, _password);

            int outputImageWidth = image.Width * 4 + 1;
            int outputImageHeight = image.Height * 4 + 1;
            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, outputImage.Width, outputImage.Height);
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        connectionString = _url + "-" + (tile.column + j).ToString() + "-" + (tile.row + i).ToString();
                        image = ConnectToMapServer.ConnectToWMTS(connectionString, _username, _password);
                        image.Save(path + "part" + i.ToString() + "-" + j.ToString() + ".png");
                        graphics.DrawImage(image, new Rectangle(new Point(j * (image.Width), i * (image.Height)), image.Size),
                         new Rectangle(new Point(), image.Size), GraphicsUnit.Pixel);
                    }
                }
            }
            _tile = tile;
            return outputImage;
        }

        private Tile GetTile(double westCorner, double northCorner)
        {
            PointF pf = MeterToTilePos(westCorner, northCorner, _zoomLayer);
            Tile tile = new Tile();
            tile.column = Convert.ToInt32(pf.X);
            tile.row = Convert.ToInt32(pf.Y);
            pf = TileToMeterPos(tile.row, tile.column, _zoomLayer);
            tile.westCorner = pf.X;
            tile.northCorner = pf.Y;

            pf = TileToMeterPos(tile.row + 4, tile.column + 4, _zoomLayer);
            double img_height = (double)(_tile_size * 4 * MapServerParameters.Resize + 2);
            MapServerParameters.yscale = Math.Abs(pf.Y - tile.northCorner) / img_height;
            MapServerParameters.xscale = Math.Abs(pf.X - tile.westCorner) / img_height;

            return tile;
        }
        public PointF MeterToTilePos(double x, double y, int zoom)
        {
            PointF p = new Point();
            int count = Convert.ToInt32(Math.Pow(2, _zoomLayer));
            p.X = (float)((x - _wmtsCoverageWestCorner) / (Math.Abs(_wmtsCoverageWestCorner) * 2) * count);
            p.Y = (float)((_wmtsCoverageNorthCorner - y) / (Math.Abs(_wmtsCoverageNorthCorner) * 2) * count);
            return p;
        }

        public PointF TileToMeterPos(double tile_y, double tile_x, int zoom)
        {
            PointF p = new Point();
            p.X = (float)(tile_x / Math.Pow(2.0, _zoomLayer) * Math.Abs(_wmtsCoverageWestCorner) * 2 + _wmtsCoverageWestCorner);
            p.Y = (float)(_wmtsCoverageNorthCorner - tile_y / Math.Pow(2.0, _zoomLayer) * Math.Abs(_wmtsCoverageNorthCorner) * 2);
            return p;
        }
    }
}
