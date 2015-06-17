using Strabo.Core.Utility;
using System;

namespace Strabo.Core.Worker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length < 6)
            {
                Log.WriteLine("Input parameter is invalid. Please use \"strabo.core.exe double west_x, double north_y, string intermediate_folder, string output_folder, string layer, int thread_number\"");
                return;
            }
            try
            {
                BoundingBox bbx = new BoundingBox();
                bbx.BBW = args[0];
                bbx.BBN = args[1];
                InputArgs inputArgs = new InputArgs();
                inputArgs.outputFileName = "geojson";
                inputArgs.bbx = bbx;
                inputArgs.intermediatePath = args[2];
                inputArgs.outputPath = args[3];
                inputArgs.mapLayerName = args[4];
                inputArgs.threadNumber = Int32.Parse(args[5]);

                CommandLineWorker cmdWorker = new CommandLineWorker();
                cmdWorker.Apply(inputArgs, false);
                Log.WriteLine("Strabo process finised.");
            }
            catch(Exception e)
            {
                Log.WriteLine(e.Message);
                Log.WriteLine(e.Source);
                Log.WriteLine(e.StackTrace);
            }
        }
    }
}
