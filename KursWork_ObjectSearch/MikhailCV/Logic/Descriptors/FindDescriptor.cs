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
    public class FindDescriptor
    {
        private static int BINS_COUNT = 8;
        private static int GRID_SIZE = 4;
        private static int BLOCK_SIZE = 4;

        public static List<Descriptor> Calculate(Mat image, List<InterestingPoint> points)
        {
            var dx = new Mat(image.Width, image.Height);
            var dy = new Mat(image.Width, image.Height);
            var gradient = SobelHelper.Sobel(image, dx, dy, BorderWrapType.Mirror);

            var descriptors = new List<Descriptor>();
            var step = 2 * Math.PI / BINS_COUNT;

            foreach (var center in points)
            {
                var bins = new double[BINS_COUNT];

                int from = GRID_SIZE * BLOCK_SIZE / 2,
                    to = GRID_SIZE * BLOCK_SIZE - from;

                for (var u = -from; u < to; u++)
                {
                    for (var v = -from; v < to; v++)
                    {
                        int x = center.getX() + u,
                            y = center.getY() + v;

                        var theta = Math.Atan2(dy.GetPixel(x, y, BorderWrapType.Mirror),
                                        dx.GetPixel(x, y, BorderWrapType.Mirror)) + Math.PI;
                        var magnitude = gradient.GetPixel(x, y, BorderWrapType.Mirror);

                        var leftBin = Math.Min((int) Math.Floor(theta / step), BINS_COUNT - 1);
                        var rightBin = (leftBin + 1) % BINS_COUNT;

                        var ratio = theta % step / step;
                        bins[leftBin] += magnitude * (1 - ratio);
                        bins[rightBin] += magnitude * ratio;
                    }
                }

                var vector = new Vector(BINS_COUNT, bins).Normalize();

                descriptors.Add(new Descriptor
                {
                    Point = center,
                    Vector = vector
                });
            }

            return descriptors;
        }

        public static Vector CalculateForPointWithRotation(Mat gradient, Mat dx, Mat dy, double sigma,
            int gridSize, int binsCount, InterestingPoint center, double alpha)
        {
            var expScale = -1 / (2 * sigma * sigma);
            var radius = (int) Math.Round(3 * sigma);

            return CalculateForPointWithRotation(gradient, dx, dy, expScale, gridSize, radius, binsCount, center,
                alpha, true);
        }


        private static Vector CalculateForPointWithRotation(Mat gradient, Mat dx, Mat dy, double expScale,
            int gridSize, double radius, int binsCount, InterestingPoint center, double alpha, bool affine)
        {
            var centerX = center.getX() / (1 << center.Octave);
            var centerY = center.getY() / (1 << center.Octave);

            var size = (int) (radius * 2 + 1);
            var blockSize = size / gridSize;
            var step = 2 * Math.PI / binsCount;

            double cos = Math.Cos(-alpha),
                sin = Math.Sin(-alpha);

            var bins = new Dictionary<ValueTuple<int, int>, List<double>>();

            for (var u = -radius; u < radius; u++)
            for (var v = -radius; v < radius; v++)
            {
                int x = (int) (centerX + u),
                    y = (int) (centerY + v);

                var magnitude = gradient.GetPixel(x, y, BorderWrapType.Copy);
                var theta = Math.Atan2(dy.GetPixel(x, y, BorderWrapType.Copy), dx.GetPixel(x, y, BorderWrapType.Copy));
                if (theta < 0) theta += Math.PI * 2;

                var rotatedU = u * cos + v * sin;
                var rotatedV = v * cos - u * sin;

                var row = (rotatedV + size / 2D) / blockSize;
                var column = (rotatedU + size / 2D) / blockSize;

                if (column < 0 || column >= gridSize || row < 0 || row >= gridSize) continue;

                magnitude *= Math.Exp(expScale * (rotatedU * rotatedU + rotatedV * rotatedV));

                var rotatedTheta = theta + alpha;
                if (rotatedTheta > Math.PI * 2) rotatedTheta -= Math.PI * 2;

                var ratio = rotatedTheta % step / step;
                var leftBin = Math.Min((int) Math.Floor(rotatedTheta / step), binsCount - 1);
                var rightBin = (leftBin + 1) % binsCount;

                if (!bins.ContainsKey(((int) row, (int) column)))
                    bins.Add(((int) row, (int) column), Enumerable.Repeat(0D, binsCount).ToList());

                if (!affine)
                {
                    PutToBins(bins, (int) row, (int) column, 1, leftBin, rightBin, ratio, magnitude);
                }
                else
                {
                    var i = (int) row;
                    if (row - i <= 0.5) i--;

                    var j = (int) column;
                    if (column - j <= 0.5) j--;

                    var ki = 1 - Math.Abs(column - (j + 0.5));
                    var kj = 1 - Math.Abs(row - (i + 0.5));

                    PutToBins(bins, i, j, ki * kj, leftBin, rightBin, ratio, magnitude);
                    PutToBins(bins, i, j + 1, (1 - ki) * kj, leftBin, rightBin, ratio, magnitude);
                    PutToBins(bins, i + 1, j, ki * (1 - kj), leftBin, rightBin, ratio, magnitude);
                    PutToBins(bins, i + 1, j + 1, (1 - ki) * (1 - kj), leftBin, rightBin, ratio, magnitude);
                }
            }

            var result = new Vector(gridSize * gridSize * binsCount);

            for (var i = 0; i < gridSize; i++)
            for (var j = 0; j < gridSize; j++)
            for (var k = 0; k < binsCount; k++)
            {
                double value = 0;
                if (bins.ContainsKey((i, j))) value = bins[(i, j)][k];

                result.Set(i * gridSize * binsCount + j * binsCount + k, value);
            }

            return result.Normalize();
        }

        private static void PutToBins(Dictionary<ValueTuple<int, int>, List<double>> bins, int row, int column,
            double k, int leftBin, int rightBin, double ratio, double magnitude)
        {
            if (!bins.ContainsKey((row, column))) return;

            bins[(row, column)][leftBin] += k * magnitude * (1 - ratio);
            bins[(row, column)][rightBin] += k * magnitude * ratio;
        }

    }
}
