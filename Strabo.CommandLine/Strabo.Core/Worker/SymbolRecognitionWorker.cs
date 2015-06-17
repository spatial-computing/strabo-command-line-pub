///////////////////////////////////////////////////////////
////SymbolRecognitionWorker////////////////////////////////
////this class need three parameters to start//////////////
////Scannining Method: To Scan the Image///////////////////
////Descriptor: the method to Build the Feature Vector/////
////MachineLearning Method: to Classify the Feature Vector/
////12Feb, 2015, 11:00pm, Simam///////////////////////////////////
///////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing;
using Strabo.Core.SymbolRecognition;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using System.Threading;

namespace Strabo.Core.Worker
{

    public class SymbolRecognitionWorker
    {
        public enum ScanningMethod
        {
            ScanningByPixel,
            ScanningByImageDimention,

        }
        public enum DescriptorMethod
        {
            SIFT,
            SURF,
            DCT,

        }
        public enum MachineLearningMethod
        {
            KNN,
            KMEAN,
            SVM,
        }
        private ScanningMethod _scanningMethod;
        private DescriptorMethod _descriptorType;
        private MachineLearningMethod _machineLearningMethod;
        private Descriptor _descriptor;
        private MachineLearning _machineLearning;
        private ScanImage _scanImage;
        private Utility.SymbolParameters _symbolParameters;

        private string _inPath;
        private string _outPath;
        private string _intermediatePath;
        private string _inputPath;

        private Image<Bgr, Byte> _test;
        private Image<Bgr, Byte> _element;
        private Image<Gray, Byte> _gElement;
        private int _hessianThreshould;
        private double _uniquenessThresh;
        private int _tm;
        private double _histogramMatchingScore;


        public ScanningMethod scanningMethod { set { _scanningMethod = value; } }
        public DescriptorMethod descriptorMethod { set { _descriptorType = value; } }
        public MachineLearningMethod machineLearningMethod { set { _machineLearningMethod = value; } }
        private TextWriter _log;

        public void SymbolRecognitionBySVM(string positivePath, string negativePath, string testPath)
        {
            double[][] inputs, testInputs;
            int[] outputs;
            Image<Bgr, Byte>[] Positives, Negatives;
            Image<Bgr, Byte> test;
            int total = 0;
            int counter = 0;
            _scanImage = new ScanImage();
            _descriptor = new Descriptor();
            _scanImage.SetImages(positivePath, negativePath, testPath, out Positives, out Negatives, out test);
            if (Negatives != null && Negatives.Length != 0)
                total += Negatives.Length;
            if (Positives != null && Positives.Length != 0)
                total += Positives.Length;
            inputs = new double[total][];
            outputs = new Int32[total];

            if (Negatives != null || Negatives.Length != 0)
                for (int i = 0; i < Negatives.Length; i++)
                {
                    inputs[counter] = _descriptor.GetImageVector(Negatives[i], 100);
                    outputs[counter++] = 0;
                }
            if (Positives != null || Positives.Length != 0)
                for (int i = 0; i < Positives.Length; i++)
                {
                    inputs[counter] = _descriptor.GetImageVector(Positives[i], 100);
                    outputs[counter++] = 1;
                }

            _scanImage.ScanByPixel(@"C:\Users\simakmo\Documents\Sima\Symbol Reconition\data\", test, Positives[0], 50, 50);



            string[] filePaths = Directory.GetFiles(@"C:\Users\simakmo\Documents\Sima\Symbol Reconition\data\Intermediate", "*.jpg");
            testInputs = new double[filePaths.Length][];
             counter = 0;
            foreach (string path in filePaths)
                testInputs[counter++] = _descriptor.GetImageVector(new Image<Bgr, Byte>(path), 150);
            _machineLearning = new MachineLearning();

            int dimention = Positives[0].Height * Positives[0].Width;

            int[] testOutputs = _machineLearning.ApplySVMByGussianKernel(2, 2.5, 0.001, 0.2, inputs, outputs, dimention, testInputs);
            List<Point> results = new List<Point>();
            for (int j = 0; j < testOutputs.Length; j++)
            {
                int x, y, num;
                if (testOutputs[j] == 1)
                {
                    GetImageParameters(out x, out y, out num, filePaths[j]);
                    results.Add(new Point(x, y));
                }
            }
            Visualization.DrawResults(results, Positives[0].Size, test, testPath);
        }

        public SymbolRecognitionWorker()
        {

            //_scanImage = new ScanImage();

            //_scanImage.ScanByPixel(@"C:\Users\Sima\Documents\strabo-command-line\Strabo.CommandLine\data\Symbol\", 50, 50, out _element, out  _test, out _gElement, _log);

        }
        public SymbolRecognitionWorker(string inputPath, ScanningMethod scanningMethod, DescriptorMethod descriptorMethod, MachineLearningMethod machineLearningMethod)
        {



            #region SURF
            //SetParameters(inputPath, scanningMethod, descriptorMethod, machineLearningMethod);

            //switch (scanningMethod)
            //{
            //    case ScanningMethod.ScanningByPixel:
            //        break;
            //    case ScanningMethod.ScanningByImageDimention:
            // //      _scanImage.ScanByImageDimension(inputPath, out _element, out _test, out _gElement, _log);
            //        _scanImage.ScanByPixel(inputPath, 50, 50, out _element, out _test, out _gElement, _log);
            //        break;
            //    default:
            //        break;
            //}
            //switch (descriptorMethod)
            //{
            //    case DescriptorMethod.SIFT:
            //        break;
            //    case DescriptorMethod.SURF:
            //        ApplySURF();
            //        break;
            //    case DescriptorMethod.DCT:
            //        break;
            //    default:
            //        break;
            //}
            #endregion

        }

        private void SetParameters(string inputPath, ScanningMethod scanningMethod, DescriptorMethod descriptorMethod, MachineLearningMethod machineLearningMethod)
        {
            _symbolParameters = new Utility.SymbolParameters();
            _hessianThreshould = _symbolParameters.HessianThresh;
            _uniquenessThresh = _symbolParameters.UniquenessThress;
            _histogramMatchingScore = _symbolParameters.HistogramMatchingScore;
            _tm = _symbolParameters.TM;

            _log = File.AppendText(inputPath + "/log.txt");


            _machineLearningMethod = machineLearningMethod;
            _scanningMethod = scanningMethod;
            _descriptorType = descriptorMethod;
            //check the FilePath 
            _inputPath = inputPath;
            _inPath = inputPath + "/in";
            _outPath = inputPath + "/out";
            _intermediatePath = inputPath + "/Intermediate";

            ValidateFilePath(_inPath, "in");
            ValidateFilePath(_outPath, "out");
            ValidateFilePath(_intermediatePath, "Intermediate");
            ValidateFilePath(_inputPath, "");

            _descriptor = new Descriptor();
            _machineLearning = new MachineLearning();
            _scanImage = new ScanImage();
        }
        private void ValidateFilePath(string path, string name)
        {
            if (name != "" && name == "Intermediate" && Directory.Exists(path))
                Directory.Delete(path, true);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

        }
        private void ApplySURF()
        {


            List<VectorOfKeyPoint> keyPointsList;
            SURFDetector surfCPU;
            Tuple<Image<Bgr, byte>, HomographyMatrix> homo1;
            Tuple<Image<Bgr, byte>, Rectangle, double> tup;

            Stopwatch watch = Stopwatch.StartNew();
            Image<Bgr, Byte> pTest;
            Tuple<Image<Bgr, byte>, float[]> drawResult = null;
            ArrayList allMatches = new ArrayList();
            int x, y, counter;
            string[] filePaths = Directory.GetFiles(_intermediatePath, "*.jpg");
            foreach (string path in filePaths)
            {
                GetImageParameters(out x, out y, out counter, path);
                pTest = new Image<Bgr, byte>(path);

                keyPointsList = _descriptor.SURF_BruteForceMatcher(_gElement, pTest.Convert<Gray, Byte>(), _hessianThreshould, out surfCPU);
                homo1 = _machineLearning.SURFMatcher_KNN(_gElement, pTest.Convert<Gray, Byte>(), surfCPU, keyPointsList, _uniquenessThresh, _tm);
                drawResult = _machineLearning.ApplyHistogramMatching(out tup, homo1, _gElement, pTest.Convert<Gray, Byte>(), _test, _inputPath, counter, x, y, _histogramMatchingScore);

                if (drawResult != null)
                {
                    if (drawResult.Item2[2] > 0)
                    {
                        allMatches.Add(drawResult.Item2);
                        _log.WriteLine(string.Format("\n\tSW2 location: ({0}, {1})\n\tHistogram score: {2}]", drawResult.Item2[0], drawResult.Item2[1], drawResult.Item2[2]));
                    }
                }

            }
            if (allMatches != null)
            {
                _log.WriteLine("The count before consolidation: " + allMatches.Count);



                ////////  Consolidate
                //HashSet<float[]> hash0 = Consolidation.Consolidate(allMatches, _gElement.Width - 1, _gElement.Height - 1, _log);
                //ArrayList al = new ArrayList();

                //foreach (float[] i in hash0)
                //    al.Add(i);

                //HashSet<float[]> hash = Consolidation.Consolidate(al, _gElement.Width - 1, _gElement.Height - 1, _log);


                ///////Drawing
                //_log.WriteLine("The count after consolidation: " + hash.Count);
                //Visualization.DrawingResults(hash, _gElement, _test, _inputPath);
                Visualization.DrawingResults(allMatches, _gElement, _test, _inputPath);
            }

            watch.Stop();
            _log.WriteLine(watch.Elapsed);
            _log.Close();



        }
        private void GetImageParameters(out int x, out int y, out int counter, string path)
        {
            string[] parts = path.Split('/');
            string name = parts[parts.Length - 1];
            name = name.Substring(0, name.IndexOf('.'));
            parts = name.Split('-');
            x = Int32.Parse(parts[1]);
            y = Int32.Parse(parts[2]);
            counter = Int32.Parse(parts[3]);


        }
        private void SURFThread(int x, int y, int counter, Image<Bgr, byte> pTest, int i, out Tuple<Image<Bgr, byte>, float[]> result)
        {
            List<VectorOfKeyPoint> keyPointsList;
            SURFDetector surfCPU;
            Tuple<Image<Bgr, byte>, HomographyMatrix> homo1;
            Tuple<Image<Bgr, byte>, Rectangle, double> tup;

            keyPointsList = _descriptor.SURF_BruteForceMatcher(_gElement, pTest.Convert<Gray, Byte>(), _hessianThreshould + i * 50, out surfCPU);
            homo1 = _machineLearning.SURFMatcher_KNN(_gElement, pTest.Convert<Gray, Byte>(), surfCPU, keyPointsList, _uniquenessThresh, _tm);
            result = _machineLearning.ApplyHistogramMatching(out tup, homo1, _gElement, pTest.Convert<Gray, Byte>(), _test, _inputPath, counter, x, y, _histogramMatchingScore);

        }


        #region Old Version Which works by DrawMatching Class
        public void Apply(string inputPath, double uniquenessThresh, int tm, int hessianThresh)
        {


            Stopwatch watch = Stopwatch.StartNew();
            ArrayList allMatches = new ArrayList();
            Tuple<Image<Bgr, byte>, float[]> drawResult;
            float[] recStat;
            // Handling file names.



            string topic = "";
            TextWriter log = File.AppendText(inputPath + topic + "/log.txt");

            // Read images.
            Image<Bgr, Byte> element = new Image<Bgr, Byte>(string.Format("{0}{1}/in/element.png", inputPath, topic));
            Image<Bgr, Byte> test = new Image<Bgr, Byte>(string.Format("{0}{1}/in/test.png", inputPath, topic));
            Bitmap window = test.ToBitmap();

            // Convert to gray-level images and save.
            Image<Gray, Byte> gElement = element.Convert<Gray, Byte>();
            Image<Gray, Byte> gTest = test.Convert<Gray, Byte>();
            gElement.Save(string.Format("{0}{1}/in/g-element.png", inputPath, topic));
            gTest.Save(string.Format("{0}{1}/in/g-test.png", inputPath, topic));

            // Get image dimensions.
            int wfactor = 2;
            // The size of the element image.
            int ex = element.Width;
            int ey = element.Height;
            // The size of the test image.
            int tx = test.Width;
            int ty = test.Height;
            // The distance that the sliding window shifts.
            int xshift = tx / ex / wfactor * 2 - 1;
            int yshift = ty / ey / wfactor * 2 - 1;

            log.WriteLine(string.Format("Element Image: ({0}*{1})\nTest Image:({2}*{3})\n", ex, ey, tx, ty));

            for (int j = 0; j < yshift; j++)
            {
                for (int i = 0; i < xshift; i++)
                {
                    int xstart = i * ex * wfactor / 2;
                    int ystart = j * ey * wfactor / 2;
                    int counter = i + j * xshift;

                    Rectangle r = new Rectangle(xstart, ystart, ex * wfactor, ey * wfactor);
                    Image<Bgr, Byte> pTest = new Image<Bgr, Byte>(window.Clone(r, window.PixelFormat));
                    pTest.Save(string.Format("{0}{1}/in/part.jpg", inputPath, topic));

                    drawResult = DrawMatches.Draw(gElement, pTest.Convert<Gray, Byte>(), test, xstart, ystart, inputPath, topic, counter, log, uniquenessThresh, tm, hessianThresh);
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    log.WriteLine(string.Format("\n\nSub-image #{0}:\n\tLoop #({1}, {2})\n\tSW1 location: ({3}, {4})", counter, i, j, xstart, ystart));
                    test = drawResult.Item1;
                    recStat = drawResult.Item2;
                    if (recStat[2] > 0)
                    {
                        allMatches.Add(recStat);
                        log.WriteLine(string.Format("\n\tSW2 location: ({0}, {1})\n\tHistogram score: {2}]", recStat[0], recStat[1], recStat[2]));
                    }


                }
            }

            log.WriteLine("The count before consolidation: " + allMatches.Count);

            HashSet<float[]> hash0 = DrawMatches.Consolidate(allMatches, gElement.Width - 1, gElement.Height - 1, log);
            ArrayList al = new ArrayList();

            foreach (float[] i in hash0)
                al.Add(i);


            HashSet<float[]> hash = DrawMatches.Consolidate(al, gElement.Width - 1, gElement.Height - 1, log);

            log.WriteLine("The count after consolidation: " + hash.Count);
            //Blue 
            TextWriter coordinatesOnMapBlue = File.AppendText(inputPath + topic + "/coordinatesOnMapBlue.txt");
            foreach (float[] i in hash)
            {
                test.Draw(new Rectangle(new Point((int)i[0], (int)i[1]), gElement.Size), new Bgr(Color.Blue), 5);
                coordinatesOnMapBlue.WriteLine("x= " + (int)i[0] + ", y=" + (int)i[1] + "");
            }
            coordinatesOnMapBlue.Close();
            test.Save(string.Format("{0}{1}/out.jpg", inputPath, topic));

            watch.Stop();
            log.WriteLine(watch.Elapsed);

            log.Close();


        }
        #endregion
    }
}
