using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Convolution
{
    class ConvolutionHelper
    {
        public static Mat Convolution(Mat source, Kernel kernel, BorderWrapType borderWrap = BorderWrapType.Copy)
        {
            var target = new Mat(source.Width, source.Height);
            Separable(source, target, kernel, borderWrap);
            return target;
        }

        public static void Separable(Mat source, Mat target, Kernel kernel, BorderWrapType borderWrap)
        {
            int kx = kernel.GetXVector().Width / 2,
                ky = kernel.GetYVector().Width / 2;

            var tmp = source.Clone();
            for (var x = 0; x < source.Width; x++)
                for (var y = 0; y < source.Height; y++)
                {
                    double value = 0;

                    for (var dx = -kx; dx <= kx; dx++)
                        value += source.GetPixel(x - dx, y, borderWrap) * kernel.GetXVector().GetAt(dx + kx, 0);

                    tmp.Set(x, y, value);
                }

            if (tmp == source)
                tmp = null;

            for (var x = 0; x < source.Width; x++)
                for (var y = 0; y < source.Height; y++)
                {
                    double value = 0;

                    for (var dy = -ky; dy <= ky; dy++)
                        value += tmp.GetPixel(x, y - dy, borderWrap) * kernel.GetYVector().GetAt(dy + ky, 0);

                    target.Set(x, y, value);
                }
        }

        public static void NonSeparable(Mat source, Mat target, Mat kernel, BorderWrapType borderWrap)
        {
            int kx = kernel.Width / 2,
                ky = kernel.Height / 2;

            for (var x = 0; x < source.Width; x++)
                for (var y = 0; y < source.Height; y++)
                {
                    double value = 0;

                    for (var dx = -kx; dx <= kx; dx++)
                        for (var dy = -ky; dy <= ky; dy++)
                            value += source.GetPixel(x - dx, y - dy, borderWrap) * kernel.GetAt(dx + kx, dy + ky);

                    target.Set(x, y, value);
                }
        }

        public static double ConvolveCell(Mat source, Mat kernel, int x, int y)
        {
            double result = 0;
            var w2 = kernel.Width / 2;
            var h2 = kernel.Height / 2;

            for (var kx = 0; kx < kernel.Width; kx++)
                for (var ky = 0; ky < kernel.Height; ky++)
                    result += source.GetPixel(x - kx + w2, y - ky + h2, BorderWrapType.Mirror) * kernel.GetPixel(kx, ky, BorderWrapType.Mirror);

            return result;
        }

    }
}
