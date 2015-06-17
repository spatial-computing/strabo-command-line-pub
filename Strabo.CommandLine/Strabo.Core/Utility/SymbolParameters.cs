using System;

namespace Strabo.Core.Utility
{
   public  class SymbolParameters
    {
        private double _uniquenessThresh;
        private int _tm;
        private int _hessianThress;
        private double _histogramMatchingScore;
        public double UniquenessThress { get {return _uniquenessThresh;} set{_uniquenessThresh=value;} }
        public int TM { get { return _tm; } set { _tm= value;} }
        public int HessianThresh { get { return _hessianThress; } set { _hessianThress= value;} }
        public double HistogramMatchingScore { get { return _histogramMatchingScore; }  }
        public SymbolParameters()
        {
           
            _tm =Int32.Parse((ReadConfigFile.ReadModelConfiguration("TM") != "") ? ReadConfigFile.ReadModelConfiguration("TM") : "");
            _uniquenessThresh =Double.Parse( (ReadConfigFile.ReadModelConfiguration("UniguenessThresh") != "") ? ReadConfigFile.ReadModelConfiguration("UniguenessThresh") : "");
            _hessianThress =Int32.Parse( (ReadConfigFile.ReadModelConfiguration("HessianThresh") != "") ? ReadConfigFile.ReadModelConfiguration("HessianThresh") : "");
            _histogramMatchingScore = Double.Parse((ReadConfigFile.ReadModelConfiguration("HistogramMatchingScore") != "") ? ReadConfigFile.ReadModelConfiguration("HistogramMatchingScore") : "");
        }
    }
}
