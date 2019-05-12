using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace KursWork_ObjectSearch.EmguCV
{
    public static class DrawMatches
    {
        public static Mat Draw(Mat modelImage, Mat modelImage2,  Mat observedImage, out long matchTime)
        {
            Mat homography, homography2;
            VectorOfKeyPoint modelKeyPoints, modelKeyPoints2;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            using (VectorOfVectorOfDMatch matches2 = new VectorOfVectorOfDMatch())
            {
                Mat mask, mask2;
                FindMatch(modelImage, modelImage2, observedImage, 
                   out matchTime, out modelKeyPoints, out observedKeyPoints, matches, out mask, out homography,
                   out modelKeyPoints2, matches2, out mask2, out homography2);

                //Draw the matched keypoints
                Mat result = new Mat();
                Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
                   matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

                Features2DToolbox.DrawMatches(modelImage2, modelKeyPoints2, observedImage, observedKeyPoints,
                matches2, result, new MCvScalar(255, 0, 255), new MCvScalar(255, 255, 255), mask2);


                #region draw the projected region on the image

                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[]
                    {
                          new PointF(rect.Left, rect.Bottom),
                          new PointF(rect.Right, rect.Bottom),
                          new PointF(rect.Right, rect.Top),
                          new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);

#if NETFX_CORE
                    Point[] points = Extensions.ConvertAll<PointF, Point>(pts, Point.Round);
#else
                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
#endif
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                    }
                }
                #endregion

                #region draw the projected region on the image

                if (homography2 != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage2.Size);
                    PointF[] pts = new PointF[]
                    {
                        new PointF(rect.Left, rect.Bottom),
                        new PointF(rect.Right, rect.Bottom),
                        new PointF(rect.Right, rect.Top),
                        new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography2);

#if NETFX_CORE
                    Point[] points = Extensions.ConvertAll<PointF, Point>(pts, Point.Round);
#else
                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
#endif
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 120, 255), 5);
                    }
                }
                #endregion


                return result;

            }
        }

        public static void FindMatch(Mat modelImage, Mat modelImage2, Mat observedImage,
            out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, 
            out VectorOfKeyPoint modelKeyPoints2, VectorOfVectorOfDMatch matches2, out Mat mask2, out Mat homography2)
        {
            int k = 2;
            double uniquenessThreshold = 0.80;

            Stopwatch watch;
            homography = null;
            homography2 = null;

            modelKeyPoints = new VectorOfKeyPoint();
            modelKeyPoints2 = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

            using (UMat uModelImage = modelImage.GetUMat(AccessType.Read)) // Создаем объект модели изображения 
            using (UMat uModelImage2 = modelImage2.GetUMat(AccessType.Read))
            using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
            {
                KAZE featureDetector = new KAZE();

                //извлекаем точки интереса из изображения объекта
                Mat modelDescriptors = new Mat();
                featureDetector.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                Mat modelDescriptors2 = new Mat();
                featureDetector.DetectAndCompute(uModelImage2, null, modelKeyPoints2, modelDescriptors2, false);

                watch = Stopwatch.StartNew();

                // извлекаем точки интереса из исследуемого изображения
                Mat observedDescriptors = new Mat();
                featureDetector.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);


                // Bruteforce, slower but more accurate
                // You can use KDTree for faster matching with slight loss in accuracy
                using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams())
                using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
                using (DescriptorMatcher matcher = new FlannBasedMatcher(ip, sp))
                {
                    matcher.Add(modelDescriptors);

                    matcher.KnnMatch(observedDescriptors, matches, k, null);
                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= 4)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                            matches, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                                observedKeyPoints, matches, mask, 2);
                    }
                }

                using (Emgu.CV.Flann.LinearIndexParams ip = new Emgu.CV.Flann.LinearIndexParams())
                using (Emgu.CV.Flann.SearchParams sp = new SearchParams())
                using (DescriptorMatcher matcher2 = new FlannBasedMatcher(ip, sp))
                {
                    matcher2.Add(modelDescriptors2);

                    matcher2.KnnMatch(observedDescriptors, matches2, k, null);
                    mask2 = new Mat(matches2.Size, 1, DepthType.Cv8U, 1);
                    mask2.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches2, uniquenessThreshold, mask2);

                    int nonZeroCount = CvInvoke.CountNonZero(mask2);
                    if (nonZeroCount >= 4)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints2, observedKeyPoints,
                            matches2, mask2, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography2 = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints2,
                                observedKeyPoints, matches2, mask2, 2);
                    }
                }


                watch.Stop();

            }
            matchTime = watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Draw the model image and observed image, the matched features and homography projection.
        /// </summary>
        /// <param name="modelImage">The model image</param>
        /// <param name="observedImage">The observed image</param>
        /// <param name="matchTime">The output total time for computing the homography matrix.</param>
        /// <returns>The model image and observed image, the matched features and homography projection.</returns>
        /// 
      
    }
}
