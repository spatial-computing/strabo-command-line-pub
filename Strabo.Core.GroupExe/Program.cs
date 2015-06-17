using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Strabo.Core.GroupExe
{
    class Program
    {
        static void Main(string[] args)
        {

            //string path = @"C:\Users\simakmo\Documents\Visual Studio 2010\Projects\CLSL\Strabo.Core\bin\Debug";
            //StreamWriter sw = new StreamWriter(path + "CrowdedSamples.bat");
            //int w = 580000;
            //int n = 189000;
            //int w = 580000;
            //int n = 195000;
            int w = 588000;
            int n = 184000;

            string input = @"C:\Users\Sima\Documents\VisualStudio2013\Projects\CLSL\Strabo.Core.GroupExe\bin\Debug\Imagestest";
            string output = @"C:\Users\Sima\Documents\VisualStudio2013\Projects\CLSL\Strabo.Core.GroupExe\bin\Debug\Imagestest";

            ProcessStartInfo processInfo;
            Process process;
            for (int count = 51; count < 65; count++)
            {
                Console.WriteLine(DateTime.Now.ToString());
                processInfo = new ProcessStartInfo(@"C:\Users\Sima\Documents\VisualStudio2013\Projects\CLSL\Strabo.Core\bin\Debug\strabo.core.exe",
                    w.ToString() + " " + n.ToString() + " " +
                        input + "\\" + count.ToString() + "  " + output + "\\" + count.ToString() + "\\Data" + " " +
                        "nls-cs-1920" + " " + "4");

                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                // *** Redirect the output ***
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                try
                {
                    process = Process.Start(processInfo);
                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    throw e;
                }
                string output1 = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                int exitCode = process.ExitCode;

                Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                process.Close();
                n += 1000;
//                w += 1000;
                Console.WriteLine(DateTime.Now.ToString());

            }


            //for ( int count = 0; count < 1; count++)
            //{
            //    sw.WriteLine("cd " + path);
            //    sw.WriteLine("strabo.core.exe" + " " + w.ToString() + " " + n.ToString() + " " +
            //        input + "\\" + count.ToString() + " " + output + "\\" + count.ToString() + " " +
            //        "nlfors-cs-1920" + " " + "4");
            //   // n += 1100;
            //    w += 1100;
            //}
            //sw.Close();

            //Process IntExe = new Process();
            //IntExe.StartInfo.FileName = Path.Combine(path, "CrowdedSamples.bat"); // corner.exe is the old one
            //IntExe.StartInfo.Arguments = "";
            //IntExe.StartInfo.UseShellExecute = false;
            //IntExe.StartInfo.CreateNoWindow = true;
            //IntExe.StartInfo.RedirectStandardOutput = true;

            //IntExe.Start();

            //IntExe.WaitForExit();

        }
    }
}
