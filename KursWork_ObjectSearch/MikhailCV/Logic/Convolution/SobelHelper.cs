using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Convolution
{
    public static class SobelHelper
    {
        public static readonly Mat SobelKernelX = new Mat(3, 3, new double[] { 1, 0, -1, 2, 0, -2, 1, 0, -1 });
        public static readonly Mat SobelKernelY = new Mat(3, 3, new double[] { 1, 2, 1, 0, 0, 0, -1, -2, -1 });

        private static readonly Mat SobelA = new Mat(3, 1, new double[] { -1, 0, 1 });
        private static readonly Mat SobelB = new Mat(3, 1, new double[] { 1, 2, 1 });

        private static readonly Kernel SobelKernelA = Kernel.For(SobelA, SobelB);
        private static readonly Kernel SobelKernelB = Kernel.For(SobelB, SobelA);

        public static Mat Sobel(Mat source, Mat dx, Mat dy, BorderWrapType borderWrapType)
        {
            ConvolutionHelper.Separable(source, dx, SobelKernelA, borderWrapType);
            ConvolutionHelper.Separable(source, dy, SobelKernelB, borderWrapType);

            var target = new Mat(source.Width, source.Height);
            for (var x = 0; x < source.Width; x++)
                for (var y = 0; y < source.Height; y++)
                    target.Set(x, y, MathHelper.SqrtOfSqrSum(dx.GetAt(x, y), dy.GetAt(x, y)));

            return target;
        }

        public static Mat Sobel(Mat source, BorderWrapType borderWrapType)
        {
            Mat dx = new Mat(), dy = new Mat();

            ConvolutionHelper.Separable(source, dx, SobelKernelA, borderWrapType);
            ConvolutionHelper.Separable(source, dy, SobelKernelB, borderWrapType);

            var target = new Mat(source.Width, source.Height);
            for (var x = 0; x < source.Width; x++)
                for (var y = 0; y < source.Height; y++)
                    target.Set(x, y, MathHelper.SqrtOfSqrSum(dx.GetAt(x, y), dy.GetAt(x, y)));

            return target;
        }

        public static Mat SobelNonSeparable(Mat source, Mat dx, Mat dy)
        {
            ConvolutionHelper.NonSeparable(source, dx, SobelKernelX, BorderWrapType.Copy);
            ConvolutionHelper.NonSeparable(source, dy, SobelKernelY, BorderWrapType.Copy);

            var target = new Mat(source.Width, source.Height);
            for (var x = 0; x < source.Width; x++)
                for (var y = 0; y < source.Height; y++)
                    target.Set(x, y, MathHelper.SqrtOfSqrSum(dx.GetAt(x, y), dy.GetAt(x, y)));

            return target;
        }

        public static Mat GetSobelX(Mat source, BorderWrapType borderWrapType)
        {
            Mat dx = source.Clone();
            ConvolutionHelper.NonSeparable(source, dx, SobelKernelX, borderWrapType);
            return dx;
        }

        public static Mat GetSobelY(Mat source, BorderWrapType borderWrapType)
        {
            Mat dy = source.Clone();
            ConvolutionHelper.NonSeparable(source, dy, SobelKernelY, borderWrapType);
            return dy;
        }


        public static Mat getGradient(Mat xImage, Mat yImage)
        {
            if (xImage.Height != yImage.Height || xImage.Width != yImage.Width)
                throw new Exception("Изображения разного размера");

            Mat gradient = new Mat(xImage.Width, xImage.Height);

            for (int x = 0; x < gradient.Width; x++)
                for (int y = 0; y < gradient.Height; y++)
                {
                    gradient.Set(x, y, Math.Sqrt(MathHelper.Sqr(xImage.GetAt(x, y)) + MathHelper.Sqr(yImage.GetAt(x, y))));
                }


            return gradient;
        }

        public static Mat getGradientAngle(Mat xImage, Mat yImage)
        {
            if (xImage.Height != yImage.Height || xImage.Width != yImage.Width)
                throw new Exception("Изображения разного размера");
            Mat gradientAngle = new Mat(xImage.Width, xImage.Height);

            for (int x = 0; x < gradientAngle.Width; x++)
                for (int y = 0; y < gradientAngle.Height; y++)
                {
                    gradientAngle.Set(x, y, Math.Atan2(yImage.GetAt(x, y), xImage.GetAt(x, y)));
                }

            return gradientAngle;
        }
    }
}
