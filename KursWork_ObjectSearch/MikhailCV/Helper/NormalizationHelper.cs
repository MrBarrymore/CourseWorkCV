using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;

namespace KursWork_ObjectSearch.MikhailCV.Helper
{
    class NormalizationHelper
    {
        public static Mat Normalization(Mat source)
        {
            var target = new Mat(source.Width, source.Height);
            Normalization(source, target);
            return target;
        }

        private static void Normalization(Mat source, Mat target, double newMin = 0, double newMax = 1)
        {
            Normalization(source.GetData(), target.GetData(), newMin, newMax);
        }

        private static void Normalization(double[] source, double[] target, double newMin, double newMax)
        {
            double min = source[0], max = source[0];
            foreach (var value in source)
            {
                if (double.IsNaN(value))
                    continue;

                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }

            for (var i = 0; i < source.Length; i++)
                target[i] = (source[i] - min) * (newMax - newMin) / (max - min) + newMin;
        }

        public static Mat Normalization(Mat source, double newMin, double newMax)
        {
            var result = new Mat(source.Width, source.Height);

            double min = source.GetAt(0, 0), max = source.GetAt(0, 0);
            foreach (var value in source.GetData())
            {
                if (double.IsNaN(value))
                    continue;

                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }

            for (var j = 0; j < source.Width; j++)
            for (var i = 0; i < source.Height; i++)
            {
                result.Set(i, j, (source.GetAt(i, j) - min) * (newMax - newMin) / (max - min) + newMin);
            }

            return result;
        }
    }
}
