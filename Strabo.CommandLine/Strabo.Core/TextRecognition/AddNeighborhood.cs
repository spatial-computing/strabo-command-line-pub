using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Strabo.Core.Utility;
using Emgu.CV;
using Emgu.CV.CvEnum;





namespace Strabo.Core.TextRecognition
{



    class AddNeighborhood
    {
        public void AddNeighbor(List<TessResult> tessOcrResultList, string inputpath)
        {
            string file_name = "";
            List<PointF> points = new List<PointF>();
            Boolean Continue = true;
            int Counter = 0;
            SVM SVMForClassification=new SVM(@"C:\Users\nhonarva\Documents\MachineLearning\sampleTraintData\");

            for (int k = 0; k < tessOcrResultList.Count; k++)  /// add neighbors of each bounding box
            {
                points = new List<PointF>();
                string TotalPixelToSizeRatio = "";
                file_name = tessOcrResultList[k].fileName;
                Bitmap ResultOfTextDetection;

                string[] split = file_name.Split('_');
                if (split[6].Equals("0"))
                    ResultOfTextDetection = new Bitmap(inputpath + file_name + ".png");
                else
                {
                    string newFileName = "";
                    split[6] = "0";
                    for (int j = 0; j < split.Length; j++)
                    {
                        if (j < split.Length - 1)
                            newFileName += split[j] + "_";
                        else
                            newFileName += split[j];
                    }
                    ResultOfTextDetection = new Bitmap(inputpath + newFileName + ".png");
                }
                Bitmap OriginalBinaryImage = new Bitmap(inputpath + "BinaryOutput.png");


                for (int i = 0; i < ResultOfTextDetection.Width; i++)    //// scan image to find foreground pixels
                {
                    for (int j = 0; j < ResultOfTextDetection.Height; j++)
                    {
                        Color color = ResultOfTextDetection.GetPixel(i, j);
                        if (!ResultOfTextDetection.GetPixel(i, j).Name.Equals("ffffffff"))
                        {
                            PointF point = new PointF(i, j);
                            points.Add(point);
                        }
                    }
                }

                String[] splitTokens = file_name.Split('_');
                int numberOfRecognizedLetters = tessOcrResultList[k].tess_word3.Length - 2; /// -2 because we have \n\n at the end of word

                int x = Convert.ToInt32(tessOcrResultList[k].x);
                int y = Convert.ToInt32(tessOcrResultList[k].y);
                int width = Convert.ToInt32(tessOcrResultList[k].w);
                int height = Convert.ToInt32(tessOcrResultList[k].h);

                if (splitTokens[6].Equals("0"))     /// bounding box with slope 0 (horizental)
                {

                    int estimatedSizeOfOneLetter = width / numberOfRecognizedLetters;
                    double pixelToSizeRatio = (double)(points.Count) / (double)(width * height);
                    TotalPixelToSizeRatio = Convert.ToString(Math.Round((decimal)pixelToSizeRatio, 2));

                    Point left = new Point();   /// left coordinate for box at left hand side of blob
                    int leftPixelCount = 0;
                    double leftPixelToSizeRatio = 0;
                    left.X = x - estimatedSizeOfOneLetter;
                    left.Y = y;

                    Point right = new Point();          //// right coordinate for box at right hand side of blob
                    int rightPixelCount = 0;
                    double rightPixelToSizeRatio = 0;
                    right.X = x + width;
                    right.Y = y;

                    Continue = true;

                    if (char.IsUpper(tessOcrResultList[k].tess_word3.ToCharArray()[0]))
                        Continue = false;
                    if (left.X < 0)
                        Continue = false;
                    //   continue;

                    int AddedAreaWidth=estimatedSizeOfOneLetter;

                    Counter = 0;
                    while (Continue)
                    {
                        
                        if ((left.X - estimatedSizeOfOneLetter) < 0)
                            AddedAreaWidth = left.X;

                        int[,] AddedAreaPoints = new int[AddedAreaWidth, height];

                        for (int i = left.X; i < left.X + AddedAreaWidth; i++)
                        {
                            for (int j = left.Y; j < left.Y + height; j++)
                            {
                                if ((!OriginalBinaryImage.GetPixel(i, j).Name.Equals("ffffffff")))
                                {
                                    leftPixelCount++;
                                    AddedAreaPoints[i-left.X, j-left.Y] = 1;
                                }
                                else
                                    AddedAreaPoints[i-left.X, j-left.Y] = 0;
                            }
                        }


                        Counter++;
                        leftPixelToSizeRatio = (double)(leftPixelCount) / (double)(AddedAreaWidth * height);
                        QGISJsonNeighbothood.AddFeature(left.X, left.Y, left.X + AddedAreaWidth, left.Y, left.X + AddedAreaWidth, left.Y + tessOcrResultList[k].h, left.X, left.Y + tessOcrResultList[k].h, tessOcrResultList[k].id + "-" + Convert.ToString(Counter), Convert.ToString(Math.Round((decimal)leftPixelToSizeRatio, 2)), -1);
                        left.X -= estimatedSizeOfOneLetter;
                        leftPixelCount = 0;
                        if (left.X < 0)
                            Continue = false;
                        if (leftPixelToSizeRatio < 0.2)
                            Continue = false;
                        if (leftPixelToSizeRatio > 0.2)
                            SVMForClassification.Classification(AddedAreaPoints, tessOcrResultList[k].id + "-" + Convert.ToString(Counter));
        
                    }
                    

                    Continue = true;

                    if (right.X > OriginalBinaryImage.Width)
                        Continue = false;
                    
                    AddedAreaWidth = estimatedSizeOfOneLetter;                    
                    Counter = 0;

                    while (Continue)
                    {
                        if ((right.X + estimatedSizeOfOneLetter) > OriginalBinaryImage.Width)
                            AddedAreaWidth = OriginalBinaryImage.Width - right.X;

                        int[,] AddedAreaPoints = new int[AddedAreaWidth, height]; 

                        for (int i = right.X; i < right.X + AddedAreaWidth; i++)
                        {
                            for (int j = right.Y; j < right.Y + height; j++)
                            {

                                if (!OriginalBinaryImage.GetPixel(i, j).Name.Equals("ffffffff"))
                                {
                                    rightPixelCount++;
                                    AddedAreaPoints[i-right.X, j-right.Y] = 1;
                                }
                                else
                                    AddedAreaPoints[i-right.X, j-right.Y] = 0;
                            }
                        }
                        Counter++;
                        rightPixelToSizeRatio = (double)(rightPixelCount) / (double)(AddedAreaWidth * height);
                        QGISJsonNeighbothood.AddFeature(right.X, right.Y, right.X + AddedAreaWidth, right.Y, right.X + AddedAreaWidth, right.Y + tessOcrResultList[k].h, right.X, right.Y + tessOcrResultList[k].h, tessOcrResultList[k].id + "-" + Convert.ToString(Counter), Convert.ToString(Math.Round((decimal)rightPixelToSizeRatio, 2)), -1);
                        right.X += estimatedSizeOfOneLetter;
                        rightPixelCount = 0;
                        if (right.X > OriginalBinaryImage.Width)
                            Continue = false;
                        if (rightPixelToSizeRatio < 0.2)
                            Continue = false;
                        if (rightPixelToSizeRatio > 0.2)
                            SVMForClassification.Classification(AddedAreaPoints, tessOcrResultList[k].id + "-" + Convert.ToString(Counter));
                    }
                    //    }

                    QGISJsonNeighbothood.AddFeature(x, y, x + width, y, x + width, y + height, x, y + height, tessOcrResultList[k].id, TotalPixelToSizeRatio, -1);
                }
                else
                {
                    PointF[] pts;
                    pts = points.ToArray();
                    Emgu.CV.Structure.MCvBox2D box = PointCollection.MinAreaRect(pts);
                    Rectangle boundingbox = PointCollection.BoundingRectangle(pts);

                    Emgu.CV.Structure.MCvBox2D boxinoriginalImage = box;
                    boxinoriginalImage.center.X += x - boundingbox.X;   /// 63=originalboundingbox.x(1069)(I used result of intermediate image)-boundingbox.x(21)
                    boxinoriginalImage.center.Y += y - boundingbox.Y;
                    PointF[] vertices = boxinoriginalImage.GetVertices();
                    double pixelToSizeRatio = (double)(points.Count) / (double)(box.size.Width * box.size.Height);
                    TotalPixelToSizeRatio = Convert.ToString(Math.Round((decimal)pixelToSizeRatio, 2));
                    QGISJsonNeighbothood.AddFeature(vertices[0].X, vertices[0].Y, vertices[1].X, vertices[1].Y, vertices[2].X, vertices[2].Y, vertices[3].X, vertices[3].Y, tessOcrResultList[k].id, TotalPixelToSizeRatio, -1);

                    for (int m = 1; m < 4; m++)
                    {
                        Emgu.CV.Structure.MCvBox2D addedArea = box;

                        if (box.size.Height > box.size.Width)   ///Non horizental bounding boxes that slope of bounding box is less than zero 
                        {
                            double angleInRadius = ((90 + box.angle) * Math.PI) / 180;


                            double addToX = (((box.size.Height) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Cos(angleInRadius);  // I have multiplied height to 3/4 because we have four letter in recognized image and I want to find the center of added part for boundingbox
                            double addToY = (((box.size.Height) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Sin(angleInRadius);


                            addedArea.center.X += (x - boundingbox.X) + (float)addToX;
                            addedArea.center.Y += (y - boundingbox.Y) + (float)addToY;

                            addedArea.size.Height = (float)((box.size.Height) / numberOfRecognizedLetters);
                        }

                        else    ///Non horizental bounding boxes that slope of bounding box is greater than zero 
                        {
                            double angleInRadius = (((-1) * box.angle) * Math.PI) / 180;


                            double addToX = (((box.size.Width) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Cos(angleInRadius);  // I have multiplied height to 3/4 because we have four letter in recognized image and I want to find the center of added part for boundingbox
                            double addToY = (((box.size.Width) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Sin(angleInRadius);


                            addedArea.center.X += (x - boundingbox.X) - (float)addToX;
                            addedArea.center.Y += (y - boundingbox.Y) + (float)addToY;

                            addedArea.size.Width = (float)((box.size.Width) / numberOfRecognizedLetters);
                        }
                        PointF[] AddedAreaVertices = addedArea.GetVertices();

                        PointF ExploreAddedArea_MinPoint = AddedAreaVertices[0];
                        PointF ExploreAddedArea_MaxPoint = AddedAreaVertices[0];

                        for (int i = 1; i < AddedAreaVertices.Length; i++)
                        {
                            if (AddedAreaVertices[i].X < ExploreAddedArea_MinPoint.X)
                                ExploreAddedArea_MinPoint.X = AddedAreaVertices[i].X;

                            if (AddedAreaVertices[i].Y < ExploreAddedArea_MinPoint.Y)
                                ExploreAddedArea_MinPoint.Y = AddedAreaVertices[i].Y;

                            if (AddedAreaVertices[i].X > ExploreAddedArea_MaxPoint.X)
                                ExploreAddedArea_MaxPoint.X = AddedAreaVertices[i].X;

                            if (AddedAreaVertices[i].Y > ExploreAddedArea_MaxPoint.Y)
                                ExploreAddedArea_MaxPoint.Y = AddedAreaVertices[i].Y;
                        }


                        int AddedAreaPixelNumber = 0;

                        double[] slope = new double[4];
                        double[] b = new double[4];
                        for (int i = 0; i < AddedAreaVertices.Length; i++)    //// find four lines that create min area rectangle bounding box
                        {
                            if (i < AddedAreaVertices.Length - 1)
                                slope[i] = (AddedAreaVertices[i].Y - AddedAreaVertices[i + 1].Y) / (AddedAreaVertices[i].X - AddedAreaVertices[i + 1].X);

                            else
                                slope[i] = (AddedAreaVertices[i].Y - AddedAreaVertices[0].Y) / (AddedAreaVertices[i].X - AddedAreaVertices[0].X);

                            b[i] = -(AddedAreaVertices[i].Y - slope[i] * AddedAreaVertices[i].X);
                        }

                        for (int i = Convert.ToInt32(ExploreAddedArea_MinPoint.X); i < Convert.ToInt32(ExploreAddedArea_MaxPoint.X) + 1; i++)
                        {
                            for (int j = Convert.ToInt32(ExploreAddedArea_MinPoint.Y); j < Convert.ToInt32(ExploreAddedArea_MaxPoint.Y) + 1; j++)
                            {
                                if ((i < OriginalBinaryImage.Width) && (j < OriginalBinaryImage.Width) && (i > 0) && (j > 0))
                                {
                                    Color color = OriginalBinaryImage.GetPixel(i, j);
                                    double[] result = new double[4];
                                    for (int l = 0; l < result.Length; l++)
                                        result[l] = j - slope[l] * i + b[l];

                                    if (result[0] < 0 && result[1] > 0 && result[2] > 0 && result[3] < 0)   /// explore if pixel is in bounding box of Min Area Rectangle
                                    {
                                        //  sourceImage1.SetPixel(i, j, Color.Green);
                                        if (!OriginalBinaryImage.GetPixel(i, j).Name.Equals("ffffffff"))
                                        {
                                            AddedAreaPixelNumber++;
                                        }

                                    }
                                }
                                else
                                    break;
                            }
                        }

                        double addedAreaSize = addedArea.size.Height * addedArea.size.Width;
                        double AddedAreaPixelToSizeRatio = (double)(AddedAreaPixelNumber) / (addedArea.size.Height * addedArea.size.Width);


                        QGISJsonNeighbothood.AddFeature(AddedAreaVertices[0].X, AddedAreaVertices[0].Y, AddedAreaVertices[1].X, AddedAreaVertices[1].Y, AddedAreaVertices[2].X, AddedAreaVertices[2].Y, AddedAreaVertices[3].X, AddedAreaVertices[3].Y, tessOcrResultList[k].id, Convert.ToString(Math.Round((decimal)AddedAreaPixelToSizeRatio, 2)), -1);
                    }


                    for (int m = 1; m < 4; m++)
                    {
                        Emgu.CV.Structure.MCvBox2D addedArea = box;

                        if (box.size.Height > box.size.Width)   ///Non horizental bounding boxes that slope of bounding box is less than zero 
                        {
                            double angleInRadius = ((90 + box.angle) * Math.PI) / 180;


                            double addToX = (((box.size.Height) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Cos(angleInRadius);  // I have multiplied height to 3/4 because we have four letter in recognized image and I want to find the center of added part for boundingbox
                            double addToY = (((box.size.Height) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Sin(angleInRadius);


                            addedArea.center.X += (x - boundingbox.X) - (float)addToX;
                            addedArea.center.Y += (y - boundingbox.Y) - (float)addToY;

                            addedArea.size.Height = (float)((box.size.Height) / numberOfRecognizedLetters);
                        }

                        else    ///Non horizental bounding boxes that slope of bounding box is greater than zero 
                        {
                            double angleInRadius = (((-1) * box.angle) * Math.PI) / 180;


                            double addToX = (((box.size.Width) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Cos(angleInRadius);  // I have multiplied height to 3/4 because we have four letter in recognized image and I want to find the center of added part for boundingbox
                            double addToY = (((box.size.Width) * ((m - 1) * 2 + numberOfRecognizedLetters + 1)) / (numberOfRecognizedLetters * 2)) * Math.Sin(angleInRadius);


                            addedArea.center.X += (x - boundingbox.X) + (float)addToX;
                            addedArea.center.Y += (y - boundingbox.Y) - (float)addToY;

                            addedArea.size.Width = (float)((box.size.Width) / numberOfRecognizedLetters);
                        }
                        PointF[] AddedAreaVertices = addedArea.GetVertices();

                        PointF ExploreAddedArea_MinPoint = AddedAreaVertices[0];
                        PointF ExploreAddedArea_MaxPoint = AddedAreaVertices[0];

                        for (int i = 1; i < AddedAreaVertices.Length; i++)
                        {
                            if (AddedAreaVertices[i].X < ExploreAddedArea_MinPoint.X)
                                ExploreAddedArea_MinPoint.X = AddedAreaVertices[i].X;

                            if (AddedAreaVertices[i].Y < ExploreAddedArea_MinPoint.Y)
                                ExploreAddedArea_MinPoint.Y = AddedAreaVertices[i].Y;

                            if (AddedAreaVertices[i].X > ExploreAddedArea_MaxPoint.X)
                                ExploreAddedArea_MaxPoint.X = AddedAreaVertices[i].X;

                            if (AddedAreaVertices[i].Y > ExploreAddedArea_MaxPoint.Y)
                                ExploreAddedArea_MaxPoint.Y = AddedAreaVertices[i].Y;
                        }


                        int AddedAreaPixelNumber = 0;

                        double[] slope = new double[4];
                        double[] b = new double[4];
                        for (int i = 0; i < AddedAreaVertices.Length; i++)    //// find four lines that create min area rectangle bounding box
                        {
                            if (i < AddedAreaVertices.Length - 1)
                                slope[i] = (AddedAreaVertices[i].Y - AddedAreaVertices[i + 1].Y) / (AddedAreaVertices[i].X - AddedAreaVertices[i + 1].X);

                            else
                                slope[i] = (AddedAreaVertices[i].Y - AddedAreaVertices[0].Y) / (AddedAreaVertices[i].X - AddedAreaVertices[0].X);

                            b[i] = -(AddedAreaVertices[i].Y - slope[i] * AddedAreaVertices[i].X);
                        }

                        for (int i = Convert.ToInt32(ExploreAddedArea_MinPoint.X); i < Convert.ToInt32(ExploreAddedArea_MaxPoint.X) + 1; i++)
                        {
                            for (int j = Convert.ToInt32(ExploreAddedArea_MinPoint.Y); j < Convert.ToInt32(ExploreAddedArea_MaxPoint.Y) + 1; j++)
                            {
                                if ((i < OriginalBinaryImage.Width) && (j < OriginalBinaryImage.Width) && (i > 0) && (j > 0))
                                {
                                    Color color = OriginalBinaryImage.GetPixel(i, j);
                                    double[] result = new double[4];
                                    for (int l = 0; l < result.Length; l++)
                                        result[l] = j - slope[l] * i + b[l];

                                    if (result[0] < 0 && result[1] > 0 && result[2] > 0 && result[3] < 0)   /// explore if pixel is in bounding box of Min Area Rectangle
                                    {
                                        //  sourceImage1.SetPixel(i, j, Color.Green);
                                        if (!OriginalBinaryImage.GetPixel(i, j).Name.Equals("ffffffff"))
                                        {
                                            AddedAreaPixelNumber++;
                                        }

                                    }
                                }
                                else
                                    break;
                            }
                        }

                        double addedAreaSize = addedArea.size.Height * addedArea.size.Width;
                        double AddedAreaPixelToSizeRatio = (double)(AddedAreaPixelNumber) / (addedArea.size.Height * addedArea.size.Width);


                        QGISJsonNeighbothood.AddFeature(AddedAreaVertices[0].X, AddedAreaVertices[0].Y, AddedAreaVertices[1].X, AddedAreaVertices[1].Y, AddedAreaVertices[2].X, AddedAreaVertices[2].Y, AddedAreaVertices[3].X, AddedAreaVertices[3].Y, tessOcrResultList[k].id, Convert.ToString(Math.Round((decimal)AddedAreaPixelToSizeRatio, 2)), -1);
                    }
                }
            }
            QGISJsonNeighbothood.WriteGeojsonFileByPixels();

        }
    }
}
