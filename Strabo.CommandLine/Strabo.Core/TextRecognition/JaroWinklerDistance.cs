
/*
    2  * Licensed to the Apache Software Foundation (ASF) under one or more
    3  * contributor license agreements.  See the NOTICE file distributed with
    4  * this work for additional information regarding copyright ownership.
    5  * The ASF licenses this file to You under the Apache License, Version 2.0
    6  * (the "License"); you may not use this file except in compliance with
    7  * the License.  You may obtain a copy of the License at
    8  *
    9  *     http://www.apache.org/licenses/LICENSE-2.0
   10  *
   11  * Unless required by applicable law or agreed to in writing, software
   12  * distributed under the License is distributed on an "AS IS" BASIS,
   13  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   14  * See the License for the specific language governing permissions and
   15  * limitations under the License.
   16  */

using System;
using System.Linq;

namespace Strabo.Core.TextRecognition
{
    public class JaroWinklerDistance : StringDistance
    {
        private float threshold = 0.7f;

        private static int[] Matches(String s1, String s2)
        {
            String Max, Min;

            if (s1.Length > s2.Length)
            {
                Max = s1;
                Min = s2;
            }
            else
            {
                Max = s2;
                Min = s1;
            }

            var range = Math.Max(Max.Length / 2 - 1, 0);
            var matchIndexes = new int[Min.Length];

            for (var i = 0; i < matchIndexes.Length; i++)
                matchIndexes[i] = -1;

            var matchFlags = new bool[Max.Length];
            var matches = 0;

            for (var mi = 0; mi < Min.Length; mi++)
            {
                var c1 = Min[mi];
                for (int xi = Math.Max(mi - range, 0),
                    xn = Math.Min(mi + range + 1, Max.Length); xi < xn; xi++)
                {
                    if (matchFlags[xi] || c1 != Max[xi]) continue;

                    matchIndexes[mi] = xi;
                    matchFlags[xi] = true;
                    matches++;
                    break;
                }
            }

            var ms1 = new char[matches];
            var ms2 = new char[matches];

            for (int i = 0, si = 0; i < Min.Length; i++)
            {
                if (matchIndexes[i] != -1)
                {
                    ms1[si] = Min[i];
                    si++;
                }
            }

            for (int i = 0, si = 0; i < Max.Length; i++)
            {
                if (matchFlags[i])
                {
                    ms2[si] = Max[i];
                    si++;
                }
            }

            var transpositions = ms1.Where((t, mi) => t != ms2[mi]).Count();

            var prefix = 0;
            for (var mi = 0; mi < Min.Length; mi++)
            {
                if (s1[mi] == s2[mi])
                {
                    prefix++;
                }
                else
                {
                    break;
                }
            }

            return new int[] { matches, transpositions / 2, prefix, Max.Length };
        }

        public float GetDistance(String s1, String s2)
        {
            var mtp = Matches(s1, s2);
            var m = (float)mtp[0];

            if (m == 0)
                return 0f;

            float j = ((m / s1.Length + m / s2.Length + (m - mtp[1]) / m)) / 3;
            float jw = j < Threshold ? j : j + Math.Min(0.1f, 1f / mtp[3]) * mtp[2] * (1 - j);
            return jw;
        }

        /// <summary>
        /// Gets or sets the current value of the threshold used for adding the Winkler bonus.
        /// Set to a negative value to get the Jaro distance. The default value is 0.7.
        /// </summary>
        public float Threshold
        {
            get { return threshold; }
            set { this.threshold = value; }
        }
    }




    public interface StringDistance
    {
        /// <summary>
        /// Returns a float between 0 and 1 based on how similar the specified strings are to one another.  
        /// Returning a value of 1 means the specified strings are identical and 0 means the
        /// string are maximally different.
        /// </summary>
        /// <param name="s1">The first string.</param>
        /// <param name="s2">The second string.</param>
        /// <returns>a float between 0 and 1 based on how similar the specified strings are to one another.</returns>
        float GetDistance(string s1, string s2);

    }
}