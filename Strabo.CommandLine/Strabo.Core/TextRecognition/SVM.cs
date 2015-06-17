using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using System;
using System.Drawing;
using System.IO;
using Tesseract;

namespace Strabo.Core.TextRecognition
{
    class SVM
    {
        MulticlassSupportVectorMachine ksvm;
        private TesseractEngine _engine;
        Page page;
        public SVM(string TrainedDataInputFile)  //// Do training for all existing trained Data
        {

            _engine = new TesseractEngine(@"./tessdata3", "eng", EngineMode.TesseractAndCube);
            _engine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            _engine.SetVariable("tessedit_char_blacklist", "¢§+~»~`!@#$%^&*()_+-={}[]|\\:\";\'<>?,./");

            string[] TrainedData = Directory.GetFiles(TrainedDataInputFile, "*.png");
            double[][] inputs = new double[TrainedData.Length][];   /// 
            double[] InputArray = new double[784];
            int[] Outputs = new int[TrainedData.Length];

            for (int i = 0; i < TrainedData.Length; i++)
            {
                string filename = Path.GetFileNameWithoutExtension(TrainedData[i]);
                Bitmap TrainingImage = new Bitmap(TrainedData[i]);
                string[] split = filename.Split('.');
                for (int j = 0; j < 28; j++)
                {
                    for (int k = 0; k < 28; k++)
                    {
                        if ((!TrainingImage.GetPixel(j, k).Name.Equals("ffffffff")))
                            InputArray[j * 28 + k] = 1;
                        else
                            InputArray[j * 28 + k] = 0;
                    }
                }

                inputs[i] = InputArray;
                Outputs[i] = Convert.ToInt32(split[0]);
                InputArray = new double[784];
            }

            IKernel kernel;
            kernel = new Polynomial(2, 0);
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
        }

        public Boolean Classification(int[,] AddedAreaPoints, string ImageID)
        {
          //  Bitmap DestinationImage = new Bitmap(28, 28);      /// To see Result of Scaling
            Bitmap DestinationImage = new Bitmap(AddedAreaPoints.Rows(), AddedAreaPoints.Columns());
            int[,] DestinationPoints = new int[28, 28];        /// To see Result of Scaling

            double XConvertor = (double)(AddedAreaPoints.Rows()) / (double)(28);
            double YConvertor = (double)(AddedAreaPoints.Columns()) / (double)(28);

            double[] input = new double[784];

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    double XInSourceImage = (double)(XConvertor * i);
                    double YInSourceImage = (double)(YConvertor * j);
                    int X = (int)(Math.Floor(XInSourceImage));
                    int Y = (int)(Math.Floor(YInSourceImage));
                    input[i * 28 + j] = AddedAreaPoints[X, Y];
                    DestinationPoints[i, j] = AddedAreaPoints[X, Y];
                }
            }


          //  for (int i = 0; i < 28; i++)
                for (int i = 0; i < AddedAreaPoints.Rows(); i++)
            {
              //  for (int j = 0; j < 28; j++)
                    for (int j = 0; j < AddedAreaPoints.Columns(); j++)
                {
                  //  if (DestinationPoints[i, j] == 1)
                        if (AddedAreaPoints[i, j] == 1)
                        DestinationImage.SetPixel(i, j, Color.Black);
                    else
                        DestinationImage.SetPixel(i, j, Color.White);
                }
            }

            DestinationImage.Save(@"C:\Users\nhonarva\Documents\ResultsOfScaling\scaling" + ImageID + ".png");
            var image = Pix.LoadFromFile(@"C:\Users\nhonarva\Documents\ResultsOfScaling\scaling" + ImageID + ".png");
          //  Page page;
            page=_engine.Process(image, PageSegMode.SingleBlock);
            string text = page.GetText();
            double confidence = page.GetMeanConfidence();
            page.Dispose();

            int actual = (int)ksvm.Compute(input);
            if (actual == 1)
                return true;
            else
                return false;
        }
    }
}
