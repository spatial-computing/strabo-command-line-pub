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

using System.Drawing;

namespace Strabo.Core.ImageProcessing
{
    /// <summary>
    /// To do: remove the dependency of BitmapToBoolArray2D to save memory usage
    /// </summary>
    public class ConnectLinesWithOnePixelGap
    {
        public ConnectLinesWithOnePixelGap() { }
        public Bitmap Apply(Bitmap srcimg)
        {
            bool[,] image = ImageUtils.BitmapToBoolArray2D(srcimg, 0);
            bool[,] tmp = new bool[srcimg.Height,srcimg.Width];
            for (int i = 1; i < srcimg.Width - 1; i++)
                for (int j = 1; j < srcimg.Height - 1; j++)
                {
                    if (image[j, i] == false) //background pixel
                    {
                        tmp[j,i]=image[j,i];
                        bool[] marks = new bool[8];
                        int nei_count = 0;
                        // 0 1 2
                        // 3 x 4
                        // 5 6 7
                        int count = 0;
                        for (int r = -1; r < 2; r++) //scan neighbors
                            for (int c = -1; c < 2; c++)
                            {
                                if (r == 0 && c == 0) 
                                    continue;
                                if (image[j + r, i + c]) 
                                    count++;
                                marks[nei_count] = image[j + r, i + c];
                                nei_count++;
                            }
                        //convert one pixel gaps between lines
                        {
                            if ((marks[0] && marks[7]) || (marks[1] && marks[6]) ||
                                (marks[2] && marks[5]) || (marks[3] && marks[4]))
                                tmp[j, i] = true;
                        }
                      
                    }
                        else 
                            tmp[j,i]=image[j,i];
                }
            for (int i = 1; i < srcimg.Width - 1; i++)
                for (int j = 1; j < srcimg.Height - 1; j++)
                    image[j, i] = tmp[j, i];
            return ImageUtils.ArrayBool2DToBitmap(image);
        }
    }
}
