using Strabo.Core.Utility;
using Strabo.Core.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strabo.Test
{
    public class TestGetMapFromServiceWorker
    {
        public TestGetMapFromServiceWorker() { }
        public void Apply()
        {
            string output_dir = @"C:\Users\yaoyichi\Desktop\";
            Log.SetLogDir(output_dir);
            Log.SetOutputDir(output_dir);
            GetMapFromServiceWorker gmfsWoker = new GetMapFromServiceWorker();
            gmfsWoker.Apply("12957312", "4852401", "Tianditu_cva", output_dir, "map.png");
        }
    }
}
