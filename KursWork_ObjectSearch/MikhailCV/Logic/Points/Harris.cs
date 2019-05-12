using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Convolution;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Points
{
    class Harris
    {
        private static Mat _image;

        public static List<InterestingPoint> DoHarris(double minValue, int windowSize, Mat image)
        {
            int radius = (int) (windowSize / 2);

            _image = image.Clone();

            // Полчаем матрицу откликов оператоа Харриса
            var harrisMat = Find(image, radius, minValue, BorderWrapType.Copy);


            var points = new List<InterestingPoint>();

            for (var x = 0; x < image.Width; x++)
            for (var y = 0; y < image.Height; y++)
            {
                if (Math.Abs(harrisMat.GetAt(x, y)) < 1e-2)
                    continue;

                points.Add(new InterestingPoint(x, y, harrisMat.GetAt(x, y)));
            }

            return points;
        }
        public static Mat Find(Mat image, int radius, double threshold, BorderWrapType borderWrap)
        {
            var gauss = Gauss.GetFullKernel(radius / 3D);
            var gaussK = gauss.Width / 2;

            var dx = new Mat(image.Width, image.Height);
            var dy = new Mat(image.Width, image.Height);

            SobelHelper.Sobel(image, dx, dy, BorderWrapType.Mirror);

            var lambdas = new Mat(image.Width, image.Height);
            for (var x = 0; x < image.Width; x++)
                for (var y = 0; y < image.Height; y++)
                {
                    double a = 0, b = 0, c = 0;

                    for (var u = -radius; u <= radius; u++)
                        for (var v = -radius; v <= radius; v++)
                        {
                            var multiplier = gauss.GetAt(u + gaussK, v + gaussK);

                            a += multiplier * MathHelper.Sqr(dx.GetPixel(x + u, y + v, borderWrap));
                            b += multiplier * dx.GetPixel(x + u, y + v, borderWrap) *
                                 dy.GetPixel(x + u, y + v, borderWrap);
                            c += multiplier * MathHelper.Sqr(dy.GetPixel(x + u, y + v, borderWrap));
                        }

                    lambdas.Set(x, y, LambdaMin(a, b, c));
                }

            return CornerDetectionHelper.FindPoints(lambdas, threshold);
        }

        public static double FindAt(Mat image, int radius, int x, int y)
        {
            var gauss = Gauss.GetFullKernel(radius / 3.0);

            double a = 0, b = 0, c = 0;
            for (var u = -radius; u <= radius; u++)
                for (var v = -radius; v <= radius; v++)
                {
                    var ix = ConvolutionHelper.ConvolveCell(image, SobelHelper.SobelKernelX, x + u, y + v);
                    var iy = ConvolutionHelper.ConvolveCell(image, SobelHelper.SobelKernelY, x + u, y + v);
                    var gaussPoint = gauss.GetPixel(u + radius, v + radius, BorderWrapType.Copy);
                    a += gaussPoint * ix * ix;
                    b += gaussPoint * ix * iy;
                    c += gaussPoint * iy * iy;
                }

            return LambdaMin(a, b, c);
        }

        private static double LambdaMin(double varA, double varB, double varC)
        {
            double a = 1;
            var b = -(varA + varC);
            var c = varA * varC - varB * varB;
            var d = b * b - 4 * a * c;

            if (d < 0 && d > -1e-6) d = 0;

            if (d < 0) return 0;

            var a1 = (-b + Math.Sqrt(d)) / (2 * a);
            var a2 = (-b - Math.Sqrt(d)) / (2 * a);

            return Math.Min(a1, a2);
        }

    }
}
