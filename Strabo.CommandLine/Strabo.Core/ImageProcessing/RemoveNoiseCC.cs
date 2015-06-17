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
    public class RemoveNoiseCC
    {
        public RemoveNoiseCC() { }

        public Bitmap Apply(Bitmap srcimg, int char_size)
        {
            double min_pixel_area_size = 0.18;

            srcimg = ImageUtils.ConvertGrayScaleToBinary(srcimg, 254);
            srcimg = ImageUtils.InvertColors(srcimg);

            MyConnectedComponentsAnalysisFast.MyBlobCounter char_bc = new MyConnectedComponentsAnalysisFast.MyBlobCounter();
            List<MyConnectedComponentsAnalysisFast.MyBlob> char_blobs = char_bc.GetBlobs(srcimg);
            ushort[] char_labels = char_bc.objectLabels;

            HashSet<int> noise_char_idx_set = new HashSet<int>();

            for (int i = 0; i < char_blobs.Count; i++)
            {
                if(((double)char_blobs[i].pixel_count / (double)char_blobs[i].area) <min_pixel_area_size) //line
                    noise_char_idx_set.Add(i);
                if(char_blobs[i].bbx.Width < char_size &&
                    char_blobs[i].bbx.Height < char_size ) // small cc
                    noise_char_idx_set.Add(i);
                if(char_blobs[i].bbx.Width < char_size && char_blobs[i].bbx.Height > char_size *3)
                    noise_char_idx_set.Add(i);
                if (char_blobs[i].bbx.Height < char_size && char_blobs[i].bbx.Width > char_size * 3)
                    noise_char_idx_set.Add(i);
            }

            for (int i = 0; i < srcimg.Width * srcimg.Height; i++)
            {
                if (char_labels[i] != 0)
                {
                    int idx = char_labels[i] - 1;
                    if (noise_char_idx_set.Contains(idx)) char_labels[i] = 0;
                }
            }
            bool[,] img = new bool[srcimg.Height, srcimg.Width];
            for (int i = 0; i < srcimg.Width; i++)
                for (int j = 0; j < srcimg.Height; j++)
                    if (char_labels[j * srcimg.Width + i] == 0)
                        img[j, i] = false;
                    else
                        img[j, i] = true;
            return ImageUtils.ArrayBool2DToBitmap(img);
        }
    }
}
