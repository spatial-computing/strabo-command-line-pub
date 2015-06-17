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

using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Strabo.Core.TextRecognition
{
    public class RemoveNoiseText
    {
        public RemoveNoiseText() { }
        public static bool NotUpperCaseInTheMiddle(string word)
        {
            string testword = Regex.Replace(word, @"\n", " ");
            string[] token = word.Split(' ');
            //Check case
            //Get the culture property of the thread.
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            //Create TextInfo object.
            TextInfo textInfo = cultureInfo.TextInfo;
            for (int i = 0; i < token.Length; i++)
            {
                // remove anything with an upper-case letter in the middle of the string e.g., xxXxx 
                if ((textInfo.ToTitleCase(token[i]) == token[i] || textInfo.ToUpper(token[i]) == token[i] || textInfo.ToLower(token[i]) == token[i]))
                    return true;
                 
            }
            return false;
        }
        public static bool NotTooManyNoiseCharacters(string word)
        {
            if (word == null) return false;
           
            //Check alphabet ratio
            string clean = Regex.Replace(word, @"[^a-zA-Z -]", "");//Regex.Replace(word, @"[^a-zA-Z0-9]", "");
            if (((double)clean.Length / (double)(word.Length-2) <= 0.5))
                return false; //too many noise characters
            if(clean.Length <3)
                return false;
            
            
            //Q and digits
        //    if (word.Contains("Q") && Regex.IsMatch(word, @"\d"))
        //        return false;
            //Q and some other lower case characters
        //    if (Regex.IsMatch(word, @"[a-z]") && word.Contains("Q"))
        //       return false;

            //Check specific special characters
            //if (word.Contains("/") || word.Contains("\\") || word.Contains("%") || word.Contains("?") || word.Contains("!") || word.Contains("|") || word.Contains("*") || word.Equals("Q") || 
            //    word.Contains("_") || word.Contains("$") || word.Contains("¢") || word.Contains("§") || word.Contains("+") || word.Contains("~") || word.Contains("»") || word.Contains("<") || word.Contains(">") || word.Contains(@"\"))
            //    return false;

            int count = word.Split('Q').Length - 1;
            if (count >= 2) return false;
            count = word.Split('1').Length - 1;
            if (count >= 4) return false;
            count = word.Split('I').Length - 1;
            if (count >= 5) return false;
            int backSlashCount = word.Split('\"').Length - 1;
            if (backSlashCount >= 2) return false;
           // if ((word.Contains(";")))
           //     return false;
            //if (!Regex.IsMatch(word, @"[A-Za-z0-9.? )*]"))
            //    return false;
            //if (Regex.IsMatch(word, @"\n\n"))
            //{
            //    if (word.Length == 3)
            //        return false;
            //    if (word.Length == 4)
            //    {
            //       string tempword = word.Substring(0, 2);
            //     //  foreach (char inputchar in tempword)
            //     //  {
            //       if (char.IsLower(tempword[0]) && char.IsLower(tempword[1]))
            //               return false;
            //       if (char.IsLower(tempword[0]) && char.IsUpper(tempword[1]))
            //           return false;
            //     //  }
            //       //if (!CheckDictionaryTwoLetters.checkTwoWordDictionary(tempword))
            //       //    return false;
            //    }
            //    else if (word.Length > 4)
            //    {
            //        word = EditWord(word);
            //        if (word.Equals(""))
            //            return false;
            //    }
            //}
            //else
            //{
            //    if (word.Length == 1)
            //        return false;

            //    //if (word.Length == 2)
            //    //{
            //    //    if (!CheckDictionaryTwoLetters.checkTwoWordDictionary(word))
            //    //        return false;
            //    //}
            //    else if (word.Length > 2)
            //    {
            //        word = EditWord(word);
            //        if (word.Equals(""))
            //            return false;
            //    }
            //}
            return true;
        }

        public static string EditWord(string word)
        {
              string finalResult="";
              //string newword = Regex.Replace(word, "\n\n", "");
              char[] linedelimiterChars = { '\n' };
              char[] spacedelimiter = { ' ' };
              string[] splitline = word.Split(linedelimiterChars);
              if (splitline.Length > 6)
                  return "";
              for (int i = 0; i < splitline.Length - 2; i++)
              {
                  if (Regex.IsMatch(splitline[i], " "))
                  {
                      string[] spliteachline = splitline[i].Split(spacedelimiter);
                      for (int j = 0; j < spliteachline.Length; j++)
                      {
                          if (spliteachline[j].Length > 0)
                          {
                              if (j < spliteachline.Length - 1)
                                  finalResult += spliteachline[j] + " ";
                              else
                                  finalResult += spliteachline[j];
                          }
                      }
                  }
                  else
                  {
                      if (splitline[i].Length > 1)
                          finalResult += splitline[i];

                  }

                  if (!finalResult.Equals("") && splitline[i].Length!=1)
                  {
                      if (i < splitline.Length - 3)
                          finalResult += "\n";
                      else                         
                         finalResult += "\n\n";
                  }

                  if (!finalResult.Equals("") && splitline[i].Length == 1 && i == splitline.Length - 3)
                      finalResult += "\n";

              }
              return finalResult;
        }
    }
}
