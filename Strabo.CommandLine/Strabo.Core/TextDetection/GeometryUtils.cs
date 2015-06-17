/*******************************************************************************
 * Copyright 2010 University of Southern California
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * 	http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * This code was developed as part of the Strabo map processing project 
 * by the Spatial Sciences Institute and by the Information Integration Group 
 * at the Information Sciences Institute of the University of Southern 
 * California. For more information, publications, and related projects, 
 * please see: http://spatial-computing.github.io/
 ******************************************************************************/

using System;

namespace Strabo.Core.Worker
{
    /// <summary>
    /// Narges
    /// </summary>
    public class GeometryUtils
    {
        public static double DiffSlope(double s1, double s2)
        {
            double ss1 = s1 - 180;
            if (ss1 < 0) ss1 = s1;
            double ss2 = s2 - 180;
            if (ss2 < 0) ss2 = s2;

            double diff1 = Math.Abs(ss1 - ss2);
            double diff2 = 180 - diff1;
            if (diff1 < diff2) return diff1;
            else return diff2;
        }
        public static double DistancePoint2LineSegment(double px, double py, double x, double y, double xx, double yy)
        {
            double LineMag;
            double U;
            double X, Y;
            LineMag = DistancePoint2Point(x, y, xx, yy);
            U = ((px - x) * (xx - x) + (py - y) * (yy - y)) / (LineMag * LineMag);
            if (U < 0.0 || U > 1.0) // closest point does not fall within the line segment
            {
                double d1 = DistancePoint2Point(px, py, x, y);
                double d2 = DistancePoint2Point(px, py, xx, yy);
                if (d1 < d2) return d1;
                else return d2;
            }
            X = x + U * (xx - x);
            Y = y + U * (yy - y);
            return DistancePoint2Point(px, py, X, Y);
        }
        public static double DistancePoint2Line(double px, double py, double x, double y, double xx, double yy)
        {
            double LineMag;
            double U;
            double X, Y;
            LineMag = DistancePoint2Point(x, y, xx, yy);
            U = ((px - x) * (xx - x) + (py - y) * (yy - y)) / (LineMag * LineMag);
            X = x + U * (xx - x);
            Y = y + U * (yy - y);
            return DistancePoint2Point(px, py, X, Y);
        }
        public static double DistancePoint2Point(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
        //public static double DistanceRoadPoint2RoadPoint(RoadPoint rp1, RoadPoint rp2)
        //{
        //    double x1 = rp1.x;
        //    double y1 = rp1.y;
        //    double x2 = rp2.x;
        //    double y2 = rp2.y;
        //    return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        //}
        public static double[] NearestPointLine(double px, double py, double x, double y, double xx, double yy)
        {
            double LineMag;
            double U;
            LineMag = DistancePoint2Point(x, y, xx, yy);
            U = ((px - x) * (xx - x) + (py - y) * (yy - y)) / (LineMag * LineMag);
            double[] XY = new double[2];
            if (U < 0.0 || U > 1.0) // closest point does not fall within the line segment
            {
                double d1 = DistancePoint2Point(px, py, x, y);
                double d2 = DistancePoint2Point(px, py, xx, yy);
                if (d1 < d2)
                {
                    XY[0] = x; XY[1] = y;
                }
                else
                {
                    XY[0] = xx; XY[1] = yy;
                }
                return XY;
            }

            XY[0] = x + U * (xx - x);
            XY[1] = y + U * (yy - y);
            return XY;
        }
        public static int IsPointInBoundingBox(double x1, double y1, double x2, double y2, double px, double py)
        {
            double left, top, right, bottom; // Bounding Box For Line Segment
            // For Bounding Box
            if (x1 < x2)
            {
                left = x1;
                right = x2;
            }
            else
            {
                left = x2;
                right = x1;
            }

            if (y1 > y2)
            {
                top = y1;
                bottom = y2;
            }
            else
            {
                top = y2;
                bottom = y1;
            }
            if (px >= left && px <= right && py >= bottom && py <= top)
                return 1;
            else
                return 0;
        }
        public static int LineSegmentIntersection(double l1x1, double l1y1, double l1x2, double l1y2, double l2x1, double l2y1, double l2x2, double l2y2)
        {
            double m1, c1, m2, c2, intersection_X, intersection_Y;
            double dx, dy;
            dx = l1x2 - l1x1;
            dy = l1y2 - l1y1;
            m1 = dy / dx;
            // y = mx + c

            // intercept c = y - mx
            c1 = l1y1 - m1 * l1x1; // which is same as y2 - slope * x2
            dx = l2x2 - l2x1;
            dy = l2y2 - l2y1;
            m2 = dy / dx;
            // y = mx + c

            // intercept c = y - mx
            c2 = l2y1 - m2 * l2x1; // which is same as y2 - slope * x2
            if ((m1 - m2) == 0)
                return 0;
            else
            {
                intersection_X = (c2 - c1) / (m1 - m2);
                intersection_Y = m1 * intersection_X + c1;
            }
            //if (IsPointInBoundingBox(l1x1, l1y1, l1x2, l1y2, intersection_X, intersection_Y) == 1 && IsPointInBoundingBox(l2x1, l2y1, l2x2, l2y2, intersection_X, intersection_Y) == 1)
            if (IsPointInBoundingBox(l1x1, l1y1, l1x2, l1y2, intersection_X, intersection_Y) == 1)
                return 1;
            else
                return 0;
        }
        public static int SidePointOfLine(double ax, double ay, double bx, double by, double cx, double cy)
        {
            double c = ((bx - ax) * (cy - ay) - (by - ay) * (cx - ax));
            if (c > 0)
                return 1;
            else
                return -1;
        }
    }
}
