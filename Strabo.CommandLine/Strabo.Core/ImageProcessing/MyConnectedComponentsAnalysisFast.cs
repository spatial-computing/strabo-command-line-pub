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
using System.Drawing;
using System.Drawing.Imaging;

namespace Strabo.Core.ImageProcessing
{
    /// <summary>
    /// Narges
    /// MyConnectedComponentsAnalysisFast class is used to retrieve all blobs in an image 
    /// In this class we have two other classes "MyBlob" and "MyBlobCounter". In MyBlob class we have information that we want for a blob.
    /// In MyBlobCounter we will investigate pixel by pixel of image to find different lables and blobs
    /// /// </summary>
    public class MyConnectedComponentsAnalysisFast
    {
        public class MyBlob
        {
            public Rectangle bbx;
            public Point mass_center;
            //LineOfBestFit lbf;               Because LineOfBestFit has commented here, I will comment lines that are related to LineOfBestFit- lines are 355,356 and 357

            public int pixel_count = 0;         /// It has been used in MyConnectedComponentsAnalysisFast but MyBlobCounter
            public int area;                    /// It has been used in MyConnectedComponentsAnalysisFast but MyBlobCounter
            public int pixel_id;
            //  public List<int> string_id_list = new List<int>();    It has not been used anywhere
            public int string_id;
            //   public bool salient;                     It has not been used anywhere
            public List<int> neighbors = new List<int>();
            public double orientation = -1;        //It has been used in MyConnectedComponentsAnalysisFast but MyBlobCounter
            public int sample_x;
            public int sample_y;
            public double m;
            public double b;               /// It has been used in MyConnectedComponentsAnalysisFast but MyBlobCounter
            public bool included = false;
            //  public bool included_during_cc_merge = true;     
            public int split_visited = 0;
            public bool visited = false;
            public bool sizefilter_included = true;
            public bool split_here = false;
            public int neighbor_count = 0;
            // public double[] mean_shift_results = new double[4];     It has not been used anywhere
           // public double[] mean_shift_results2 = new double[4];    It has not been used anywhere 
          //  public byte[] rgb = new byte[3];                        It has not been used anywhere

        }
        public class MyBlobCounter
        {

            public ushort[] objectLabels; // narges  
            private ushort objectsCount;                          //// I want to examine if parameters of MyBlobCounter can be private          
          // private int[, , ,] bbx;
         //  private int[] black;
          //  private int[] white;     bbx, black and white have not been used so I commented them 

            private int[] objectSize;
            private void Process(Bitmap srcImg)           ////I don’t know if I should have another function with a similar name that accepts path as input?
            {
                srcImg = ImageUtils.toGray(srcImg);
                // get source image size
                int width = srcImg.Width;                          
                int height = srcImg.Height;                         
                // allocate labels array
                objectLabels = new ushort[width * height];
                // initial labels count
                ushort labelsCount = 0;                             
                // create map
                int maxObjects = ((width / 2) + 1) * ((height / 2) + 1) + 1;
                int[] map = new int[maxObjects];
                // initially map all labels to themself
                for (int i = 0; i < maxObjects; i++)
                    map[i] = i;
                // lock source bitmap data
                BitmapData srcData = srcImg.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                int srcStride = srcData.Stride;
                int srcOffset = srcStride - width;
                // do the job
                unsafe
                {
                    byte* src = (byte*)srcData.Scan0.ToPointer();
                    int p = 0;
                    // label the first pixel
                    // 1 - for pixels of the first row
                    if (*src != 0)
                        objectLabels[p] = ++labelsCount; // label start from 1
                    ++src;
                    ++p;
                    // label the first row
                    for (int x = 1; x < width; x++, src++, p++)
                    {
                        // check if we need to label current pixel
                        if (*src != 0)
                        {
                            // check if the previous pixel already labeled
                            if (src[-1] != 0)
                                // label current pixel, as the previous
                                objectLabels[p] = objectLabels[p - 1];
                            else
                                // create new label
                                objectLabels[p] = ++labelsCount;
                        }
                    }
                    src += srcOffset;
                    // 2 - for other rows
                    // for each row
                    for (int y = 1; y < height; y++)
                    {
                        // for the first pixel of the row, we need to check
                        // only upper and upper-right pixels
                        if (*src != 0)
                        {
                            // check surrounding pixels
                            if (src[-srcStride] != 0)
                                // label current pixel, as the above
                                objectLabels[p] = objectLabels[p - width];
                            else if (src[1 - srcStride] != 0)
                                // label current pixel, as the above right
                                objectLabels[p] = objectLabels[p + 1 - width];
                            else
                                // create new label
                                objectLabels[p] = ++labelsCount;
                        }
                        ++src;
                        ++p;
                        // check left pixel and three upper pixels
                        for (int x = 1; x < width - 1; x++, src++, p++)
                        {
                            if (*src != 0)
                            {
                                // check surrounding pixels
                                if (src[-1] != 0)
                                    // label current pixel, as the left
                                    objectLabels[p] = objectLabels[p - 1];
                                else if (src[-1 - srcStride] != 0)
                                    // label current pixel, as the above left
                                    objectLabels[p] = objectLabels[p - 1 - width];
                                else if (src[-srcStride] != 0)
                                    // label current pixel, as the above
                                    objectLabels[p] = objectLabels[p - width];
                                if (src[1 - srcStride] != 0)
                                {
                                    if (objectLabels[p] == 0)
                                        // label current pixel, as the above right
                                        objectLabels[p] = objectLabels[p + 1 - width];
                                    else
                                    {
                                        int l1 = objectLabels[p];
                                        int l2 = objectLabels[p + 1 - width];
                                        if ((l1 != l2) && (map[l1] != map[l2]))
                                        {
                                            // merge
                                            if (map[l1] == l1)
                                                // map left value to the right
                                                map[l1] = map[l2];
                                            else if (map[l2] == l2)
                                                // map right value to the left
                                                map[l2] = map[l1];
                                            else
                                            {
                                                // both values already mapped
                                                map[map[l1]] = map[l2];
                                                map[l1] = map[l2];
                                            }
                                            // reindex
                                            for (int i = 1; i <= labelsCount; i++)
                                            {
                                                if (map[i] != i)
                                                {
                                                    // reindex
                                                    int j = map[i];
                                                    while (j != map[j])
                                                        j = map[j];
                                                    map[i] = j;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (objectLabels[p] == 0)
                                    // create new label
                                    objectLabels[p] = ++labelsCount;
                            }
                        }
                        // for the last pixel of the row, we need to check
                        // only upper and upper-left pixels
                        if (*src != 0)
                        {
                            // check surrounding pixels
                            if (src[-1] != 0)
                            {
                                // label current pixel, as the left
                                objectLabels[p] = objectLabels[p - 1];
                            }
                            else if (src[-1 - srcStride] != 0)
                            {
                                // label current pixel, as the above left
                                objectLabels[p] = objectLabels[p - 1 - width];
                            }
                            else if (src[-srcStride] != 0)
                            {
                                // label current pixel, as the above
                                objectLabels[p] = objectLabels[p - width];
                            }
                            else
                            {
                                // create new label
                                objectLabels[p] = ++labelsCount;
                            }
                        }
                        ++src;
                        ++p;
                        src += srcOffset;
                    }
                }
                // unlock source images
                srcImg.UnlockBits(srcData);
                // allocate remapping array
                ushort[] reMap = new ushort[map.Length];
                // count objects and prepare remapping array
                objectsCount = 0;
                for (int i = 1; i <= labelsCount; i++)
                {
                    if (map[i] == i)
                    {
                        // increase objects count
                        reMap[i] = ++objectsCount;
                    }
                }
                // second pass to compete remapping
                for (int i = 1; i <= labelsCount; i++)
                {
                    if (map[i] != i)
                    {
                        reMap[i] = reMap[map[i]];
                    }
                }
                // repair object labels
                for (int i = 0, n = objectLabels.Length; i < n; i++)
                {
                    objectLabels[i] = reMap[objectLabels[i]];
                }
            }
            // Get array of objects rectangles
            public List<MyBlob> GetBlobs(Bitmap srcImg)
            {
                Process(srcImg);
                ushort[] labels = this.objectLabels;
                int count = this.objectsCount;
                this.objectSize = new int[count + 1];
                double[] center_x = new double[count + 1];
                double[] center_y = new double[count + 1];
                // LBF off for now
                //LineOfBestFit[] lbf = new LineOfBestFit[count + 1];
                // image size
                int width = srcImg.Width;
                int height = srcImg.Height;
                int i = 0, label;
                // create object coordinates arrays
                int[] x1 = new int[count + 1];
                int[] y1 = new int[count + 1];
                int[] x2 = new int[count + 1];
                int[] y2 = new int[count + 1];
                int[] sample_x = new int[count + 1];
                int[] sample_y = new int[count + 1];
                for (int j = 1; j <= count; j++)
                {
                    x1[j] = width;
                    y1[j] = height;
                }
                // walk through labels array, skip one row and one col
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, i++)
                    {
                        // get current label
                        label = labels[i];
                        // skip unlabeled pixels
                        if (label == 0)
                            continue;
                        // LBF off for now
                        //if (lbf[label] == null)
                        //    lbf[label] = new LineOfBestFit();
                        //lbf[label].Update(x, height - y);
                        this.objectSize[label]++;
                        // check and update all coordinates
                        center_x[label] += x;
                        center_y[label] += y;
                        sample_x[label] = x; // record the last point as a sample
                        sample_y[label] = y;
                        if (x < x1[label])
                        {
                            x1[label] = x;
                        }
                        if (x > x2[label])
                        {
                            x2[label] = x;
                        }
                        if (y < y1[label])
                        {
                            y1[label] = y;
                        }
                        if (y > y2[label])
                        {
                            y2[label] = y;
                        }
                    }
                }
                List<MyBlob> blobs = new List<MyBlob>();
                for (int j = 1; j <= count; j++)
                {
                    MyBlob b = new MyBlob();
                    b.bbx = new Rectangle(x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1);
                    b.pixel_count = this.objectSize[j];
                    b.mass_center = new Point(Convert.ToInt32(center_x[j] / (double)b.pixel_count), Convert.ToInt32(center_y[j] / (double)b.pixel_count));
                    b.area = (x2[j] - x1[j] + 1) * (y2[j] - y1[j] + 1);
                    b.pixel_id = j;
                    b.sample_x = sample_x[j];
                    b.sample_y = sample_y[j];
                    // LBF off for now
                    if (b.bbx.Width == 1)
                    {
                        b.orientation = 90;
                        b.m = Double.NaN;
                        b.b = x1[j]; // x = 4
                    }
                    else if (b.bbx.Height == 1)
                    {
                        b.orientation = 180;
                        b.m = 0;
                        b.b = y1[j]; // y = 4
                    }
                    else
                    {
                     //   b.orientation = lbf[j].GetOrientation();  
                      //  b.m = lbf[j].m;
                       // b.b = lbf[j].b;
                    }
                    blobs.Add(b);
                }
                return blobs;
            }
        }
    }
}