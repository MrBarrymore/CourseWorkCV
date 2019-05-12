using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.enums;
using KursWork_ObjectSearch.MikhailCV.Logic.Convolution;
using KursWork_ObjectSearch.MikhailCV.Logic.Image;
using KursWork_ObjectSearch.MikhailCV.Logic.Points;

namespace KursWork_ObjectSearch.MikhailCV.Logic.Descriptors
{
    public static class RotationInvariant
    {
        private const int GridSize = 4;
        private const int BinsCount = 36;

        public static List<Descriptor> Calculate(Mat image, List<InterestingPoint> points)
        {
            var dx = new Mat(image.Width, image.Height);
            var dy = new Mat(image.Width, image.Height);
            var gradient = SobelHelper.Sobel(image, dx, dy, BorderWrapType.Mirror);

            return Calculate(dx, dy, gradient, points);
        }

        private static List<Descriptor> Calculate(Mat dx, Mat dy, Mat gradient, List<InterestingPoint> points)
        {
            var descriptors = new List<Descriptor>();
            var step = 2 * Math.PI / BinsCount;

            foreach (var point in points)
            {
                var scale = 1.6;

                if (point.Radius != -1) scale = point.Radius / Math.Pow(2, point.Octave + 1);

                // var full = FindDescriptor.CalculateForPointWithRotation(gradient, dx, dy, scale * 1.5, 1, BinsCount, point, 0);
                var full = FindDescriptor.CalculateForPointWithRotation(gradient, dx, dy, scale * 1.5, 1, BinsCount, point, 0);

                var peaks = FindPeaks(full);
                var angles = new List<double>();
                InterpolatePeak(angles, full.GetData(), peaks[0], step);

                if (peaks.Count == 2)
                    if (full.Get(peaks[1]) / full.Get(peaks[0]) > 0.8)
                        InterpolatePeak(angles, full.GetData(), peaks[1], step);

                point.Angles = angles;

                foreach (var angle in angles)
                    descriptors.Add(new Descriptor
                    {
                        Angle = angle,
                        Point = point,
                        Vector = FindDescriptor.CalculateForPointWithRotation(gradient, dx, dy, scale * 3,
                            GridSize, 8, point, angle)
                    });
            }

            return descriptors;
        }

        private static List<int> FindPeaks(Mat vector)
        {
            var a = vector.GetData();

            var maxList = new List<ValueTuple<double, int>>();
            for (var i = 1; i < a.Length - 1; i++)
                if (a[i] > a[i + 1] && a[i] > a[i - 1])
                    maxList.Add(new ValueTuple<double, int>(a[i], i));

            maxList.Sort();
            maxList.Reverse();

            return maxList.Take(2).Select(x => x.Item2).ToList();
        }

        private static void InterpolatePeak(List<double> list, double[] hist, int index, double fullStep)
        {
            var left = index == 0 ? hist.Length - 1 : index - 1;
            var right = index == hist.Length - 1 ? 0 : index + 1;

            if (hist[left] >= hist[index] || hist[index] <= hist[right]) return;

            var result = index + 0.5 * (hist[left] - hist[right])
                         / (hist[left] - 2 * hist[index] + hist[right]);

            if (result < 0) result += hist.Length;
            if (result >= hist.Length) result -= hist.Length;

            list.Add(2 * Math.PI - result * fullStep);
        }
    }
}
