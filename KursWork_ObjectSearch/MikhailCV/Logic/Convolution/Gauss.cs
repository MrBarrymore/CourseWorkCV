using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Convolution
{
    public static class Gauss
    {
        public static Kernel GetKernel(double sigma)
        {
            return GetKernel((int)Math.Round(3 * sigma), sigma);
        }

        private static Kernel GetKernel(int k, double sigma)
        {
            var vector = new double[k * 2 + 1];
            var prefix = 1.0 / (Math.Sqrt(2 * Math.PI) * sigma);

            for (var x = 0; x <= k; x++)
                vector[-x + k] = vector[x + k] =
                    prefix * Math.Exp(-MathHelper.Sqr(x) / (2 * MathHelper.Sqr(sigma)));

            double sum = 0;
            for (var x = -k; x <= k; x++) sum += vector[x + k];

            for (var x = -k; x <= k; x++) vector[x + k] /= sum;

            var mat = new Mat(vector.Length, 1, vector);
            return Kernel.For(mat, mat);
        }

        public static Mat GetFullKernel(double sigma)
        {
            var halfSize = (int)Math.Round(3 * sigma);
            var fullSize = halfSize * 2 + 1;

            var data = new double[fullSize * fullSize];
            var k = 1.0 / (2 * Math.PI * sigma * sigma);

            for (var x = -halfSize; x <= halfSize; x++)
            for (var y = -halfSize; y <= halfSize; y++)
            {
                var value = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
                data[(y + halfSize) * fullSize + x + halfSize] = k * value;
            }

            return new Mat(fullSize, fullSize, data);
        }
    }
}
