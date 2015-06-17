using Strabo.Core.Worker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace Strabo.Core.TextDetection
{
    public class DetectTextOrientation
    {
        private MergeTextStrings _mts = null;
        private int _tnum;
        public DetectTextOrientation() { }

        public void Apply(MergeTextStrings mts, int tnum)
        {
            _tnum = tnum;
            _mts = mts;
            Thread[] thread_array = new Thread[_tnum];
            for (int i = 0; i < tnum; i++)
            {
                thread_array[i] = new Thread(new ParameterizedThreadStart(DetectOrientationThread));
                thread_array[i].Start(i);
            }
            for (int i = 0; i < tnum; i++)
                thread_array[i].Join();
        }
        public void DetectOrientationThread(object s)
        {
            int counter = 0;
            int start = (int)s;
            for (int i = start; i < _mts.text_string_list.Count; i += _tnum)
            {
                if (_mts.text_string_list[i].char_list.Count > 2)
                {
                    double avg_size = 0;
                    for (int j = 0; j < _mts.text_string_list[i].char_list.Count; j++)
                    {
                        avg_size += _mts.text_string_list[i].char_list[j].bbx.Width + _mts.text_string_list[i].char_list[j].bbx.Height;
                    }
                    counter++;
                    avg_size /= (double)(_mts.text_string_list[i].char_list.Count * 2);
                    MultiThreadsSkewnessDetection mtsd = new MultiThreadsSkewnessDetection();
                    int[] idx = mtsd.Apply(1, _mts.text_string_list[i].srcimg, (int)avg_size, 0, 180, 3);

                    if (idx[0] <= 90)
                    {
                        _mts.text_string_list[i].orientation_list.Add(idx[0]);
                        _mts.text_string_list[i].rotated_img_list.Add((Bitmap)mtsd.rotatedimg_table[idx[0]]);
                        if (GeometryUtils.DiffSlope(idx[0], 90) < 5)
                        {
                            _mts.text_string_list[i].orientation_list.Add(idx[1]);
                            _mts.text_string_list[i].rotated_img_list.Add((Bitmap)mtsd.rotatedimg_table[idx[1]]);
                        }
                    }
                    else
                    {
                        _mts.text_string_list[i].orientation_list.Add(idx[1]);
                        _mts.text_string_list[i].rotated_img_list.Add((Bitmap)mtsd.rotatedimg_table[idx[1]]);
                        if (GeometryUtils.DiffSlope(idx[1], 270) < 5)
                        {
                            _mts.text_string_list[i].orientation_list.Add(idx[0]);
                            _mts.text_string_list[i].rotated_img_list.Add((Bitmap)mtsd.rotatedimg_table[idx[0]]);
                        }
                    }
                }
            }
        }
        public List<int> findNearestSrings(int num, List<TextString> text_string_list)
        {
            Rectangle rec = text_string_list[num].bbx;
            int x1 = rec.X;
            int y1 = rec.Y;
            Rectangle bbx = new Rectangle(x1 - rec.Width * 2, y1 - rec.Height * 2, rec.Width * 4, rec.Height * 4);
            List<int> nearest_string_list = new List<int>();
            string line = "";
            for (int i = 0; i < text_string_list.Count; i++)
            {
                if (text_string_list[i].char_list.Count <= 3) continue;
                if (bbx.IntersectsWith(text_string_list[i].bbx))
                {
                    nearest_string_list.Add(i);
                    line += (i + 1) + "@" + text_string_list[i].rotated_img_list.Count + "@";
                }
            }
            return nearest_string_list;
        }
    }
}
