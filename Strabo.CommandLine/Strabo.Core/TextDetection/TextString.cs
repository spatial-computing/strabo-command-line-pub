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

namespace Strabo.Core.TextDetection
{
    /// <summary>
    /// Sima
    /// </summary>
    public class TextString
    {
        private bool _debug = true;
        private Rectangle _bbx = new Rectangle(0, 0, 0, 0);
        private double _mean_width = 0;
        private double _mean_height = 0;
        private bool _net = false;
        private List<MyConnectedComponentsAnalysisFast.MyBlob> _char_list = new List<MyConnectedComponentsAnalysisFast.MyBlob>();
        private List<TextString> _final_string_list = new List<TextString>();
        private bool _needsplit = false;
        private int _smaller_angle_threshold;
        private int _larger_angle_threshold;
        private Bitmap _srcimg;
        private List<Bitmap> _rotated_img_list = new List<Bitmap>();
        private List<double> _orientation_list = new List<double>();
        private List<string> _recognized_text_list = new List<string>();
        private int _x_offset = 0;
        private int _y_offset = 0;
        private Point _mass_center;
        public TextString() { }
        private double ShortestDistance(MyConnectedComponentsAnalysisFast.MyBlob char_blob)
        {
            double min_distance = Double.MaxValue;
            Point a = char_blob.mass_center;
            for (int i = 0; i < _char_list.Count; i++)
            {
                Point b = _char_list[i].mass_center;
                double distance = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
                if (distance < min_distance)
                    min_distance = distance;
            }
            return min_distance;
        }
        private double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        public  void AddChar(MyConnectedComponentsAnalysisFast.MyBlob char_blob)
        {
            if (_char_list.Contains(char_blob)) return;
            for (int i = 0; i < _char_list.Count; i++)
                if (Distance(_char_list[i].mass_center, char_blob.mass_center) < 3)
                    return;
            _mean_height = (_mean_height * _char_list.Count + char_blob.bbx.Height);
            _mean_width = (_mean_width * _char_list.Count + char_blob.bbx.Width);
            _char_list.Add(char_blob);
            _mean_height /= (double)_char_list.Count;
            _mean_width /= (double)_char_list.Count;
            // Extend bbx
            if (_bbx.Width == 0)
                _bbx = char_blob.bbx;
            else
            {
                int x = _bbx.X, y = _bbx.Y, xx = _bbx.X + _bbx.Width - 1, yy = _bbx.Y + _bbx.Height - 1;
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

                _bbx.X = x2; _bbx.Y = y2;
                _bbx.Width = xx2 - x2 + 1;
                _bbx.Height = yy2 - y2 + 1;

            }


        }
        private int GetLargeCharCount()
        {
            int c = 0;
            for (int i = 0; i < _char_list.Count; i++)
                if (_char_list[i].sizefilter_included)
                    c++;
            return c;
        }
        private void BBXConnect(int min_dist, int char_width, int[] labels, int label_width, int label_height)
        {
            int char_count = _char_list.Count;

            for (int i = 0; i < char_count; i++)
            {
                if (_char_list[i].bbx.Width < char_width / 2 && _char_list[i].bbx.Height < char_width / 2) continue;
                for (int j = i + 1; j < char_count; j++)
                {

                    if (_char_list[j].bbx.Width < char_width / 2 && _char_list[j].bbx.Height < char_width / 2) continue;

                    int x1 = _char_list[i].bbx.X;
                    int y1 = _char_list[i].bbx.Y;
                    bool overlap = false;
                    if (_char_list[i].bbx.IntersectsWith(_char_list[j].bbx))
                        overlap = true;
                    else
                    {
                        Rectangle rect = new Rectangle(x1 - min_dist, y1 - min_dist, _char_list[i].bbx.Width + min_dist * 2, _char_list[i].bbx.Height + min_dist * 2);
                        if (rect.IntersectsWith(_char_list[j].bbx))
                            overlap = true;
                    }

                    if (overlap)//(dist <= min_dist)
                    {
                        _char_list[i].neighbors.Add(j);
                        _char_list[j].neighbors.Add(i);
                        _char_list[i].neighbor_count++;
                        _char_list[j].neighbor_count++;
                    }
                }
            }
            for (int i = 0; i < char_count; i++)
            {
                if (_char_list[i].neighbors.Count > 2) _needsplit = true;
                if (_debug)
                {
                    Console.Write("NE: " + i + "--- ");

                    for (int j = 0; j < _char_list[i].neighbors.Count; j++)
                        Console.Write(_char_list[i].neighbors[j] + " ");
                    Console.WriteLine();
                }
            }
        }
        private void Split(int min_dist, int char_width, int[] labels, int label_width, int label_height, int smaller_angle_threshold, int larger_angle_threshold)
        {
            this._smaller_angle_threshold = smaller_angle_threshold;
            this._larger_angle_threshold = larger_angle_threshold;

            int cc = 0;
            for (int i = 0; i < _char_list.Count; i++)
            {
                if (_char_list[i].bbx.Width < char_width / 2 && _char_list[i].bbx.Height < char_width / 2)
                    _char_list[i].sizefilter_included = false;
                else cc++;
            }
            // If the string has fewer than 3 CCs (non-small CCs), mark as "short string"
            if (cc <= 3) { Convert2FinalStringList(); return; }

            // Applying distance transformation (distance between characters)
            Connect(min_dist, char_width, labels, label_width, label_height);
            // Checking for "net" structure
            int connecting_node_count = 0;
            for (int i = 0; i < _char_list.Count; i++)
            {
                if (_char_list[i].neighbors.Count > 2)
                    connecting_node_count++;
                if (_debug)
                {
                    Console.Write("NE: " + i + "--- ");
                    for (int j = 0; j < _char_list[i].neighbors.Count; j++)
                        Console.Write(_char_list[i].neighbors[j] + " ");
                    Console.WriteLine();
                }
            }
            if (connecting_node_count > _char_list.Count / 2)
            { _net = true; Convert2FinalStringList(); return; }
            // If no character has more than 2 connections and any set of three connected characters constitutes an acute angle, send the string to break
            if (connecting_node_count == 0)
                BreakIt(smaller_angle_threshold); // loose threshold
            else
                SplitIt();
        }
        private void Connect(int min_dist, int char_width, int[] labels, int label_width, int label_height)
        {
            int char_count = _char_list.Count;
            int mw = _bbx.Width + 2; int mh = _bbx.Height + 2;// 1 pixel border
            List<int[,]> charimg_list = new List<int[,]>();
            List<int[,]> dist_charimg_list = new List<int[,]>();
            DistanceTransformation dist = new DistanceTransformation();
            for (int i = 0; i < char_count; i++)
            {
                int[,] charimg = new int[mh, mw];
                for (int xx = 1; xx < mw - 1; xx++)
                    for (int yy = 1; yy < mh - 1; yy++)
                        if (labels[(yy - 1 + _bbx.Y) * label_width + (xx - 1 + _bbx.X)] == _char_list[i].pixel_id)
                            charimg[yy, xx] = 1; // 1 is fg
                        else
                            charimg[yy, xx] = 0;
                charimg_list.Add(charimg);
                dist_charimg_list.Add(dist.ApplyFGisZero(charimg));
            }
            for (int i = 0; i < char_count; i++)
                for (int j = i + 1; j < char_count; j++)
                {
                    int min = mw * mh;
                    for (int yy = 1; yy < mh - 1; yy++)
                    {
                        int[,] charimg = charimg_list[i];
                        for (int xx = 1; xx < mw - 1; xx++)
                        {
                            int[,] dist_charimg = dist_charimg_list[j];
                            if (charimg[yy, xx] == 1 && min > dist_charimg[yy, xx])
                                min = dist_charimg[yy, xx];
                        }
                    }
                    if (min <= min_dist)
                    {


                        if ((!_char_list[j].sizefilter_included && !_char_list[i].sizefilter_included) || //S S
                            (_char_list[j].sizefilter_included && _char_list[i].sizefilter_included))
                        {
                            _char_list[i].neighbors.Add(j);
                            _char_list[i].neighbor_count++;

                            _char_list[j].neighbors.Add(i);
                            _char_list[j].neighbor_count++;
                        }
                        else
                        {
                            if (!_char_list[i].sizefilter_included) // S B
                            {
                                _char_list[i].neighbors.Add(j);
                                _char_list[i].neighbor_count++;
                            }
                            else
                            {
                                _char_list[j].neighbors.Add(i);
                                _char_list[j].neighbor_count++;
                            }
                        }
                    }
                }
        }
        private void BreakIt(int min_angel)
        {
            List<int> tmp_list4one = new List<int>();
            for (int i = 0; i < _char_list.Count; i++)
            {
                if (_char_list[i].sizefilter_included == false)
                {
                    tmp_list4one.Add(i); continue;
                }
                _char_list[i].split_visited = 1;
                if (_char_list[i].neighbor_count == 2)
                {
                    if (_debug) Console.WriteLine("Break test: Points");
                    double angel = CosAngel(_char_list[i].neighbors[0], _char_list[i].neighbors[1], i);
                    if (_debug) Console.WriteLine("Break test: Idx " + _char_list[i].neighbors[0] + " " + i + " " + _char_list[i].neighbors[1] + " _   " + angel);

                    if (angel < min_angel) // acute
                    {
                        _char_list[i].split_visited++;
                        _char_list[i].split_here = true;
                        if (_debug) Console.WriteLine("Break here @ " + i);
                    }
                    else
                        if (_debug) Console.WriteLine("Break test passed");
                }
            }
            List<List<int>> tmp_list4two = new List<List<int>>();
            for (int i = 0; i < _char_list.Count; i++)
            {
                if (_char_list[i].sizefilter_included == false || _char_list[i].split_visited <= 0 || _char_list[i].split_here == true)
                    continue;
                List<int> substring_list = new List<int>();
                if (_debug) Console.Write(" BreakItTrace: ");
                BreakItTrace(substring_list, i);
                if (_debug) Console.WriteLine();

                if (substring_list.Count == 2)
                {
                    tmp_list4two.Add(substring_list);
                    if (_debug) Console.WriteLine("                 Add tmp2ts:     " + substring_list[0] + "   " + substring_list[1]);
                }
                else
                    AddSubstring(substring_list);
            }
            MergeTwo(tmp_list4two);
            MergeOne(tmp_list4one);
        }
        private void BreakItTrace(List<int> substring_list, int i)
        {
            if (_char_list[i].split_visited <= 0) return;
            if (_debug) Console.Write(i + " ");
            substring_list.Add(i);
            _char_list[i].split_visited--;
            if (_char_list[i].split_here != true)
                for (int j = 0; j < _char_list[i].neighbors.Count; j++)
                    BreakItTrace(substring_list, _char_list[i].neighbors[j]);
        }
        private void MergeOne(List<int> tmp_list4one)
        {
            if (_debug) Console.WriteLine("*******Merge One******");
            for (int i = 0; i < tmp_list4one.Count; i++)
            {
                // for small cc only since small cc still has neighbors
                int idx1 = tmp_list4one[i];
                MyConnectedComponentsAnalysisFast.MyBlob char1 = _char_list[idx1];
                for (int ni = 0; ni < char1.neighbors.Count; ni++)
                {
                    int idx = char1.neighbors[ni];
                    for (int j = 0; j < _final_string_list.Count; j++)
                    {
                        TextString ts = _final_string_list[j];
                        if (ts._char_list.Contains(_char_list[idx]))
                        {
                            _char_list[idx1].included = true;
                            ts.AddChar(_char_list[idx1]);
                            if (_debug) Console.WriteLine("                 Add TS: " + idx1 + " to TS: " + j);
                        }
                    }
                }
                if (_debug) Console.WriteLine(idx1 + " is not a small cc");
                if (!_char_list[idx1].included)
                {
                    _char_list[idx1].included = true;
                    List<int> tmp_list = new List<int>();
                    tmp_list.Add(idx1);
                    AddSubstring(tmp_list);
                }
            }
        }
        private void MergeTwo(List<List<int>> tmp_list4two)
        {
            if (_debug) Console.WriteLine("identify non-connected two CC, add them to TS");
            for (int i = 0; i < tmp_list4two.Count; i++)
            {
                int idx1 = tmp_list4two[i][0];
                int idx2 = tmp_list4two[i][1];
                if (!_char_list[idx1].included && !_char_list[idx2].included)
                {
                    _char_list[idx1].included = true;
                    _char_list[idx2].included = true;
                    AddSubstring(tmp_list4two[i]);
                }
            }
            if (_debug) Console.WriteLine("assign other two cc substring to each connected substring");
            for (int i = 0; i < tmp_list4two.Count; i++)
            {
                int idx1 = tmp_list4two[i][0];
                int idx2 = tmp_list4two[i][1];
                if (!_char_list[idx1].included || !_char_list[idx2].included)
                {
                    int idx = 0;
                    int idx3 = 0;
                    bool addtwo = false;
                    if (_char_list[idx1].included && _char_list[idx3].sizefilter_included == false)
                    { idx = idx1; idx3 = idx2; }
                    else if (_char_list[idx3].included && _char_list[idx1].sizefilter_included == false)
                    { idx = idx2; idx3 = idx1; }
                    else addtwo = true;
                    if (addtwo)
                    {
                        _char_list[idx1].included = true;
                        _char_list[idx2].included = true;
                        AddSubstring(tmp_list4two[i]);
                    }
                    else
                    {
                        for (int j = 0; j < _final_string_list.Count; j++)
                        {
                            TextString ts = _final_string_list[j];
                            if (ts._char_list.Contains(_char_list[idx]))
                            {
                                ts.AddChar(_char_list[idx3]);
                                _char_list[idx3].included = true;
                                if (_debug) Console.WriteLine("Add: " + idx3 + " to " + idx + "\'ts");
                            }
                        }
                    }
                }
                else
                {
                    if (_debug) Console.WriteLine(idx1 + "   " + idx2 + "    added already!");

                }
            }
        }
        private void TraceCharacters(List<int> substring_list, int p1, int p3, int min_angle)
        {
            if (_char_list[p3].neighbors.Count <= 0) return;

            double max_angle = -1;
            int max_angle_idx = 0;

            if (_debug) Console.WriteLine("*****Trace:Char " + p1 + " and " + p3);
            MyConnectedComponentsAnalysisFast.MyBlob char1 = _char_list[p3];
            for (int i = 0; i < char1.neighbors.Count; i++)
            {
                int idx = char1.neighbors[i];
                if (_char_list[idx].sizefilter_included)
                {
                    double angel = CosAngel(p1, idx, p3);
                    if (angel > max_angle) { max_angle = angel; max_angle_idx = idx; }
                }
            }
            if (max_angle_idx >= 0 && max_angle > min_angle)
            {
                substring_list.Add(max_angle_idx);
                _char_list[max_angle_idx].neighbors.Remove(p3);
                _char_list[p3].neighbors.Remove(max_angle_idx);
                if (_debug) Console.WriteLine("     Found: " + max_angle_idx + " Angle: " + max_angle + " Min Angle: " + min_angle);
                if (_char_list[p3].split_here || _char_list[max_angle_idx].split_here)
                    TraceCharacters(substring_list, p3, max_angle_idx, _larger_angle_threshold); // strict
                else TraceCharacters(substring_list, p3, max_angle_idx, _smaller_angle_threshold);


            }
            else
                if (_debug) Console.WriteLine("     Found none: " + max_angle_idx + " Angle: " + max_angle + " Min Angle: " + min_angle);

        }
        private int FindNextNeighbor(int c)
        {
            MyConnectedComponentsAnalysisFast.MyBlob char1 = _char_list[c];
            for (int i = 0; i < char1.neighbors.Count; i++)
            {
                int idx = char1.neighbors[i];
                if (_char_list[idx].sizefilter_included)
                    return idx;
            }
            return -1;
        }
        private void SplitIt()
        {
            if (_debug) Console.WriteLine("************ SplitIt *****************");
            List<int> tmp_list4one = new List<int>();
            List<List<int>> tmp_list4two = new List<List<int>>();
            for (int i = 0; i < _char_list.Count; i++)
                if (_char_list[i].neighbor_count > 2)
                    _char_list[i].split_here = true;
            for (int i = 0; i < _char_list.Count; i++)
            {
                if (_char_list[i].sizefilter_included == false)
                {
                    tmp_list4one.Add(i);
                    if (_debug) Console.WriteLine(" Add " + i + " to one");
                    continue;
                }

                int next = FindNextNeighbor(i);

                if (next == -1 && _char_list[i].included != true) // no neighbor
                {
                    tmp_list4one.Add(i);
                    if (_debug) Console.WriteLine(" Add " + i + " to one");
                    continue;
                }
                while (next != -1)
                {
                    List<int> substring_list = new List<int>();
                    if (_debug) Console.WriteLine("     Start to trace from:" + i + " to " + next);

                    substring_list.Add(i);
                    substring_list.Add(next);

                    _char_list[i].neighbors.Remove(next);
                    _char_list[next].neighbors.Remove(i);
                    if (_char_list[next].split_here || _char_list[i].split_here)
                        TraceCharacters(substring_list, i, next, _larger_angle_threshold); // strict
                    else TraceCharacters(substring_list, i, next, _smaller_angle_threshold);
                    if (_char_list[next].split_here || _char_list[i].split_here)
                        TraceCharacters(substring_list, next, i, _larger_angle_threshold);
                    else TraceCharacters(substring_list, next, i, _smaller_angle_threshold);

                    if (substring_list.Count == 2)
                    {
                        tmp_list4two.Add(substring_list);
                        if (_debug) Console.WriteLine("                 Add tmp2ts:     " + substring_list[0] + "   " + substring_list[1]);
                    }
                    else
                        AddSubstring(substring_list);
                    next = FindNextNeighbor(i);
                }
            }

            MergeTwo(tmp_list4two);
            MergeOne(tmp_list4one);

        }
        private void ResetVisited()
        {
            for (int i = 0; i < _char_list.Count; i++)
                _char_list[i].visited = false;
        }
        private void ResetIncluded()
        {
            for (int i = 0; i < _char_list.Count; i++)
                _char_list[i].included = false;
        }
        private void Convert2FinalStringList()
        {
            TextString ts = new TextString();
            for (int i = 0; i < _char_list.Count; i++)
                ts.AddChar(_char_list[i]);
            _final_string_list.Add(ts);
        }
        private void AddSubstring(List<int> substring_list)
        {
            if (_debug) Console.Write("Final ST#: " + _final_string_list.Count + "                 Add TS:     ");
            TextString ts = new TextString();
            for (int i = 0; i < substring_list.Count; i++)
            {
                ts.AddChar(_char_list[substring_list[i]]);
                _char_list[substring_list[i]].included = true;
                if (_debug) Console.Write(substring_list[i] + " ");
            }

            _final_string_list.Add(ts);
            if (_debug) Console.WriteLine();
        }
        private double CosAngel(int idx1, int idx2, int i)
        {
            double error = 0.00001;

            Point p3 = new Point(_char_list[i].bbx.X + (_char_list[i].bbx.Width) / 2, _char_list[i].bbx.Y + (_char_list[i].bbx.Height) / 2);
            Point p1 = new Point(_char_list[idx1].bbx.X + (_char_list[idx1].bbx.Width) / 2, _char_list[idx1].bbx.Y + (_char_list[idx1].bbx.Height) / 2);
            Point p2 = new Point(_char_list[idx2].bbx.X + (_char_list[idx2].bbx.Width) / 2, _char_list[idx2].bbx.Y + (_char_list[idx2].bbx.Height) / 2);
            if (_debug) Console.WriteLine(p1.X + " " + p1.Y + ";" + p2.X + " " + p2.Y + ";" + p3.X + " " + p3.Y + ";");
            double x1 = p1.X;
            double x2 = p2.X;
            double x3 = p3.X;
            double y1 = p1.Y;
            double y2 = p2.Y;
            double y3 = p3.Y;
            if (x1 == x3 && x2 == x3)
                return 180;
            if (y1 == y3 && y2 == y3)
                return 180;
            double adotb = (x2 - x3) * (x1 - x3) + (y2 - y3) * (y1 - y3);
            double tmp1 = (Math.Sqrt((x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3)) * Math.Sqrt((x1 - x3) * (x1 - x3) + (y1 - y3) * (y1 - y3)));
            double tmp2 = adotb / tmp1;
            double angel = 0;
            if (Math.Abs(Math.Abs(tmp2) - 1) < error) angel = 180;
            else
            {
                angel = Math.Acos(adotb / tmp1);
                angel = angel * 180 / Math.PI;
            }
            if (angel < 0)
                return 180 + angel;

            else return angel;
        }


        public Rectangle bbx
        {
            get { return _bbx; }
            set { _bbx = value; }
        }
        public List<MyConnectedComponentsAnalysisFast.MyBlob> char_list
        {
            get { return _char_list; }
            set { _char_list = value; }
        }
        public List<TextString> final_string_list
        {
            get { return _final_string_list; }
            set { _final_string_list = value; }
        }
        public Bitmap srcimg
        {
            get { return _srcimg; }
            set { _srcimg = value; }
        }
        public List<Bitmap> rotated_img_list
        {
            get { return _rotated_img_list; }
            set { _rotated_img_list = value; }
        }
        public Point mass_center
        {
            get { return _mass_center; }
            set { _mass_center = value; }
        }
        public List<double> orientation_list
        {
            get { return _orientation_list; }
            set { _orientation_list = value; }
        }
        public int x_offset
        {
            get { return _x_offset; }
            set { _x_offset = value; }
        }
        public int y_offset
        {
            get { return _y_offset; }
            set { _y_offset = value; }
        }

    }
}
