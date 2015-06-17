using Strabo.Core.Utility;
using Strabo.Core.Worker;
using System;
using System.IO;

namespace Strabo.Test
{
    public class TestCommandLineWorker
    {
        public TestCommandLineWorker() { }
        public void Test()//do not change this function, write your own test function instead
        {
            //try
            //{
            //    Test("nls-sixinch");
            //}
            //catch (Exception e) { };
            //try
            //{
            //    Test("nls-cs-1920");
            //}
            //catch (Exception e) { };
            try
            {
                Test("Tianditu_cva");
            }
            catch (Exception e) { };
        }
        public void Test(string layer)//do not change this function, write your own test function instead
        {
            BoundingBox bbx = new BoundingBox();
            InputArgs inputArgs = new InputArgs();
            inputArgs.outputFileName = "layer";

            if (layer == "nls-sixinch")
            {
                inputArgs.mapLayerName = layer;
                bbx.BBW = "597528";
                bbx.BBN = "210822";
                //417600 274400
                //482400 262400
                //412000 202400
                //488000 221600

                //417600 275200
                //482400 263200
                //412000 203200
                //488000 222400
            }
            if (layer == "nls-cs-1920")
            {
                inputArgs.mapLayerName = layer;
                bbx.BBW = "2977681";
                bbx.BBN = "5248978";
            }
            if (layer == "Tianditu_cva")
            {
                inputArgs.mapLayerName = layer;
                bbx.BBW = "13006520";
                bbx.BBN = "4853838";
            }
            if (layer == "Tianditu_eva")
            {
                inputArgs.mapLayerName = layer;
                bbx.BBW = "12957312";
                bbx.BBN = "4852401";
            }
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Directory.GetParent(appPath).Parent.Parent.FullName + "\\data\\";

            inputArgs.bbx = bbx;

            inputArgs.intermediatePath = dataPath + "intermediate\\";
            inputArgs.outputPath = dataPath + "output\\";

            inputArgs.threadNumber = 8;

            CommandLineWorker cmdWorker = new CommandLineWorker();
            cmdWorker.Apply(inputArgs, false);
            //cmdWorker.Apply(inputArgs, true);
            File.Copy(inputArgs.outputPath + "SourceMapImage.png", dataPath + layer + ".png", true);
            File.Copy(inputArgs.outputPath + layer + "ByPixels.txt", dataPath + layer + "geojsonByPixels.txt", true);
            Log.WriteLine("Process finished");
        }
        public void TestLocalFiles()//do not change this function, write your own test function instead
        {
            BoundingBox bbx = new BoundingBox();
            bbx.BBW = "2977681";
            bbx.BBN = "5248978";

            InputArgs inputArgs = new InputArgs();
            inputArgs.outputFileName = "Geojson";

            inputArgs.bbx = bbx;
            inputArgs.mapLayerName = "nls-cs-1920";
            inputArgs.threadNumber = 8;

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Directory.GetParent(appPath).Parent.Parent.FullName + "\\data\\";

            inputArgs.bbx = bbx;

            inputArgs.intermediatePath = dataPath + "intermediate\\";
            inputArgs.outputPath = dataPath + "output\\";
            try
            {
                CommandLineWorker cmdWorker = new CommandLineWorker();
                for (int i = 1; i < 11; i++)
                {
                    //if (i != 10) continue;
                    File.Copy(dataPath + "1920-" + i + ".png", inputArgs.outputPath + "SourceMapImage.png", true);
                    inputArgs.outputFileName = "1920-" + i + ".png";
                    cmdWorker.Apply(inputArgs, true);
                    File.Copy(inputArgs.outputPath + "1920-" + i + ".pngByPixels.txt", dataPath + "1920-" + i + ".pngByPixels.txt", true);
                }
            }
            catch (Exception e) { Log.WriteLine(e.Message); Log.WriteLine(e.StackTrace); };
            Log.WriteLine("Process finished");
        }
        public void TestLocalTianditu_evaFiles()//do not change this function, write your own test function instead
        {
            BoundingBox bbx = new BoundingBox();
            bbx.BBW = "2977681";
            bbx.BBN = "5248978";

            InputArgs inputArgs = new InputArgs();
            inputArgs.outputFileName = "Geojson";

            inputArgs.bbx = bbx;
            inputArgs.mapLayerName = "Tianditu_eva";
            inputArgs.threadNumber = 8;

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string dataPath = Directory.GetParent(appPath).Parent.Parent.FullName + "\\data\\";

            inputArgs.bbx = bbx;

            inputArgs.intermediatePath = dataPath + "intermediate\\";
            inputArgs.outputPath = dataPath + "output\\";
            try
            {
                CommandLineWorker cmdWorker = new CommandLineWorker();
                //for (int i = 1; i < 11; i++)
                {
                    //if (i != 10) continue;
                    File.Copy(dataPath + "Tianditu_eva.png", inputArgs.outputPath + "SourceMapImage.png", true);
                    inputArgs.outputFileName = "Tianditu_eva.png";
                    cmdWorker.Apply(inputArgs, true);
                    File.Copy(inputArgs.outputPath + "Tianditu_eva.pngByPixels.txt", dataPath + "Tianditu_eva.pngByPixels.txt", true);
                }
            }
            catch (Exception e) { Log.WriteLine(e.Message); Log.WriteLine(e.StackTrace); };
            Log.WriteLine("Process finished");
        }
    }
}
