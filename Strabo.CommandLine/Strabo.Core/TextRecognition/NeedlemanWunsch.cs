using System;
using System.Configuration;

namespace Strabo.Core.TextRecognition
{
    class NeedlemanWunsch
    {
        static int refSeqCnt;
        static int alineSeqCnt;
        static int[,] scoringMatrix;
        static object[,] excelarray;
        static int excelrowNo;
        static int matchcharcount;
        static int initialWeights = -2;
        static int deleteWeights = -3;
        static int editWeights = -2;
        static int addWeights = -1;
        static int matchWeights = 3;
        static int matchdefinedWeights = 2;
        public static void initSimMatrix(string refSeq, string alineSeq)
        {
            refSeqCnt = refSeq.Length + 1;
            alineSeqCnt = alineSeq.Length + 1;
            scoringMatrix = new int[alineSeqCnt, refSeqCnt];

            for (int i = 0; i < alineSeqCnt; i++)
            {
                scoringMatrix[i, 0] = initialWeights;
            }

            for (int j = 0; j < refSeqCnt; j++)
            {
                scoringMatrix[0, j] = initialWeights;
            }

            for (int i = 0, j = 0; i < alineSeqCnt && j < refSeqCnt; i++, j++)
            {

                scoringMatrix[i, j] = 0;
            }

        }

        public static int findSimScore(string refSeq, string alineSeq)//dict, OCR
        {
            try
            {
                refSeq = refSeq.ToLower();
                alineSeq = alineSeq.ToLower();


                string weightsPath = ConfigurationSettings.AppSettings["DictionaryWeightsPath"] != "" ? ConfigurationSettings.AppSettings["DictionaryWeightsPath"] : "";
                string[] lines = System.IO.File.ReadAllLines(weightsPath);

                initSimMatrix(refSeq, alineSeq);
                for (int i = 1; i < alineSeqCnt; i++)
                {
                    int t = 0;
                    for (int j = 1; j < refSeqCnt; j++)
                    {
                        int scroeDiag = 0;
                        if (refSeq.Substring(j - 1, 1) == alineSeq.Substring(i - 1, 1))
                            scroeDiag = scoringMatrix[i - 1, j - 1] + matchWeights;
                        else
                        {
                            int scoreval;
                            string prevchar1, prevchar2;

                            if (j > 1)
                                prevchar1 = refSeq.Substring(j - 2, 1);
                            else
                                prevchar1 = string.Empty;

                            if (i > 1)
                                prevchar2 = alineSeq.Substring(i - 2, 1);
                            else
                                prevchar2 = string.Empty;

                            scoreval = GetScore(lines, refSeq.Substring(j - 1, 1), prevchar1, alineSeq.Substring(i - 1, 1), prevchar2);

                            scroeDiag = scoringMatrix[i - 1, j - 1] + scoreval;

                        }

                        int scroeLeft = scoringMatrix[i, j - 1] + addWeights; //insert
                        int scroeUp = scoringMatrix[i - 1, j] + deleteWeights; //delete

                        int maxScore = Math.Max(Math.Max(scroeDiag, scroeLeft), scroeUp);

                        scoringMatrix[i, j] = maxScore;
                    }
                }
                //    findAlignment(refSeq,alineSeq);
                return scoringMatrix[alineSeqCnt - 1, refSeqCnt - 1];
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private static void findAlignment(string refSeq, string alineSeq) //not used
        {
            //Traceback Step
            char[] alineSeqArray = alineSeq.ToCharArray();
            char[] refSeqArray = refSeq.ToCharArray();

            string AlignmentA = string.Empty;
            string AlignmentB = string.Empty;
            int m = alineSeqCnt - 1;
            int n = refSeqCnt - 1;

            while (m > 0 || n > 0)
            {
                int scroeDiag = 0;

                if (m == 0 && n > 0)
                {
                    AlignmentA = refSeqArray[n - 1] + AlignmentA;
                    AlignmentB = "-" + AlignmentB;
                    n = n - 1;
                }
                else if (n == 0 && m > 0)
                {
                    AlignmentA = "-" + AlignmentA;
                    AlignmentB = alineSeqArray[m - 1] + AlignmentB;
                    m = m - 1;
                }
                else
                {
                    //Remembering that the scoring scheme is +3 for a match, -2 for a mismatch, and -1 for a gap and +2 for matrix match
                    if (alineSeqArray[m - 1] == refSeqArray[n - 1])
                        scroeDiag = 3;
                    else
                        scroeDiag = -2;

                    if (m > 0 && n > 0 && scoringMatrix[m, n] == scoringMatrix[m - 1, n - 1] + scroeDiag)
                    {
                        AlignmentA = refSeqArray[n - 1] + AlignmentA;
                        AlignmentB = alineSeqArray[m - 1] + AlignmentB;
                        m = m - 1;
                        n = n - 1;
                    }
                    else if (n > 0 && scoringMatrix[m, n] == scoringMatrix[m, n - 1] - 1)
                    {
                        AlignmentA = refSeqArray[n - 1] + AlignmentA;
                        AlignmentB = "-" + AlignmentB;
                        n = n - 1;
                    }
                    else if (m > 0 && scoringMatrix[m, n] == scoringMatrix[m - 1, n] + -1)
                    {
                        AlignmentA = "-" + AlignmentA;
                        AlignmentB = alineSeqArray[m - 1] + AlignmentB;
                        m = m - 1;
                    }
                    /* else //2 char in alignseq map to 1 char in refseq
                     {
                         AlignmentA = refSeqArray[n - 1] + AlignmentA;
                         AlignmentB = alineSeqArray[m - 1] + alineSeqArray[m - 2] + AlignmentB;
                         n = n - 1;
                         m = m - 2;
                     }*/
                }
            }
        }

        private static int GetScore(string[] lines, string currentchar1, string prevchar1, string currentchar2, string prevchar2)
        {

            foreach (string line in lines)
            {
                string[] combinationChars = line.Split(' ');
                if (combinationChars[0] == currentchar1 && combinationChars[1] == currentchar2)
                {
                    matchcharcount = 1;
                    return matchdefinedWeights;
                }

                if (combinationChars[0] == String.Concat(prevchar1, currentchar1) && combinationChars[1] == currentchar2)
                {
                    matchcharcount = 2;
                    return matchdefinedWeights;
                }

                if (combinationChars[1] == String.Concat(prevchar1, currentchar1) && combinationChars[0] == currentchar2)
                {
                    matchcharcount = 2;
                    return matchdefinedWeights;
                }

                if (combinationChars[0] == currentchar1 && combinationChars[1] == String.Concat(prevchar2, currentchar2))
                {
                    matchcharcount = 2;
                    return matchdefinedWeights;
                }

            }
            return editWeights;
        }

    }
}
