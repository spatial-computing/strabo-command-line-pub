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
using Strabo.Core.TextDetection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Strabo.Core.Worker
{
    /// <summary>
    /// Sima
    /// </summary>
    public class MergeTextStrings
    {
        public List<TextString> text_string_list = new List<TextString>();

        public MergeTextStrings() { }
        public void WriteBMP(string output_path)
        {
            for (int i = 0; i < text_string_list.Count; i++)
            {
                if (text_string_list[i].char_list.Count == 1)
                    continue;
                int x = text_string_list[i].mass_center.X;
                int y = text_string_list[i].mass_center.Y;

                for (int s = 0; s < text_string_list[i].orientation_list.Count; s++)
                {
                    string slope = Convert.ToInt16(text_string_list[i].orientation_list[s]).ToString();
                    if (slope == "360") // rotated 360 degress is 0 degree...
                        continue;
                   
                    ImageStitcher imgstitcher1 = new ImageStitcher();
                    using (Bitmap single_img = imgstitcher1.ExpandCanvas(text_string_list[i].rotated_img_list[s], 20))
                    {
                        string fn = i + "_p_" + text_string_list[i].char_list.Count + "_" + x + "_" + y + "_s_" + slope + "_" + text_string_list[i].bbx.X + "_" + text_string_list[i].bbx.Y + "_" + text_string_list[i].bbx.Width + "_" + text_string_list[i].bbx.Height+".png";
                        single_img.Save(Path.Combine (output_path,fn));
                    }
                }
                //write original orientation
                ImageStitcher imgstitcher2 = new ImageStitcher();
                using (Bitmap srcimg = imgstitcher2.ExpandCanvas(text_string_list[i].srcimg, 20))
                {
                    string fn = i + "_p_" + text_string_list[i].char_list.Count + "_" + x + "_" + y + "_s_0" + "_" + text_string_list[i].bbx.X + "_" + text_string_list[i].bbx.Y + "_" + text_string_list[i].bbx.Width + "_" + text_string_list[i].bbx.Height + ".png";
                    srcimg.Save(Path.Combine(output_path, fn));
                }
            }
        }
        public void AddTextString(TextString dstString)
        {
            Update(ref dstString);
            List<int> insert_idx_list = new List<int>();// insert = -1;
            int[] matched_idx_array = new int[dstString.char_list.Count];
            for(int i=0;i<matched_idx_array.Length;i++)
                matched_idx_array[i] = -1;
            int[] matched_char_blob_count = new int [text_string_list.Count];
            for (int i = 0; i < text_string_list.Count; i++)
            {
                TextString srcString = text_string_list[i];
                for(int x=0;x<srcString.char_list.Count;x++)
                {
                    for(int y=0;y<dstString.char_list.Count;y++)
                    {
                        if(Distance(srcString.char_list[x].mass_center,dstString.char_list[y].mass_center)<3)
                        {
                            matched_idx_array[y] = i;
                            matched_char_blob_count[i]++;
                        }
                    }
                      //if(insert!=-1)
                            //break;
                }
            }
            int max_matched = 0;
            int max_matched_idx = -1;
            for (int i = 0; i < matched_char_blob_count.Length; i++)
            {
                if (matched_char_blob_count[i] > max_matched)
                {
                    max_matched = matched_char_blob_count[i];
                    max_matched_idx = i;
                }
            }
            int insert = max_matched_idx;
            if (insert == -1) text_string_list.Add(dstString);
            else
            {
                List<int> remove_string_list = new List<int>();
                for (int i = 0; i < matched_idx_array.Length ; i++)
                {
                    if (matched_idx_array[i] != -1 && matched_idx_array[i] != insert)
                        remove_string_list.Add(i);
                }
                Point p1  =  new Point(text_string_list[insert].bbx.X,text_string_list[insert].bbx.Y);
                Point p2 = new Point(dstString.bbx.X, dstString.bbx.Y);
                for (int c = 0; c < dstString.char_list.Count; c++)
                    text_string_list[insert].AddChar(dstString.char_list[c]);

                Bitmap newimg1 = new Bitmap(text_string_list[insert].bbx.Width, text_string_list[insert].bbx.Height);
                Bitmap newimg2 = new Bitmap(text_string_list[insert].bbx.Width, text_string_list[insert].bbx.Height);
                Graphics g1 = Graphics.FromImage(newimg1);
                g1.Clear(Color.White);
                g1.DrawImage(text_string_list[insert].srcimg,
                    p1.X - text_string_list[insert].bbx.X,
                    p1.Y - text_string_list[insert].bbx.Y);

                Graphics g2 = Graphics.FromImage(newimg2);
                g2.Clear(Color.White);
                g2.DrawImage(dstString.srcimg,
                    p2.X - text_string_list[insert].bbx.X,
                    p2.Y - text_string_list[insert].bbx.Y);
                g1.Dispose(); g2.Dispose();
                
                //ASHISH
                newimg1 = ImageUtils.GetIntersection(newimg1, newimg2);

                text_string_list[insert].srcimg = newimg1;

                //for (int i = 0; i < remove_string_list.Count; i++)
                //{

                //}
            }
        } 
        public double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        public void Update(ref TextString srcString)
        {
            MyConnectedComponentsAnalysisFast.MyBlob char_blob0 = srcString.char_list[0];
            char_blob0.mass_center.X += srcString.x_offset;
            char_blob0.mass_center.Y += srcString.y_offset;
            char_blob0.bbx.X += srcString.x_offset;
            char_blob0.bbx.Y += srcString.y_offset;
            Rectangle bbx = char_blob0.bbx;

            for (int i = 1; i < srcString.char_list.Count; i++)
            {
                MyConnectedComponentsAnalysisFast.MyBlob char_blob = srcString.char_list[i];
                char_blob.mass_center.X += srcString.x_offset;
                char_blob.mass_center.Y += srcString.y_offset;
                char_blob.bbx.X += srcString.x_offset;
                char_blob.bbx.Y += srcString.y_offset;

                int x = bbx.X, y = bbx.Y, xx = bbx.X + bbx.Width - 1, yy = bbx.Y + bbx.Height - 1;
                int x1 = char_blob.bbx.X, y1 = char_blob.bbx.Y, xx1 = char_blob.bbx.X + char_blob.bbx.Width - 1, yy1 = char_blob.bbx.Y + char_blob.bbx.Height - 1;

                int x2, y2, xx2, yy2;

                if (x < x1) x2 = x;
                else x2 = x1;
                if (y < y1) y2 = y;
                else y2 = y1;

                if (xx < xx1) xx2 = xx1;
                else xx2 = xx;
                if (yy < yy1) yy2 = yy1;
                else yy2 = yy;

                bbx.X = x2; bbx.Y = y2;
                bbx.Width = xx2 - x2 + 1;
                bbx.Height = yy2 - y2 + 1;
            }
            srcString.bbx = bbx;
        }
    }
}
