using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strabo.Core.Worker;
namespace Strabo.Test
{
    class TestSymbolRecognition
    {
        string path = @"C:\Users\Sima\Documents\strabo-command-line\SymbolRecognition Test Results\Hotel";
        private SymbolRecognitionWorker _symbolRecognitionWorker;
        public TestSymbolRecognition()
        {
            //Old Version
            // SymbolRecognitionWorker _symbolRecognitionWorker = new SymbolRecognitionWorker();
            //_symbolRecognitionWorker.Apply(path, 1,5,500);
            //_symbolRecognitionWorker.Apply(path, 0.78, 13, 700);



            //_symbolRecognitionWorker = new SymbolRecognitionWorker(path, SymbolRecognitionWorker.ScanningMethod.ScanningByImageDimention, SymbolRecognitionWorker.DescriptorMethod.SURF, SymbolRecognitionWorker.MachineLearningMethod.KNN);
            _symbolRecognitionWorker = new SymbolRecognitionWorker();
            _symbolRecognitionWorker.SymbolRecognitionBySVM(@"C:\Users\simakmo\Documents\Sima\Symbol Reconition\data\in\Positive", @"C:\Users\simakmo\Documents\Sima\Symbol Reconition\data\in\Negative",
                  @"C:\Users\simakmo\Documents\Sima\Symbol Reconition\data\");
        }

    }
}
