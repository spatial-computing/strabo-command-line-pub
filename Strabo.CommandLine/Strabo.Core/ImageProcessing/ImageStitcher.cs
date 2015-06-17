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

using System.Collections.Generic;
using System.Drawing;

namespace Strabo.Core.ImageProcessing
{
    /// <summary>
    /// Narges
    /// </summary>
    public class ImageStitcher
    {
        public int cell_width=-1, cell_height=-1;
        public int cols, rows;
        public ImageStitcher() { }
        public Bitmap ExpandCanvas(Bitmap srcimg, int size)
        {
            Bitmap string_img = new Bitmap(srcimg.Width + size*2, srcimg.Height + size*2);
            Graphics g = Graphics.FromImage(string_img);
            g.Clear(Color.White);
            g.DrawImage(srcimg, new Point(size, size));
            g.Dispose();
            return string_img;
        }
        public Bitmap ApplyWithoutGridLines(List<Bitmap> srcimg_list, int max_width)
        {
            for (int i = 0; i < srcimg_list.Count; i++)
            {
                if (cell_height < srcimg_list[i].Height)
                    cell_height = srcimg_list[i].Height;
                if (cell_width < srcimg_list[i].Width)
                    cell_width = srcimg_list[i].Width;
            }

            cell_width += 10;
            cell_height += 10;

            cols = max_width / cell_width;
            rows = srcimg_list.Count / cols + 1;
            int image_width = cols * cell_width;
            int image_height = rows * cell_height;
            Bitmap string_img = new Bitmap(image_width, image_height);
            Graphics g = Graphics.FromImage(string_img);
            g.Clear(Color.White);
         
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    if (idx >= srcimg_list.Count) continue;
                    int margin = 4;
                    int w = cell_width - srcimg_list[idx].Width - margin;
                    w /= 2;
                    int h = cell_height - srcimg_list[idx].Height - margin;
                    h /= 2;

                    g.DrawImage(srcimg_list[idx],
                        new Point(c * cell_width + w, r * cell_height + h));
                }
            g.Dispose();
            return string_img;
        }
        public Bitmap Apply(List<Bitmap> srcimg_list, int max_width)
        {
            for (int i = 0; i < srcimg_list.Count; i++)
            {
                if (cell_height < srcimg_list[i].Height)
                    cell_height = srcimg_list[i].Height;
                if (cell_width < srcimg_list[i].Width)
                    cell_width = srcimg_list[i].Width;
            }

            cell_width += 10;
            cell_height += 10;

            cols = max_width / cell_width;
            rows = srcimg_list.Count / cols + 1;
            int image_width = cols * cell_width;
            int image_height = rows * cell_height;
            Bitmap string_img = new Bitmap(image_width, image_height);
            Graphics g = Graphics.FromImage(string_img);
            g.Clear(Color.White);
            for (int r = 0; r < rows; r++)
                g.DrawLine(new Pen(Color.Black, 3), new Point(0, r * cell_height), new Point(image_width, r * cell_height));
            for (int c = 0; c < cols; c++)
                g.DrawLine(new Pen(Color.Black), new Point(c * cell_width, 0), new Point(c * cell_width, image_height));

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    if (idx >= srcimg_list.Count) continue;
                    int margin = 4;
                    int w = cell_width - srcimg_list[idx].Width - margin;
                    w /= 2;
                    int h = cell_height - srcimg_list[idx].Height - margin;
                    h /= 2;

                    g.DrawImage(srcimg_list[idx],
                        new Point(c * cell_width + w, r * cell_height + h));
                }
            g.Dispose();
            return string_img;
        }
        public Bitmap ApplyAtLeastTwoRows(List<Bitmap> srcimg_list, int max_width)
        {
            for (int i = 0; i < srcimg_list.Count; i++)
            {
                if (cell_height < srcimg_list[i].Height)
                    cell_height = srcimg_list[i].Height;
                if (cell_width < srcimg_list[i].Width)
                    cell_width = srcimg_list[i].Width;
            }

            cell_width += 50;
            cell_height += 50;

            cols = srcimg_list.Count+1;// max_width / cell_width;
            rows = 2;// srcimg_list.Count / cols + 1;
            int image_width = cols * cell_width;
            int image_height = rows * cell_height;
            Bitmap string_img = new Bitmap(image_width, image_height);
            Graphics g = Graphics.FromImage(string_img);
            g.Clear(Color.White);
            for (int r = 0; r < rows; r++)
                g.DrawLine(new Pen(Color.Black,3), new Point(0, r * cell_height), new Point(image_width, r * cell_height));
            for (int c = 0; c < cols; c++)
                g.DrawLine(new Pen(Color.Black), new Point(c * cell_width, 0), new Point(c * cell_width, image_height));
            g.DrawRectangle(new Pen(Color.Black,3), new Rectangle(0,0,image_width,image_height));
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    if (idx >= srcimg_list.Count) continue;
                    int margin = 30;
                    int w = cell_width - srcimg_list[idx].Width - margin;
                    w /= 2;
                    int h = cell_height - srcimg_list[idx].Height - margin;
                    h /= 2;

                    g.DrawImage(srcimg_list[idx],
                        new Point(c * cell_width + w, r * cell_height + h));
                }
            g.Dispose();
            return string_img;
        }
    }
}
