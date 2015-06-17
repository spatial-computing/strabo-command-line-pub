using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using Accord.Statistics.Analysis;

namespace Strabo.Test
{
    class ScalingImage
    {
        MulticlassSupportVectorMachine ksvm;

        public  ScalingImage(string TrainedDataInputFile)
        {
            string[] TrainedData = Directory.GetFiles(TrainedDataInputFile, "*.png");
            double[][] inputs=new double[TrainedData.Length][];   /// 
            double[] InputArray=new double[784];
            int[] Outputs=new int[TrainedData.Length];

            for (int i=0;i<TrainedData.Length;i++)
            {
                string filename = Path.GetFileNameWithoutExtension(TrainedData[i]);
                Bitmap TrainingImage=new Bitmap(TrainedData[i]);
                string[] split = filename.Split('.');
                for (int j=0;j<28;j++)
                {
                    for (int k=0;k<28;k++)
                    {
                        if ((!TrainingImage.GetPixel(j, k).Name.Equals("ffffffff")))
                            InputArray[j * 28 + k] = 1;
                        else
                            InputArray[j * 28 + k] = 0;
                    }
                }

                inputs[i] = InputArray;
                Outputs[i] = Convert.ToInt32(split[0]);
               InputArray=new double[784];
            }

            IKernel kernel;
            kernel=new Polynomial(2, 0);
            ksvm = new MulticlassSupportVectorMachine(784, kernel, 2);
            MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(ksvm, inputs, Outputs);

            double complexity = 1;   ///// set these three parameters Carefuly later
            double epsilon = 0.001;
            double tolerance = 0.2;

            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs);
                smo.Complexity = complexity;  /// Cost parameter for SVM
                smo.Epsilon = epsilon;
                smo.Tolerance = tolerance;
                return smo;
            };

            // Train the machines. It should take a while.
            double error = ml.Run();

        /// These lines are for testing which don't need here
            //int[] excpected = Outputs;
            //int[] actual = new int[excpected.Length];

            //for (int i=0;i<inputs.Length;i++)
            //{
            //    actual[i] = (int)ksvm.Compute(inputs[i]);
            //}
            //ConfusionMatrix confusionMatrix = new ConfusionMatrix(actual, excpected);
        }

        public void Scaling()
        {
            Bitmap sourceImage = new Bitmap(@"C:\Users\nhonarva\Documents\strabo-command-line-master\strabo-command-line-master\Strabo.CommandLine\data\intermediate\test1.png");
            Bitmap DestinationImage = new Bitmap(28,28);
            int[,] SourcePoints = new int[sourceImage.Width,sourceImage.Height];
            int[,] DestinationPoints = new int[28, 28];

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    if (!sourceImage.GetPixel(i, j).Name.Equals("ffffffff"))
                        SourcePoints[i, j] = 1;
                    else
                        SourcePoints[i, j] = 0;
                }
            }


            double XConvertor= (double)(sourceImage.Width)/(double)(28);
            double YConvertor = (double)(sourceImage.Height) / (double)(28);

            double[] input = new double[784];

            for (int i=0;i<28;i++)
            {
                for (int j=0;j<28;j++)
                {
                    double XInSourceImage=(double)(XConvertor*i);
                    double YInSourceImage=(double)(YConvertor*j);
                    int X=(int)(Math.Floor(XInSourceImage));
                    int Y=(int)(Math.Floor(YInSourceImage));
                    DestinationPoints[i,j]=SourcePoints[X,Y];
                    input[i * 28 + j] = DestinationPoints[i, j];
                }
            }



          //  int acutual = (int)ksvm.Compute(input);
           

                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                    //    if (DestinationPoints[i, j] == 1)
                        if (DestinationPoints[i, j] == 1)
                            DestinationImage.SetPixel(i, j, Color.Black);
                        else
                            DestinationImage.SetPixel(i, j, Color.White);
                    }
                }
            DestinationImage.Save(@"C:\Users\nhonarva\Documents\scaling.png");

            int acutual = (int)ksvm.Compute(input);
        }
    }
}
