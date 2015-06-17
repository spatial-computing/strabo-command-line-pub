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

namespace Strabo.Core.TextRecognition
{
    public class JaccardDistance
    {
        private int n;

        /// <summary>
        /// Creates an N-Gram distance measure using n-grams of the specified size.
        /// </summary>
        /// <param name="size">The size of the n-gram to be used to compute the string distance.</param>
        public JaccardDistance(int size)
        {
            this.n = size;
        }

        /// <summary>
        /// Creates an N-Gram distance measure using n-grams of size 2.
        /// </summary>
        public JaccardDistance()
            : this(2)
        {
        }
        public float GetDistanceFast(String source, String target)
        {
            int sl = source.Length;
            int tl = target.Length;

            if (sl == 0 || tl == 0)
            {
                if (sl == tl)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            if (sl < n || tl < n)
                return 0;

            //char[] sa = new char[sl + 2 * n - 2];
            //char[] ta = new char[tl + 2 * n - 2];

            //for (int i = 0; i < sa.Length; i++)
            //{
            //    if (i < n - 1 || i > sl)
            //        sa[i] = (char)0;//padding
            //    else
            //        sa[i] = source[i - n + 1];
            //}

            //for (int i = 0; i < ta.Length; i++)
            //{
            //    if (i < n - 1 || i > tl)
            //        ta[i] = (char)0;//padding
            //    else
            //        ta[i] = target[i - n + 1];
            //}

            HashSet<string> sset = new HashSet<string>();
            HashSet<string> tset = new HashSet<string>();
            HashSet<string> allset = new HashSet<string>();

            for (int i = 0; i < sl + 2 * n - 2 - n + 1; i++)
            {
                char[] qgram = new char[n];
                for (int j = 0; j < n; j++)
                {
                    if (i+j < n - 1 || i - n + 1 + j >= sl)
                        qgram[j] = (char)0;
                    else
                        qgram[j] = source[i -n+1 + j];
                 }
                sset.Add(new string(qgram));
                allset.Add(new string(qgram));
            }
            for (int i = 0; i < tl + 2 * n - 2 - n + 1; i++)
            {
                char[] qgram = new char[n];
                for (int j = 0; j < n; j++)
                {
                    if (i+j < n - 1 || i - n + 1 + j >= tl)
                        qgram[j] = (char)0;
                    else
                        qgram[j] = target[i - n + 1 + j];
                }
                tset.Add(new string(qgram));
                allset.Add(new string(qgram));
            }

            int matches = 0;
            foreach (string qgram in allset)
            {
                if (sset.Contains(qgram) && tset.Contains(qgram))
                    matches++;
            }


            return (float)matches / (float)(allset.Count);
        }
        public float GetDistance(String source, String target)
        {
            int sl = source.Length;
            int tl = target.Length;

            if (sl == 0 || tl == 0)
            {
                if (sl == tl)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            if (sl < n || tl < n)
                return 0;

            char[] sa = new char[sl + 2*n - 2];
            char[] ta = new char[tl + 2 * n - 2];

            for(int i=0;i<sa.Length;i++)
            {
                if (i < n - 1 || i>sl)
                    sa[i] = (char)0;//padding
                else
                    sa[i] = source[i - n + 1];
            }

            for (int i = 0; i < ta.Length; i++)
            {
                if (i < n - 1 || i > tl)
                    ta[i] = (char)0;//padding
                else
                    ta[i] = target[i - n + 1];
            }

            HashSet<string> sset = new HashSet<string>();
            HashSet<string> tset = new HashSet<string>();
            HashSet<string> allset = new HashSet<string>();

            for(int i=0;i<sa.Length-n+1;i++)
            {
                char[] qgram = new char[n];
                for (int j = 0; j < n; j++)
                    qgram[j] = sa[i + j];
                sset.Add(new string(qgram));
                allset.Add(new string(qgram));
            }
            for (int i = 0; i < ta.Length - n + 1; i++)
            {
                char[] qgram = new char[n];
                for (int j = 0; j < n; j++)
                    qgram[j] = ta[i + j];
                tset.Add(new string(qgram));
                allset.Add(new string(qgram));
            }

            int matches = 0;
            foreach(string qgram in allset)
            {
                if (sset.Contains(qgram) && tset.Contains(qgram))
                    matches++;
            }


            return (float)matches / (float)(allset.Count);
        }
    }
}
