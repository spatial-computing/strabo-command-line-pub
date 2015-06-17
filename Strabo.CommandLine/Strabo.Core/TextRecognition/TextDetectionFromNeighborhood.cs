using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Strabo.Core.ImageProcessing;

namespace Strabo.Core.TextRecognition
{
    class TextDetectionFromNeighborhood
    {
        public void NeighborhoodTextDetection(Bitmap Neighborhood)
        {
            MyConnectedComponentsAnalysisFast.MyBlobCounter ConnectedComponent = new MyConnectedComponentsAnalysisFast.MyBlobCounter();
            ConnectedComponent.GetBlobs(Neighborhood);
        }
    }
}
