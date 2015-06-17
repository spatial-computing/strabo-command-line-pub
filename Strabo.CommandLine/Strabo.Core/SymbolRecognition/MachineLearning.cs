using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;
using Emgu.CV.CvEnum;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

namespace Strabo.Core.SymbolRecognition
{
    public class MachineLearning
    {
        #region SURF Matching
        public Tuple<Image<Bgr, byte>, HomographyMatrix> SURFMatcher_KNN(Image<Gray, byte> model, Image<Gray, byte> observed, SURFDetector surfCPU, List<VectorOfKeyPoint> keyPointsList, double uniquenessThreshold, int TM)
        {
            HomographyMatrix homography = null;
            Image<Bgr, Byte> result = null;
            VectorOfKeyPoint modelKeyPoints = keyPointsList.First<VectorOfKeyPoint>();
            VectorOfKeyPoint observedKeyPoints = keyPointsList.Last<VectorOfKeyPoint>(); ;
            Matrix<int> indices;
            Matrix<byte> mask;
            int k = 2;
            BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);


            try
            {

                result = observed.Convert<Bgr, byte>();
                Matrix<float> modelDescriptors = surfCPU.ComputeDescriptorsRaw(model, null, modelKeyPoints);
                matcher.Add(modelDescriptors);
                if (observedKeyPoints.Size > 0)
                {
                    Matrix<float> observedDescriptors = surfCPU.ComputeDescriptorsRaw(observed, null, observedKeyPoints);
                    indices = new Matrix<int>(observedDescriptors.Rows, k);

                    using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
                    {
                        matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                        mask = new Matrix<byte>(dist.Rows, 1);
                        mask.SetValue(255);
                        Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
                    }

                    int nonZeroCount = CvInvoke.cvCountNonZero(mask);
                    if (nonZeroCount >= TM)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                        if (nonZeroCount >= TM)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                    }
                    result = Features2DToolbox.DrawMatches(model, modelKeyPoints, observed, observedKeyPoints,
                           indices, new Bgr(100, 200, 214), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

                }
                return new Tuple<Image<Bgr, byte>, HomographyMatrix>(result, homography);
            }
            catch (Exception e)
            {
                throw e;
            }



        }
        #endregion


        #region Histogram Matching

        public Tuple<Image<Bgr, byte>, float[]> ApplyHistogramMatching(out Tuple<Image<Bgr, byte>, Rectangle, double> tup,
               Tuple<Image<Bgr, byte>, HomographyMatrix> homo1, Image<Gray, byte> modelImage, Image<Gray, byte> observedImage,
               Image<Bgr, byte> modifiedImage, string path, int counter, int x0, int y0, double HistogramMatchingScore)
        {
            Rectangle recShift;
            Stopwatch watch;
            Image<Bgr, Byte> result;
            double bpScore = 1;
            try
            {
                recShift = new Rectangle(0, 0, 0, 0);
                tup = null;
                watch = Stopwatch.StartNew();
                result = observedImage.Convert<Bgr, byte>();

                if (homo1.Item2 != null)//homography!= null)/
                {
                    Bitmap bD = homo1.Item1.ToBitmap(); //resultSurf.ToBitmap();//
                    using (Graphics gD = Graphics.FromImage(bD))
                    {
                        (observedImage.Sobel(1, 0, 3).AbsDiff(new Gray(0)) + observedImage.Sobel(0, 1, 3).AbsDiff(new Gray(0))).Save(string.Format("{0}{1}/captured/oedge{2}.jpg", path, "", counter.ToString("D5")));

                        tup = btnCalcBackProjectPatch(modelImage, observedImage, homo1.Item1);//resultSurf); //homo1.Item1);
                        Bitmap bS = tup.Item1.ToBitmap();
                        Rectangle rec = tup.Item2;

                        gD.DrawImage(bS, new Rectangle(observedImage.Width + 1, modelImage.Height + 1, modelImage.Width, modelImage.Height), new Rectangle(0, 0, modelImage.Width, modelImage.Height), GraphicsUnit.Pixel);

                        result = new Image<Bgr, byte>(bD);

                        //capturedLog.Write("\n#" + counter.ToString("D5"));

                        bpScore = tup.Item3;
                        //coordinatesOnMap.WriteLine("Red rectangles");
                        //coordinatesOnMap.WriteLine("");

                        if (bpScore >= HistogramMatchingScore)
                        {

                            result.Draw(rec, new Bgr(Color.Red), 2);
                            recShift = new Rectangle(new Point(x0 + rec.X, y0 + rec.Y), rec.Size);

                            //capturedLog.Write("\t" + tup.Item3.ToString());
                            // capture the points matched with bpScore > 0.7 saved as file  output<{0}/{1}/coordinates.txt>
                            // coordinatesOnMap.WriteLine("x=" + x0 + rec.X + ", y= " + y0 + rec.Y + "");

                        }

                        result.Save(string.Format("{0}{1}/captured/{2}.jpg", path, "", counter.ToString("D5")));
                    }
                }

                result.Save(string.Format("{0}{1}/out/{2}.jpg", path, counter.ToString("D5"), ""));
                //capturedLog.Close();
                // coordinatesOnMap.Close();
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
            }
            catch (Exception)
            {

                throw;
            }
            if (recShift.Width > 0 && bpScore > HistogramMatchingScore)
            {
                return new Tuple<Image<Bgr, byte>, float[]>(modifiedImage, new float[] { recShift.X, recShift.Y, (float)bpScore });
            }
            else
            {
                return new Tuple<Image<Bgr, byte>, float[]>(modifiedImage, new float[] { 0, 0, 0 });
            }


        }
        private static Tuple<Image<Bgr, byte>, Rectangle, double> btnCalcBackProjectPatch(Image<Gray, byte> model, Image<Gray, byte> observed, Image<Bgr, byte> result)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Image<Bgr, Byte> imageSource = observed.Convert<Bgr, byte>();
            double scale = GetScale();
            Image<Bgr, Byte> imageSource2 = imageSource.Resize(scale, INTER.CV_INTER_LINEAR);
            Image<Bgr, Byte> imageTarget = model.Convert<Bgr, byte>();
            Image<Bgr, Byte> imageTarget2 = imageTarget.Resize(scale, INTER.CV_INTER_LINEAR);
            Image<Gray, Single> imageDest = null;
            int edgeX = imageTarget2.Size.Width;
            int edgeY = imageTarget2.Size.Height;
            Size patchSize = new Size(edgeX, edgeY);
            string colorSpace = "Gray";
            double normalizeFactor = 1d;

            Image<Gray, Byte>[] imagesSource;
            int[] bins;
            RangeF[] ranges;
            string[] captions;
            Color[] colors;

            // Get all the color bins from the source images.
            GetImagesBinsAndRanges(imageSource2, colorSpace, out imagesSource, out bins, out ranges, out captions, out colors);
            // Histogram calculation.
            DenseHistogram histTarget = CalcHist(imageTarget2.Bitmap);
            // Backproject compare method.
            HISTOGRAM_COMP_METHOD compareMethod = GetHistogramCompareMethod();
            imageDest = BackProjectPatch<Byte>(imagesSource, patchSize, histTarget, compareMethod, (float)normalizeFactor);
            // Best-match value.
            double bestValue;
            Point bestPoint;
            FindBestMatchPointAndValue(imageDest, compareMethod, out bestValue, out bestPoint);
            // Draw a rectangle with the same size of the template at the best-match point

            Rectangle rect = new Rectangle(bestPoint, patchSize);
            result = imageDest.Convert<Bgr, byte>();
            sw.Stop();

            // Disposal
            imageSource.Dispose();
            imageSource2.Dispose();
            imageTarget.Dispose();
            imageTarget2.Dispose();
            if (imageDest != null)
                imageDest.Dispose();
            foreach (Image<Gray, Byte> image in imagesSource)
                image.Dispose();
            histTarget.Dispose();

            return new Tuple<Image<Bgr, byte>, Rectangle, double>(result, rect, bestValue);
        }

        private static void GetImagesBinsAndRanges(Image<Bgr, Byte> imageSource, string colorSpace, out Image<Gray, Byte>[] images, out int[] bins, out RangeF[] ranges, out string[] captions, out Color[] colors)
        {
            images = null;
            bins = null;
            ranges = null;
            captions = null;
            colors = null;
            int channels = 0;
            Image<Gray, Byte> imageGray = null;
            Image<Gray, Byte> imageRed = null;
            Image<Gray, Byte> imageGreen = null;
            Image<Gray, Byte> imageBlue = null;
            Image<Gray, Byte> imageHue = null;
            Image<Gray, Byte> imageSaturation = null;
            Image<Gray, Byte> imageBrightness = null;
            if (colorSpace == "Gray")
            {
                channels = 1;
                imageGray = imageSource.Convert<Gray, Byte>();
            }
            else if (colorSpace == "Red")
            {
                channels = 1;
                Image<Gray, Byte>[] images2 = imageSource.Split();
                imageRed = images2[2];
            }
            else if (colorSpace == "RGB")
            {
                channels = 3;
                Image<Gray, Byte>[] images2 = imageSource.Split();
                imageRed = images2[2];
                imageGreen = images2[1];
                imageBlue = images2[0];
            }
            else if (colorSpace == "Hue")
            {
                channels = 1;
                Image<Hsv, Byte> imageHsv = imageSource.Convert<Hsv, Byte>();
                Image<Gray, Byte>[] images2 = imageHsv.Split();
                imageHue = images2[0];
            }
            else if (colorSpace == "H&S")
            {
                channels = 2;
                Image<Hsv, Byte> imageHsv = imageSource.Convert<Hsv, Byte>();
                Image<Gray, Byte>[] images2 = imageHsv.Split();
                imageHue = images2[0];
                imageSaturation = images2[1];
            }
            else if (colorSpace == "HSV")
            {
                channels = 3;
                Image<Hsv, Byte> imageHsv = imageSource.Convert<Hsv, Byte>();
                Image<Gray, Byte>[] images2 = imageHsv.Split();
                imageHue = images2[0];
                imageSaturation = images2[1];
                imageBrightness = images2[2];
            }
            if (channels > 0)
            {
                images = new Image<Gray, byte>[channels];
                bins = new int[channels];
                ranges = new RangeF[channels];
                captions = new string[channels];
                colors = new Color[channels];
                int idx = 0;
                if (imageGray != null)
                {
                    images[idx] = imageGray;
                    bins[idx] = 256;
                    ranges[idx] = new RangeF(0f, 255f);
                    captions[idx] = "Gray";
                    colors[idx] = Color.Black;
                    idx++;
                }
                if (imageRed != null)
                {
                    images[idx] = imageRed;
                    bins[idx] = 256;
                    ranges[idx] = new RangeF(0f, 255f);
                    captions[idx] = "Red";
                    colors[idx] = Color.Red;
                    idx++;
                }
                if (imageGreen != null)
                {
                    images[idx] = imageGreen;
                    bins[idx] = 256;
                    ranges[idx] = new RangeF(0f, 255f);
                    captions[idx] = "Green";
                    colors[idx] = Color.Green;
                    idx++;
                }
                if (imageBlue != null)
                {
                    images[idx] = imageBlue;
                    bins[idx] = 256;
                    ranges[idx] = new RangeF(0f, 255f);
                    captions[idx] = "Blue";
                    colors[idx] = Color.Blue;
                    idx++;
                }
                if (imageHue != null)
                {
                    images[idx] = imageHue;
                    bins[idx] = 180;
                    ranges[idx] = new RangeF(0f, 179f);
                    captions[idx] = "Hue";
                    colors[idx] = Color.BurlyWood;
                    idx++;
                }
                if (imageSaturation != null)
                {
                    images[idx] = imageSaturation;
                    bins[idx] = 256;
                    ranges[idx] = new RangeF(0f, 255f);
                    captions[idx] = "Saturation";
                    colors[idx] = Color.Chocolate;
                    idx++;
                }
                if (imageBrightness != null)
                {
                    images[idx] = imageBrightness;
                    bins[idx] = 256;
                    ranges[idx] = new RangeF(0f, 255f);
                    captions[idx] = "Brightness";
                    colors[idx] = Color.Crimson;
                    idx++;
                }
            }
        }

        // Histogram calculation.
        private static DenseHistogram CalcHist(Image image)
        {
            Image<Bgr, Byte> imageSource = new Image<Bgr, byte>((Bitmap)image);
            string colorSpace = "Gray";
            Image<Gray, Byte>[] images;
            int[] bins;
            RangeF[] ranges;
            string[] captions;
            Color[] colors;
            // Get all the color bins from the source images.
            GetImagesBinsAndRanges(imageSource, colorSpace, out images, out bins, out ranges, out captions, out colors);
            SetBins(ref bins);
            // Histogram calculation.
            DenseHistogram hist = new DenseHistogram(bins, ranges);
            hist.Calculate<Byte>(images, false, null);
            return hist;
        }

        // Histogram compare method.
        private static HISTOGRAM_COMP_METHOD GetHistogramCompareMethod()
        {
            HISTOGRAM_COMP_METHOD compareMethod = HISTOGRAM_COMP_METHOD.CV_COMP_CORREL;
            compareMethod = HISTOGRAM_COMP_METHOD.CV_COMP_CORREL;

            return compareMethod;
        }

        // Compares histogram over each possible rectangular patch of the specified size in the input images, and stores the results to the output map dst.
        // Destination back projection image of the same type as the source images
        private static Image<Gray, Single> BackProjectPatch<TDepth>(Image<Gray, TDepth>[] srcs, Size patchSize, DenseHistogram hist, HISTOGRAM_COMP_METHOD method, double factor) where TDepth : new()
        {
            Debug.Assert(srcs.Length == hist.Dimension, "The number of the source image and the dimension of the histogram must be the same.");

            IntPtr[] imgPtrs =
                Array.ConvertAll<Image<Gray, TDepth>, IntPtr>(
                    srcs,
                    delegate(Image<Gray, TDepth> img) { return img.Ptr; });
            Size size = srcs[0].Size;
            size.Width = size.Width - patchSize.Width + 1;
            size.Height = size.Height - patchSize.Height + 1;
            Image<Gray, Single> res = new Image<Gray, float>(size);
            CvInvoke.cvCalcBackProjectPatch(imgPtrs, res.Ptr, patchSize, hist.Ptr, method, factor);
            return res;
        }


        private static void SetBins(ref int[] bins)
        {
            int bin = 20;
            int.TryParse("25", out bin);
            for (int i = 0; i < bins.Length; i++)
                bins[i] = bin;
        }

        // Get the scaling factor.
        private static double GetScale()
        {
            double scale = 1d;
            double.TryParse("1", out scale);
            return scale;
        }

        // Find the best match point and its value.
        private static void FindBestMatchPointAndValue(Image<Gray, Single> image, HISTOGRAM_COMP_METHOD compareMethod, out double bestValue, out Point bestPoint)
        {
            bestValue = 0d;
            bestPoint = new Point(0, 0);
            double[] minValues, maxValues;
            Point[] minLocations, maxLocations;
            image.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
            if (compareMethod == HISTOGRAM_COMP_METHOD.CV_COMP_CHISQR || compareMethod == HISTOGRAM_COMP_METHOD.CV_COMP_BHATTACHARYYA)
            {
                bestValue = minValues[0];
                bestPoint = minLocations[0];
            }
            else
            {
                bestValue = maxValues[0];
                bestPoint = maxLocations[0];
            }
        }
        #endregion


        #region SVM
        public  int[]  ApplySVMByGussianKernel( double kernelDegree, double complexity, double epsilon, double tolerance,
            double[][] inputs, int[] outputs, int inputNumber, double[][] tests)
        {
            MulticlassSupportVectorMachine ksvm;
            int[] SVMoutputs = new Int32[tests.Length];
            IKernel kernel;
            kernel = new Gaussian(kernelDegree);
            ksvm = new MulticlassSupportVectorMachine(inputNumber, kernel, 2);
            MulticlassSupportVectorLearning ml = new MulticlassSupportVectorLearning(ksvm, inputs, outputs);
            ml.Algorithm = (svm, classInputs, classOutputs, i, j) =>
            {
                var smo = new SequentialMinimalOptimization(svm, classInputs, classOutputs);
                smo.Complexity = complexity;  /// Cost parameter for SVM
                smo.Epsilon = epsilon;
                smo.Tolerance = tolerance;
                return smo;
            };

            double error = ml.Run();
            for (int i = 0; i < tests.Length; i++)
                SVMoutputs[i] = (int)ksvm.Compute(tests[i]);
            return SVMoutputs;
        }

        #endregion
    }
}
