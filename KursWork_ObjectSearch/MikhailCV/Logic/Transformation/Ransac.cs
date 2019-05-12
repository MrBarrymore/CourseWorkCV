using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KursWork_ObjectSearch.MikhailCV.Helper;
using KursWork_ObjectSearch.MikhailCV.Logic.Descriptors;
using KursWork_ObjectSearch.MikhailCV.Logic.Points;

namespace KursWork_ObjectSearch.MikhailCV.Logic
{
    public static class Ransac
    {
        private const int Iterations = 100;
        private const int MatchesCount = 4;
        private const int MatrixWidth = 9;
        private const int Threshold = 5;

        public static ValueTuple<List<double>, List<double>> CalculateTransform(
            List<ValueTuple<Descriptor, Descriptor>> matches)
        {
            if (matches.Count < MatchesCount)
                throw new ArgumentException("Matches count must be >= " + MatchesCount);

            List<int> inliers = new List<int>();
            List<int> idxAll = Enumerable.Range(0, matches.Count).ToList();

            for (var i = 0; i < Iterations; i++)
            {
                List<int> idx = idxAll.OrderBy(a => Guid.NewGuid()).Take(MatchesCount).ToList();
                var curHomography = FindCurrentHomography(matches, idx, false);
                List<int> curInliers = GetInliers(matches, curHomography);

                if (curInliers.Count > inliers.Count) inliers = curInliers;
            }

            var finalMatch = inliers.Select(x => matches[x]).ToList();
            var finalIdx = Enumerable.Range(0, finalMatch.Count).ToList();

            List<double> h1 = FindCurrentHomography(finalMatch, finalIdx, false);
            List<double> h2 = FindCurrentHomography(finalMatch, finalIdx, true);
            return new ValueTuple<List<double>, List<double>>(h1, h2);
        }

        private static List<int> GetInliers(List<ValueTuple<Descriptor, Descriptor>> matches, List<double> homography)
        {
            var inliers = new List<int>();

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                InterestingPoint
                    a = match.Item1.Point,
                    b = match.Item2.Point;

                var point = Transform(homography, a.getX(), a.getY());
                var distance = MathHelper.SqrtOfSqrSum(point.Item1 - b.getX(), point.Item2 - b.getY());

                if (distance < Threshold) inliers.Add(i);
            }

            return inliers;
        }

        private static List<double> FindCurrentHomography(List<ValueTuple<Descriptor, Descriptor>> matches,
            List<int> choices, bool reverse)
        {
            double[,] Homography = new double[2 * choices.Count, MatrixWidth];

            var index = 0;
            foreach (var value in choices)
            {
                var match = matches[value];
                InterestingPoint a = match.Item1.Point, b = match.Item2.Point;

                if (reverse) (a, b) = (b, a);

                int i1 = 2 * index, i2 = 2 * index + 1;

                Homography[i1, 0] = a.getX();
                Homography[i1, 1] = a.getY();
                Homography[i1, 2] = 1;
                Homography[i1, 3] = 0;
                Homography[i1, 4] = 0;
                Homography[i1, 5] = 0;
                Homography[i1, 6] = -b.getX() * a.getX();
                Homography[i1, 7] = -b.getX() * a.getY();
                Homography[i1, 8] = -b.getX();

                Homography[i2, 0] = 0;
                Homography[i2, 1] = 0;
                Homography[i2, 2] = 0;
                Homography[i2, 3] = a.getX();
                Homography[i2, 4] = a.getY();
                Homography[i2, 5] = 1;
                Homography[i2, 6] = -b.getY() * a.getX();
                Homography[i2, 7] = -b.getY() * a.getY();
                Homography[i2, 8] = -b.getY();

                index++;
            }

            var m = Homography.GetLength(1);
            var n = Homography.GetLength(1);
            var k = Homography.GetLength(0);
            var mat = new double[m, n];

            alglib.rmatrixgemm(m, n, k, 1,
                Homography, 0, 0, 1,
                Homography, 0, 0, 0,
                0,
                ref mat, 0, 0
            );

            // SV decomposition. A = U * S * V^T
            m = mat.GetLength(0);
            n = mat.GetLength(1);
            alglib.rmatrixsvd(mat, m, n, 2, 0, 2, out var w, out var u, out _);

            var minIndex = Array.IndexOf(w, w.Min());

            var scale = 1 / u[MatrixWidth - 1, minIndex];
            var result = new List<double>();
            for (var i = 0; i < MatrixWidth; i++) result.Add(scale * u[i, minIndex]);

            return result;
        }

        public static ValueTuple<double, double> Transform(List<double> h, int x, int y)
        {
            var newX = (h[0] * x + h[1] * y + h[2]) / (h[6] * x + h[7] * y + h[8]);
            var newY = (h[3] * x + h[4] * y + h[5]) / (h[6] * x + h[7] * y + h[8]);

            return new ValueTuple<double, double>(newX, newY);
        }
    }
}
