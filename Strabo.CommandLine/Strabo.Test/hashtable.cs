using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Strabo.Test
{
    class hashtable
    {
        public void test()
        {
            StreamReader file = new StreamReader(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\bin\Debug\dict_all.txt");
            string line;
            //HashSet<List<string>> trainedData=new HashSet<List<string>>();
           // Hashtable TrainedData = new Hashtable(); 
            Dictionary<int, List<string>> TrainedData = new Dictionary<int, List<string>>();
            SortedDictionary<string, float> frequencyOftwoLetters = new SortedDictionary<string, float>();      
            
          // trainedData.Add()
            while ((line=file.ReadLine())!=null)
            {
                string code = "";
                float count = 0;
                for (int i = 0; i < line.Length-1;i++)
                {
                    code = line.Substring(i, 2);
                    frequencyOftwoLetters.TryGetValue(code, out count);
                    if (count ==null)
                    {
                        count = new int();
                        count = 1;
                        frequencyOftwoLetters.Add(code, count);
                    }
                    else 
                    {
                        count++;
                        frequencyOftwoLetters.Remove(code);
                        frequencyOftwoLetters.Add(code, count);
                    }
                }
                //frequencyOftwoLetters.Keys.ToList().Sort();
                string twofirstletter = line.Substring(0, 2);
                int code1 = twofirstletter.GetHashCode();
                List<string> words=new List<string>();
                TrainedData.TryGetValue(twofirstletter.GetHashCode(), out words);
                string newline="";
                if (words!=null)
                {
                    TrainedData.Remove(code1);
                    words.Add(line);
                    TrainedData.Add(code1, words);
                }
                else
                {
                    words = new List<string>();
                    words.Add(line);
                    TrainedData.Add(code1, words);
                }
                 //   words = TrainedData[twofirstletter.GetHashCode()];
              //   TrainedData.Add(twofirstletter.GetHashCode(), line);
                
            }


            float sumOfFrequrencies = 0;


            float maxFrequenncy = frequencyOftwoLetters.Values.Max();
            float frequency=0;
            List<string> keys=new List<string>();
            keys=frequencyOftwoLetters.Keys.ToList();

            for (int i = 0; i < keys.Count(); i++)
            {

                frequencyOftwoLetters.TryGetValue(keys[i], out frequency);
                sumOfFrequrencies += frequency;
            }

            for (int i=0;i<keys.Count();i++)
            {

                frequencyOftwoLetters.TryGetValue(keys[i], out frequency);
                frequencyOftwoLetters.Remove(keys[i]);
                frequency = (frequency / sumOfFrequrencies)*10000;
                int roundedFrequency=0;
                if (frequency < 1)
                    roundedFrequency = 1;
                else
                   roundedFrequency = Convert.ToInt32(frequency);
                frequencyOftwoLetters.Add(keys[i], roundedFrequency);
            }
            maxFrequenncy = frequencyOftwoLetters.Values.Max();
        }

    }
}
