
using KdKeys.DataMining.Clustering.KMeans;
using Strabo.Core.ImageProcessing;
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
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Strabo.Core.ColorSegmentation
{
    public class MyKMeans
    {
        int[] pixels;

        public class RGB
        {
            public const short B = 0;
            public const short G = 1;
            public const short R = 2;
        }

        Hashtable color_table = new Hashtable();
        public MyKMeans() { }
        public void getRGBData(ColorHistogram colorHist, double[][] data)
        {
            int size = colorHist.getNumberOfColors();

            for (int x = 0; x < size; x++)
            {
                data[x] = new double[3];
                int rgb = colorHist.getColor(x);
                data[x][0] = ((rgb & 0xFF0000) >> 16);
                data[x][1] = ((rgb & 0xFF00) >> 8);
                data[x][2] = (rgb & 0xFF);
            }
        }
        public void getYIQData(ColorHistogram colorHist, double[][] data)
        {
            int size = colorHist.getNumberOfColors();

            for (int x = 0; x < size; x++)
            {
                data[x] = new double[3];
                int rgb = colorHist.getColor(x);
                double[] yiq = RGB2YIQ(rgb);

                for (int y = 0; y < 3; y++)
                {
                    data[x][y] = yiq[y];
                }
            }
        }
        public string Apply(int k, string fn, string outImagePath)
        {
            Bitmap srcimg = new Bitmap(fn);
            pixels = ImageUtils.BitmapToArray1DIntRGB(srcimg);
            ColorHistogram colorHist = new ColorHistogram(pixels, false);
            int size = colorHist.getNumberOfColors();
            if (size <= k)
                srcimg.Save(outImagePath, ImageFormat.Png);
            else
            {
                double[][] data = new double[size][];

                getRGBData(colorHist, data);
                ClusterCollection clusters;
                KMeansParallel kMeans = new KMeansParallel();
                clusters = kMeans.ClusterDataSet(k, data);
                PaintRGB(srcimg, kMeans, clusters).Save(outImagePath, ImageFormat.Png);
            }
            return outImagePath;
        }
        public Bitmap PaintRGB(Bitmap srcimg, KMeansParallel kMeans, ClusterCollection clusters)
        {
            int width = srcimg.Width;
            int height = srcimg.Height;
            BitmapData srcData = srcimg.LockBits(
               new Rectangle(0, 0, width, height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int srcOffset = srcData.Stride - width * 3;
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += 3)
                    {
                        int rgb = src[RGB.R] * 256 * 256 + src[RGB.G] * 256 + src[RGB.B];
                        int rgb_mean;

                        if (color_table.ContainsKey(rgb))
                            rgb_mean = (int)color_table[rgb];
                        else
                        {
                            double[] yiq = new double[3];
                            yiq[0] = src[RGB.R]; yiq[1] = src[RGB.G]; yiq[2] = src[RGB.B];

                            double min_dist = Double.MaxValue;
                            int idx = 0;

                            for (int i = 0; i < clusters.Count; i++)
                            {
                                double dist = kMeans.EuclideanDistance(yiq, clusters[i].ClusterMean);
                                if (dist < min_dist)
                                {
                                    min_dist = dist;
                                    idx = i;
                                }
                            }
                            rgb_mean = (int)(clusters[idx].ClusterMean[0]) * 256 * 256 +
                                (int)(clusters[idx].ClusterMean[1]) * 256 +
                                (int)(clusters[idx].ClusterMean[2]);

                            color_table.Add(rgb, rgb_mean);
                        }
                        src[RGB.R] = (byte)((rgb_mean & 0xFF0000) >> 16);
                        src[RGB.G] = (byte)((rgb_mean & 0xFF00) >> 8);
                        src[RGB.B] = (byte)(rgb_mean & 0xFF);
                    }
                    src += srcOffset;
                }
            }
            srcimg.UnlockBits(srcData);
            return srcimg;
        }
        public Bitmap PaintYIQ(Bitmap srcimg, KMeansParallel kMeans, ClusterCollection clusters)
        {
            int width = srcimg.Width;
            int height = srcimg.Height;
            BitmapData srcData = srcimg.LockBits(
               new Rectangle(0, 0, width, height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int srcOffset = srcData.Stride - width * 3;
            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += 3)
                    {
                        int rgb = src[RGB.R] * 256 * 256 + src[RGB.G] * 256 + src[RGB.B];
                        int rgb_mean;

                        if (color_table.ContainsKey(rgb))
                            rgb_mean = (int)color_table[rgb];
                        else
                        {
                            double[] yiq = RGB2YIQ(rgb);
                            double min_dist = Double.MaxValue;
                            int idx = 0;

                            for (int i = 0; i < clusters.Count; i++)
                            {
                                double dist = kMeans.EuclideanDistance(yiq, clusters[i].ClusterMean);
                                if (dist < min_dist)
                                {
                                    min_dist = dist;
                                    idx = i;
                                }
                            }
                            rgb_mean = YIQ2RGB(clusters[idx].ClusterMean[0], clusters[idx].ClusterMean[1], clusters[idx].ClusterMean[2]);
                            color_table.Add(rgb, rgb_mean);
                        }
                        src[RGB.R] = (byte)((rgb_mean & 0xFF0000) >> 16);
                        src[RGB.G] = (byte)((rgb_mean & 0xFF00) >> 8);
                        src[RGB.B] = (byte)(rgb_mean & 0xFF);
                    }
                    src += srcOffset;
                }
            }
            srcimg.UnlockBits(srcData);
            return srcimg;
        }
        public double[] RGB2YIQ(int rgb)
        {
            double Rc = ((rgb & 0xFF0000) >> 16);
            double Gc = ((rgb & 0xFF00) >> 8);
            double Bc = (rgb & 0xFF);
            double[] yiq = new double[3];
            yiq[0] = 0.299f * Rc + 0.587f * Gc + 0.114f * Bc; // Y
            yiq[1] = 0.5957f * Rc - 0.2744f * Gc - 0.3212f * Bc; // I
            yiq[2] = 0.2114f * Rc - 0.5226f * Gc + 0.3111f * Bc; // Q
            return yiq;
        }
        public int YIQ2RGB(double Yc, double Ic, double Qc)
        {
            return Convert.ToInt32((Yc + 0.9563f * Ic + 0.6210f * Qc) * 256 * 256 +
            (Yc - 0.2721f * Ic - 0.6473f * Qc) * 256 +
            (Yc - 1.1070f * Ic + 1.7046f * Qc));
        }
    }
}


//My unsuccessful change of this code for providing center seed for kmean
#region
/*
        private class ClusterMinMaxColor
        {
            public double rMin = float.MaxValue, rMax = double.MinValue;
            public double gMin = float.MaxValue, gMax = double.MinValue;
            public double bMin = float.MaxValue, bMax = double.MinValue;
        }

        public class ImageClusterMap
        {
            public int x = 0, y = 0;
            public double centerR = 0,centerG=0,centerB=0;
            public double pixelR = 0, pixelG = 0, pixelB=0;
            public int clusterInex;
        }

      
        public KeyValuePair<ClusterCollection, ImageClusterMap[]> Apply(int k, string fn, string outImagePath, ClusterCollection lastCluster, ImageClusterMap[] lastImageClusterMapTable)
        {
            Bitmap srcimg = new Bitmap(fn);
            pixels = ImageUtils.BitmapToArray1DIntRGB(srcimg);
            ColorHistogram colorHist = new ColorHistogram(pixels, false);
            int size = colorHist.getNumberOfColors();
            //if (size <= k)
            //    return fn;

            double[][] data = new double[size][];

            RGBData(colorHist, data);
            ClusterCollection clusters;
            KMeansParallel kMeans = new KMeansParallel();
            //clusters = kMeans.ClusterDataSet(k, data);
            if (lastCluster == null || lastCluster.Count <= 1)
            {
                clusters = kMeans.ClusterDataSetRandomSeeding(k, data);
            }
            else
            {
                ClusterCollection providedClusters = getNewClusterCollection(lastCluster, lastImageClusterMapTable);
                clusters = kMeans.ClusterDataSet2(k, providedClusters, data);
            }
           
            ImageClusterMap[] imageClusterMapTable = mapClusterCollectionToImage(srcimg, kMeans, clusters);
            saveStat(clusters, imageClusterMapTable, k, fn);

            PaintRGB(srcimg, kMeans, clusters).Save(outImagePath, ImageFormat.Png);

            KeyValuePair<ClusterCollection, ImageClusterMap[]> p = new KeyValuePair<ClusterCollection, ImageClusterMap[]>(clusters, imageClusterMapTable);
            return p;
        }


        ClusterCollection getNewClusterCollection(ClusterCollection lastCluster, ImageClusterMap[] lastImageClusterMapTable)
        {
            ClusterCollection clusters = lastCluster;
            ImageClusterMap[] imageCluster = lastImageClusterMapTable;

            double[] varOfAllCenter = new double[clusters.Count];
            ClusterMinMaxColor[] minMaxColorCollection = new ClusterMinMaxColor[clusters.Count];

            //Find the cluster that has the biggest variance
            for (int i = 0; i < clusters.Count; i++)
            {
                double centerR = clusters[i].ClusterMean[0];
                double centerG = clusters[i].ClusterMean[1];
                double centerB = clusters[i].ClusterMean[2];

                double varR = 0;
                double varG = 0;
                double varB = 0;
                int sampleCountInCluster = 0;
                ClusterMinMaxColor minMaxColor = new ClusterMinMaxColor();

                for (int j = 0; j < imageCluster.Length; j++)
                {
                    if (imageCluster[j].clusterInex == i)
                    {
                        sampleCountInCluster++;

                        double sampleR = imageCluster[j].pixelR;
                        double sampleG = imageCluster[j].pixelG;
                        double sampleB = imageCluster[j].pixelB;

                        varR += Math.Pow(sampleR - centerR, 2);
                        varG += Math.Pow(sampleG - centerG, 2);
                        varB += Math.Pow(sampleB - centerB, 2);


                        //Find min and max color in each cluster
                        if (sampleR < minMaxColor.rMin)
                        {
                            minMaxColor.rMin = sampleR;
                        }
                        else if (sampleR > minMaxColor.rMax)
                        {
                            minMaxColor.rMax = sampleR;
                        }

                        if (sampleG < minMaxColor.gMin)
                        {
                            minMaxColor.gMin = sampleG;
                        }
                        else if (sampleG > minMaxColor.gMax)
                        {
                            minMaxColor.gMax = sampleG;
                        }

                        if (sampleB < minMaxColor.bMin)
                        {
                            minMaxColor.bMin = sampleB;
                        }
                        else if (sampleB > minMaxColor.bMax)
                        {
                            minMaxColor.bMax = sampleB;
                        }
                    }
                }

                varR = varR / sampleCountInCluster;
                varG = varG / sampleCountInCluster;
                varB = varB / sampleCountInCluster;

                varOfAllCenter[i] = (varR + varG + varB) / 3;
                minMaxColorCollection[i] = minMaxColor;
            }

            //find the max variance
            double maxVar = double.MinValue;
            int maxK = 0;
            double maxVarCenterR = 0, maxVarCenterG = 0, maxVarCenterB = 0;

            for (int i = 0; i < clusters.Count; i++)
            {
                if (varOfAllCenter[i] > maxVar)
                {
                    maxVar = varOfAllCenter[i];
                    maxK = i;

                    maxVarCenterR = clusters[i].ClusterMean[0];
                    maxVarCenterG = clusters[i].ClusterMean[1];
                    maxVarCenterB = clusters[i].ClusterMean[2];
                }
            }



            //create a, a2, a3 constants
            double low = 0, high = 0;

            low = Math.Abs( maxVarCenterR - minMaxColorCollection[maxK].rMin);
            high = Math.Abs( maxVarCenterR - minMaxColorCollection[maxK].rMax);
            double aR = (low > high) ? high / 2 : low / 2;

            low = Math.Abs(maxVarCenterG - minMaxColorCollection[maxK].gMin);
            high = Math.Abs(maxVarCenterG - minMaxColorCollection[maxK].gMax);
            double aG = (low > high) ? high / 2 : low / 2;

            low =  Math.Abs(maxVarCenterB - minMaxColorCollection[maxK].bMin);
            high = Math.Abs(maxVarCenterB - minMaxColorCollection[maxK].bMax);
            double aB = (low > high) ? high / 2 : low / 2;


            //remove old center and two new centers
            ClusterCollection newClusters = new ClusterCollection();

            for (int i = 0; i < clusters.Count; i++)
            {
                if (i == maxK)
                    continue;

                double centerR = clusters[i].ClusterMean[0];
                double centerG = clusters[i].ClusterMean[1];
                double centerB = clusters[i].ClusterMean[2];

                Cluster c = new Cluster();
                double[] seed = new double[3];
                seed[0] = centerR;
                seed[1] = centerG;
                seed[2] = centerB;
                c.Add(seed);
                newClusters.Add(c);
            }

            Cluster c1 = new Cluster();
            double[] seed1 = new double[3];
            seed1[0] = maxVarCenterR - aR;
            seed1[1] = maxVarCenterG - aG;
            seed1[2] = maxVarCenterB - aB;
            c1.Add(seed1);
            newClusters.Add(c1);


            Cluster c2 = new Cluster();
            double[] seed2 = new double[3];
            seed2[0] = maxVarCenterR + aR;
            seed2[1] = maxVarCenterG + aG;
            seed2[2] = maxVarCenterB + aB;
            c2.Add(seed2);
            newClusters.Add(c2);

            return newClusters;
        }

        void saveStat(ClusterCollection clusters, ImageClusterMap[] imageClusterMapTable, int k , string fn)
        {
            double intra = intraDistance(clusters, imageClusterMapTable);
            double inter = interDistance(clusters);
            double validity = intra / inter;

            #region Save Report to file

            String retSt = String.Empty;
            string reportFile = Path.GetDirectoryName(fn) + @"\reportKmean.csv";
            if (System.IO.File.Exists(reportFile))
            {
                retSt = System.IO.File.ReadAllText(reportFile);
            }
            else
            {
                retSt = "k, intra, inter, validaity " + Environment.NewLine;
            }
            retSt += String.Format("{0}, {1}, {2}, {3}", k, intra, inter, validity) + Environment.NewLine;

            System.IO.File.WriteAllText(reportFile, retSt);

            #endregion
        }

        double interDistance(ClusterCollection clusters)
        {
            double minInterDistance = int.MaxValue;

            for (int i = 0; i < clusters.Count - 1; i++)
            {
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    double cR1 = clusters[i].ClusterMean[0];
                    double cG1 = clusters[i].ClusterMean[1];
                    double cB1 = clusters[i].ClusterMean[2];

                    double cR2 = clusters[j].ClusterMean[0];
                    double cG2 = clusters[j].ClusterMean[1];
                    double cB2 = clusters[j].ClusterMean[2];

                    double d = Math.Sqrt(Math.Pow(cR1 - cR2, 2) + Math.Pow(cG1 - cG2, 2) + Math.Pow(cB1 - cB2, 2));
                    if (d < minInterDistance)
                    {
                        minInterDistance = d;
                    }
                }
            }

            return minInterDistance;
        }


        double intraDistance(ClusterCollection clusters, ImageClusterMap[] imageClusterMapTable)
        {
            double intra = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                double centerR = clusters[i].ClusterMean[0];
                double centerG = clusters[i].ClusterMean[1];
                double centerB = clusters[i].ClusterMean[2];

                for (int j = 0; j < imageClusterMapTable.Length; j++)
                {
                    if (imageClusterMapTable[j].clusterInex == i)
                    {
                        double sampleR = imageClusterMapTable[j].pixelR;
                        double sampleG = imageClusterMapTable[j].pixelG;
                        double sampleB = imageClusterMapTable[j].pixelB;

                        intra += Math.Sqrt(Math.Pow(centerR - sampleR, 2) + Math.Pow(centerG - sampleG, 2) + Math.Pow(centerB - sampleB, 2));
                    }
                }
            }

            intra /= imageClusterMapTable.Length;

            return intra;
        }

        ImageClusterMap[] mapClusterCollectionToImage(Bitmap srcimg, KMeansParallel kMeans, ClusterCollection clusters)
        {
            int width = srcimg.Width;
            int height = srcimg.Height;

            ImageClusterMap[] maps = new ImageClusterMap[width * height];
            int mapsIndex = 0;

            BitmapData srcData = srcimg.LockBits(
               new Rectangle(0, 0, width, height),
               ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int srcOffset = srcData.Stride - width * 3;

            unsafe
            {
                byte* src = (byte*)srcData.Scan0.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, src += 3)
                    {
                        double[] yiq = new double[3];
                        yiq[0] = src[RGB.R]; yiq[1] = src[RGB.G]; yiq[2] = src[RGB.B];

                        double min_dist = Double.MaxValue;
                        int idx = 0;

                        for (int i = 0; i < clusters.Count; i++)
                        {
                            double dist = kMeans.EuclideanDistance(yiq, clusters[i].ClusterMean);
                            if (dist < min_dist)
                            {
                                min_dist = dist;
                                idx = i;
                            }
                        }

                        ImageClusterMap map = new ImageClusterMap();
                        map.centerR = clusters[idx].ClusterMean[0];
                        map.centerG = clusters[idx].ClusterMean[1];
                        map.centerB = clusters[idx].ClusterMean[2];

                        map.pixelR = yiq[0];
                        map.pixelG = yiq[1];
                        map.pixelB = yiq[2];

                        map.x = x;
                        map.y = y;

                        map.clusterInex = idx;

                        maps[mapsIndex] = map;
                        mapsIndex++;
                    }
                    src += srcOffset;
                }
            }
            srcimg.UnlockBits(srcData);
            return maps;
        }

 */

#endregion