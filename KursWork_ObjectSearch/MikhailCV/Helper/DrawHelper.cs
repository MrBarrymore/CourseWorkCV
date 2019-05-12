using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Logic;
using KursWork_ObjectSearch.MikhailCV.Logic.Descriptors;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;
using KursWork_ObjectSearch.MikhailCV.Logic.Points;

namespace KursWork_ObjectSearch.MikhailCV.Helper
{
    public static class DrawHelper
    {
        private const int DefaultRadius = 10;
        private static readonly Random Random = new Random();

        public static void DrawPointsToFile(Mat image, Mat corners, string fileName)
        {
            var points = new List<InterestingPoint>();

            for (var x = 0; x < corners.Width; x++)
                for (var y = 0; y < corners.Height; y++)
                {
                    if (Math.Abs(corners.GetAt(x, y)) < 1e-6)
                        continue;

                    points.Add(new InterestingPoint(x, y, corners.GetAt(x, y)));
                }

            DrawPointsToFile(image, points, fileName);
        }

        public static void DrawPointsToFile(Mat image, List<InterestingPoint> points, string fileName)
        {
            DrawPoints(image, points).Save(fileName + ".jpeg");
        }

        public static Bitmap DrawPoints(Mat image, List<Descriptor> descriptors)
        {
            return DrawPoints(image, descriptors.Select(x => x.Point).ToList());
        }

        public static Bitmap DrawPoints(Mat image, List<InterestingPoint> points)
        {
            var img = IOHelper.MatToImage(image);
            var g = Graphics.FromImage(img);
            var pen = new Pen(Color.OrangeRed, 2);
            var penAqua = new Pen(Color.Aqua);

            foreach (var point in points)
            {
                g.DrawEllipse(pen,
                    (int)(point.getX() - point.Radius),
                    (int)(point.getY() - point.Radius),
                    (int)(2 * point.Radius),
                    (int)(2 * point.Radius)
                );

                foreach (var angle in point.Angles)
                {
                    var dx = (int)(Math.Cos(angle) * 1.5 * point.Radius);
                    var dy = (int)(Math.Sin(angle) * -1.5 * point.Radius);

                    //  g.DrawLine(penAqua, point.getX(), point.getY(), point.getX() + dx, point.getY() + dy);
                }
            }

            return img;
        }

        public static Bitmap DrawTwoImages(Bitmap img1, Bitmap img2, List<ValueTuple<Descriptor, Descriptor>> match)
        {
            var offset = 20;
            var width = img1.Width + img2.Width + offset;
            var height = Math.Max(img1.Height, img2.Height) + offset;

            var result = new Bitmap(width + 2 * offset, height + 2 * offset);

            using (var g = Graphics.FromImage(result))
            {
                g.DrawImage(img1,
                    offset,
                    offset,
                    img1.Width,
                    img1.Height);

                g.DrawImage(img2,
                    2 * offset + img1.Width,
                    2 * offset,
                    img2.Width,
                    img2.Height);
            }

            foreach (var (d1, d2) in match)
            {
                InterestingPoint
                    from = d1.Point,
                    to = d2.Point;

                DrawLine(result,
                    offset + from.getX(),
                    offset + from.getY(),
                    (int)from.Radius,
                    2 * offset + to.getX() + img1.Width,
                    2 * offset + to.getY(),
                    (int)to.Radius
                );
            }

            return result;
        }

        private static void DrawLine(Bitmap source, int x1, int y1, int r1, int x2, int y2, int r2)
        {
            using (var g = Graphics.FromImage(source))
            {
                var pen = new Pen(Color.FromArgb(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255)), 2);

                g.DrawLine(pen, x1, y1, x2, y2);

                g.DrawEllipse(pen,
                    x1 - r1,
                    y1 - r1,
                    2 * r1,
                    2 * r1);

                g.DrawEllipse(pen,
                    x2 - r2,
                    y2 - r2,
                    2 * r2,
                    2 * r2);
            }
        }

        public static void DrawLine(Image img, int x1, int y1, int x2, int y2)
        {
            using (var g = Graphics.FromImage(img))
            {
                var pen = new Pen(Color.Aqua, 2);

                g.DrawLine(pen, x1, y1, x2, y2);

                g.DrawEllipse(pen,
                    x1 - DefaultRadius,
                    y1 - DefaultRadius,
                    2 * DefaultRadius,
                    2 * DefaultRadius);

                g.DrawEllipse(pen,
                    x2 - DefaultRadius,
                    y2 - DefaultRadius,
                    2 * DefaultRadius,
                    2 * DefaultRadius);
            }
        }

        public static void DrawPolygon(Image image, Point[] points, bool corner = false)
        {
            using (var g = Graphics.FromImage(image))
            {
                var pen = new Pen(Color.Chartreuse, 5);
                g.DrawPolygon(pen, points);

                if (corner)
                    g.DrawEllipse(pen, points[0].X - 8, points[0].Y - 8, 16, 16);
            }
        }

        public static void DrawPolygon(Image image, Point[] points, int shift, int offset, bool corner = false)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X = points[i].X + shift + 2*offset;
                points[i].Y = points[i].Y + 2*offset;
            }
           
            using (var g = Graphics.FromImage(image))
            {
                
                var pen = new Pen(Color.Chartreuse, 5);
                g.DrawPolygon(pen, points);

                if (corner)
                    g.DrawEllipse(pen, points[0].X - 8, points[0].Y - 8, 16, 16);
            }
        }


        public static Bitmap BuildPicture(List<InterestingPoint> PointsA, List<InterestingPoint> PointsB, Mat inputImageA, Mat inputImageB)
        {
            Bitmap picture;

            picture = DrawHelper.CombineImages(inputImageA, inputImageB);

            Graphics graph = Graphics.FromImage(picture);
            Color pen = Color.Blue;
            Pen penLine = new Pen(Color.Blue);

            foreach (var pointPair in PointsA)
            {
                graph.FillEllipse(new SolidBrush(pen), pointPair.getX(), pointPair.getY(), 3, 3);

            }

            foreach (var pointB in PointsB)
            {
                graph.FillEllipse(new SolidBrush(pen), (inputImageA.Width + 20) + pointB.getX(), pointB.getY(), 3, 3);
            }

            return picture;
        }

        public static Bitmap CombineImages(Mat ImageA, Mat ImageB)
        {
            int newHeight = Math.Max(ImageA.Height, ImageB.Height);
            int newWidth = ImageA.Width + ImageB.Width + 20;

            Mat combinePicture = new Mat(newWidth, newHeight);

            for (int x = 0; x < ImageA.Width; x++)
                for (int y = 0; y < ImageA.Height; y++)
                {
                    combinePicture.Set(x, y, ImageA.GetAt(x, y));
                }

            for (int x = ImageA.Width; x < ImageA.Width + 20; x++)
                for (int y = 0; y < ImageA.Height; y++)
                {
                    combinePicture.Set(x, y, 1);
                }

            for (int x = 0; x < ImageB.Width; x++)
                for (int y = 0; y < ImageB.Height; y++)
                {
                    combinePicture.Set(x + ImageA.Width + 20, y, ImageB.GetAt(x, y));
                }


            Bitmap picture = Transformer.FromMatToBitmap(combinePicture);

            return picture;
        }

        public static Bitmap DrawPoints(List<InterestingPoint> point, Mat inputImage, int picture)
        {
            Bitmap image;
            image = Transformer.FromMatToBitmap(inputImage);
            Graphics graph = Graphics.FromImage(image);
            Color pen = Color.Blue;

            foreach (var interestingPoint in point)
            {
                graph.FillEllipse(new SolidBrush(pen), interestingPoint.getX(), interestingPoint.getY(), 3, 3);
            }

            return image;
        }

    }
}
