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

using SpellChecker.Net.Search.Spell;
using Strabo.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Strabo.Core.TextRecognition
{
    public class DictResult
    {
        public string text;
        public double similarity;
        public double word_count=1;
    }
    public class CheckDictionary
    {
        static int _maxNumCharacterInAWord = 30;
        static int _maxWordLength = 4;
        static int _minWordLength = 2;
        static double _minWordSimilarity = 0.33;
        static int _minWordLengthForDictionaryComparison = 3;
        static List<string>[] _IndexDictionary = new List<string>[_maxNumCharacterInAWord];
        static bool dictProcessed = false;
        static int _dictionaryExactMatchStringLength;

        private static string reverseWord(string word)
        {
            char[] charArray = word.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static double findSimilarDictionaryWord(string word, double maxSimilarity, int index, List<string> equalMinDistanceDictWordList)
        {
            index = index - _minWordLength;
            word = word.ToLower();
            double NewSimilarity = 0;
            int WordLength = word.Length;
            if ((WordLength + index) < 0)
                return maxSimilarity;
            if ((WordLength + index) >= _IndexDictionary.Length)
                return maxSimilarity;
            if (_IndexDictionary[WordLength + index] == null)
                return maxSimilarity;

            for (int j = 0; j < _IndexDictionary[WordLength + index].Count; j++)
            {
                JaroWinklerDistance JaroDist = new JaroWinklerDistance();
                NGramDistance ng = new NGramDistance();
                JaccardDistance jd = new JaccardDistance();

                NewSimilarity = jd.GetDistance(word, _IndexDictionary[WordLength + index][j]);//(double)JaroDist.GetDistance(word, _IndexDictionary[WordLenght - 1 + index][j]);

                if (NewSimilarity > maxSimilarity)
                {
                    equalMinDistanceDictWordList.Clear();
                    equalMinDistanceDictWordList.Add(_IndexDictionary[WordLength + index][j]);
                    maxSimilarity = NewSimilarity;
                }
                else if (NewSimilarity == maxSimilarity)
                    equalMinDistanceDictWordList.Add(_IndexDictionary[WordLength + index][j]);
            }
            return maxSimilarity;
        }
        public static void readDictionary(string DictionaryPath)
        {
            if (dictProcessed) return;
            dictProcessed = true;
            StreamReader file = new StreamReader(DictionaryPath);   /// relative path
            List<string> Dictionary = new List<string>();
            string line;

            // read dictionary
            while ((line = file.ReadLine()) != null)
            {
                if (_IndexDictionary[line.Length] == null)
                    _IndexDictionary[line.Length] = new List<string>();
                _IndexDictionary[line.Length].Add(line);
            }
        }
        private static DictResult checkOneWord(string text)
        {
            double maxSimilarity = 0;
            DictResult dictR = new DictResult();
            List<string> equalMinDistanceDictWordList = new List<string>();

            if (text.Length == _dictionaryExactMatchStringLength)//short strings are looking for the exact match
            {
                maxSimilarity = findSimilarDictionaryWord(text, maxSimilarity, 0, equalMinDistanceDictWordList);
                if (maxSimilarity != 1)
                {
                    dictR.similarity = 0;
                    dictR.text = "";
                }
                else
                {
                    dictR.similarity = maxSimilarity;
                    dictR.text = equalMinDistanceDictWordList[0];
                }
            }
            else
            {
                for (int m = 0; m < _maxWordLength; m++)
                    maxSimilarity = findSimilarDictionaryWord(text, maxSimilarity, m, equalMinDistanceDictWordList);
                if (maxSimilarity < _minWordSimilarity) //dictionary word not found (most similar is 1) hill vs hall = 0.333333
                {
                    dictR.similarity = 0;
                    dictR.text = "";
                }
                else
                {
                    dictR.similarity = maxSimilarity;
                    dictR.text = equalMinDistanceDictWordList[0];
                }
            }
            return dictR;
        }
        private static DictResult checkOneLine(string text)
        {
            DictResult dictRLine = new DictResult();
            dictRLine.similarity = 0;
            dictRLine.text = "";

            List<DictResult> dictionaryResultList = new List<DictResult>();

            string[] InputFragments = text.Split(' ');
            for (int k = 0; k < InputFragments.Length; k++)
            {
                DictResult dictRWord = checkOneWord(InputFragments[k]);
                dictionaryResultList.Add(dictRWord);
            }
            for (int i = 0; i < dictionaryResultList.Count; i++)
            {
                if (dictionaryResultList[i].similarity > _minWordSimilarity)
                {
                    if (i == dictionaryResultList.Count - 1)
                        dictRLine.text += dictionaryResultList[i].text;
                    else
                        dictRLine.text += dictionaryResultList[i].text + " ";
                    dictRLine.similarity += dictionaryResultList[i].similarity;
                    dictRLine.word_count++;
                }
            }
            dictRLine.similarity /= (double)(dictRLine.word_count);
            return dictRLine;
        }
        private static DictResult checkMultiLines(string text)
        {
            DictResult dictRPage = new DictResult();
            dictRPage.similarity = 0;
            dictRPage.text = "";

            List<DictResult> dictionaryResultList = new List<DictResult>();

            string[] InputFragments = text.Split('\n');
            for (int k = 0; k < InputFragments.Length; k++)
            {
                DictResult dictRWord = checkOneLine(InputFragments[k]);
                dictionaryResultList.Add(dictRWord);
            }
            for (int i = 0; i < dictionaryResultList.Count; i++)
            {
                if (i == dictionaryResultList.Count - 1)
                    dictRPage.text += dictionaryResultList[i].text;
                else
                    dictRPage.text += dictionaryResultList[i].text + "\n";

                dictRPage.similarity += dictionaryResultList[i].similarity * (double)dictionaryResultList[i].word_count;
                dictRPage.word_count = +dictionaryResultList[i].word_count;
            }
            dictRPage.similarity /= (double)(dictRPage.word_count);
            return dictRPage; ;
        }
        public static TessResult getDictionaryWord(TessResult tr, int dictionaryExactMatchStringLength)
        {
            _dictionaryExactMatchStringLength = dictionaryExactMatchStringLength;

            if (tr.tess_word3.Contains("ouse"))
                Console.WriteLine("debug");
            try
            {
                if (tr.tess_word3 == null || tr.tess_word3.Length < _dictionaryExactMatchStringLength) //input is invalid
                {
                    tr.id = "-1";
                }
                else
                {
                    DictResult dictR;

                    if (!tr.tess_word3.Contains(" ") && !tr.tess_word3.Contains("\n")) // input is a single word
                        dictR = checkOneWord(tr.tess_word3);
                    else if (!tr.tess_word3.Contains("\n")) // input is a single line
                        dictR = checkOneLine(tr.tess_word3);
                    else // input is multi-lines
                        dictR = checkMultiLines(tr.tess_word3);
                    tr.dict_similarity = dictR.similarity;
                    tr.dict_word3 = dictR.text;
                }
                return tr;
            }
            catch (Exception e)
            {
                Log.WriteLine("Check Dictionary: " + e.Message);
                throw e;
            }
        }
    }
}
