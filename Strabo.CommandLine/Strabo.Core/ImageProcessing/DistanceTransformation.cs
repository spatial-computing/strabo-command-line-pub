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

namespace Strabo.Core.ImageProcessing
{
    /// <summary>
    /// Narges
    /// </summary>
    public class DistanceTransformation
    {

        public DistanceTransformation() { }
        public int[,] ApplyFGisZero(int[,] image)
        {
            int mh = image.GetLength(0);
            int mw = image.GetLength(1);
            int zero = 1;
          //  int min_dist = 1000000;
            int minx = 1, miny = 1, maxx = mw - 1, maxy = mh - 1;
            while (zero != 0)
            {
                zero = 0;
                int[,] tmp = new int[mh, mw];
                for (int i = miny; i < maxy; i++)
                    for (int j = minx; j < maxx; j++)
                    {
                        tmp[i, j] = image[i, j];
                        if (image[i, j] == 0)
                        {
                            int min = 1000000;
                            int y = i - 1, x = j - 1;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i; x = j - 1;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i + 1; x = j - 1;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i - 1; x = j;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i + 1; x = j;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i - 1; x = j + 1;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i; x = j + 1;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            y = i + 1; x = j + 1;
                            if (image[y, x] > 0 && image[y, x] < min)
                                min = image[y, x];
                            if (min > 0 && min != 1000000)
                                tmp[i, j] = min + 1;                  // tmp[xx,yy] == 2 is the boundary pixel, i.e., tmp minimal is 2
                        }
                        if (tmp[i, j] == 0)
                            zero++;
                    }
                for (int i = miny; i < maxy; i++)
                    for (int j = minx; j < maxx; j++)
                        image[i, j] = tmp[i, j];
            }
            return image;
        }
    }
}
